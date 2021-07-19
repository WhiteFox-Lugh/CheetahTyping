using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 差分表示用
public struct Diff {
	public string op;
	public string before;
	public string after;

	public Diff(string op, string before, string after) {
		this.op = op;
		this.before = before;
		this.after = after;
	}
}

public class LongSentenceScript : MonoBehaviour {
	// diff の タイプ
	const string OP_INSERT = "insert";
	const string OP_DELETE = "delete";
	const string OP_REPLACE = "replace";
	const string OP_EQUAL = "equal";
	// diff の表示色
	const string COLOR_INSERT = "orange";
	const string COLOR_DELETE = "red";
	const string COLOR_REPLACE = "blue";
	// 正解、不正解の重み
	const int CORRECT_SCORE = 1;
	const int MISS_COST = 10;
	const int MISS_COST_MP = 1;
	private double startTime;
	private bool isShowInfo;
	private bool isFinished;
	// UI
	[SerializeField] Text UIResultTextField;
	[SerializeField] Text UITextField;
	[SerializeField] Text UIRestTime;
	[SerializeField] Text UICountDownText;
	[SerializeField] Text UIInputCounter;
	[SerializeField] TextMeshProUGUI UIScoreText;
	[SerializeField] TextMeshProUGUI UIDetailText;
	[SerializeField] InputField UIInputField;
	[SerializeField] GameObject InputPanel;
	[SerializeField] GameObject ResultPanel;
	[SerializeField] GameObject TaskPanel;
	[SerializeField] GameObject InfoPanel;
	[SerializeField] GameObject ScorePanel;
	[SerializeField] GameObject OperationPanel;
	[SerializeField] GameObject ResultOperationPanel;
	// 課題文章
	private string taskText;
	// スコア表示
	private int correctCount = 0;
	private int deleteCount = 0;
	private int insertCount = 0;
	private int replaceCount = 0;
	// 制限時間
	private static int LimitSec {
		set;
		get;
	} = 300;

	/// <summary>
	/// Update() 前の処理
	/// </summary>
	void Awake(){
			Init();
	}

	/// <summary>
	/// 各種初期化
	/// </summary>
	void Init(){
		startTime = 0.0;
		isShowInfo = false;
		isFinished = false;
		UIInputField.interactable = false;
		UITextField.text = "";
		UIInputField.text = "";
		UIRestTime.text = "";
		UIInputCounter.text = "";
		InputPanel.SetActive(true);
		TaskPanel.SetActive(true);
		InfoPanel.SetActive(true);
		ResultPanel.SetActive(false);
		ScorePanel.SetActive(false);
		OperationPanel.SetActive(true);
		ResultOperationPanel.SetActive(false);
		taskText = LoadSentenceData(ConfigScript.LongSentenceTaskName);
		StartCoroutine(CountDown());
	}

	/// <summary>
	/// カウントダウンの処理
	/// </summary>
	IEnumerator CountDown(){
		UICountDownText.text = "3";
		yield return new WaitForSeconds(1.0f);
		UICountDownText.text = "2";
		yield return new WaitForSeconds(1.0f);
		UICountDownText.text = "1";
		yield return new WaitForSeconds(1.0f);
		UICountDownText.text = "";
		AfterCountDown();
	}

	/// <summary>
	/// カウントダウン後の処理
	/// </summary>
	private void AfterCountDown(){
		// 開始時刻取得
		startTime = Time.realtimeSinceStartup;
		// 経過時間と入力文字数の表示
		isShowInfo = true;
		// 課題文表示
		UITextField.text = taskText;
		// 入力フィールドアクティブ化
		UIInputField.interactable = true;
		UIInputField.ActivateInputField();
	}

	/// <summary>
	/// 毎フレーム処理
	/// </summary>
	void Update()
	{
		// フォーカスされていなければ強制フォーカス
		if (!UIInputField.isFocused){
				UIInputField.Select();
		}
		// 必ず文末からしか編集できないようにする
		// インテルステノ方式
		UIInputField.MoveTextEnd(false);
		// 入力中はタイマーを更新
		if (isShowInfo && !isFinished){
				CheckTimer();
				CheckInputStr();
		}
	}

	/// <summary>
	/// タイマーのチェックと更新
	/// </summary>
	private void CheckTimer(){
		var elapsedTime = Time.realtimeSinceStartup - startTime;
		var elapsedTimeInt = Convert.ToInt32(Math.Floor(elapsedTime));
		if (elapsedTimeInt >= LimitSec){
				Finish();
		}
		var restMin = (LimitSec - elapsedTimeInt) / 60;
		var restSec = (LimitSec - elapsedTimeInt) % 60;
		UIRestTime.text = "残り時間: " + restMin.ToString() + " 分 " + restSec.ToString() + " 秒";
	}

	/// <summary>
	/// 入力文字数のカウントチェック
	/// </summary>
	private void CheckInputStr(){
		var inputText = UIInputField.text;
		int inputCount = inputText.Length;
		UIInputCounter.text = "入力文字数: " + inputCount.ToString();
	}

	/// <summary>
	/// 入力終了後の処理
	/// </summary>
	private void Finish(){
		// 表示の切り替え
		ResultPanel.SetActive(true);
		ScorePanel.SetActive(true);
		InfoPanel.SetActive(false);
		InputPanel.SetActive(false);
		TaskPanel.SetActive(false);
		OperationPanel.SetActive(false);
		ResultOperationPanel.SetActive(true);
		UIResultTextField.text = UIInputField.text;
		UIInputField.interactable = false;
		isFinished = true;
		// 得点計算と表示
		ShowScore();
	}

	/// <summary>
	/// スコア表示の処理
	/// </summary>
	private void ShowScore(){
		const string EOS = "{END}";
		// 編集距離の計算
		string taskSentence = taskText;
		string userInputSentence = UIInputField.text;
		List<Diff> diff = GetDiff(taskText, userInputSentence);
		var coloredText = ConvertDiffToHtml(diff);
		SetScoreDetail(diff);
		ShowOriginalScore();
		UIResultTextField.text = coloredText;
	}

	/// <summary>
	/// オリジナル計算のスコアの表示切替
	/// </summary>
	private void ShowOriginalScore(){
		int score = GetOriginalScore();
		var sbScore = new StringBuilder();
		var sbDetail = new StringBuilder();
		sbScore.Append("スコア(F)：").Append(score.ToString());
		sbDetail.Append("正解数：" + correctCount.ToString() + " x " + CORRECT_SCORE.ToString() +"点\n")
						.Append("<color=\"" + COLOR_DELETE + "\">削除：" + deleteCount.ToString())
						.Append(" x (-" + MISS_COST.ToString() + "点)</color> / ")
						.Append("<color=\"" + COLOR_INSERT + "\">余分：" + insertCount.ToString())
						.Append(" x (-" + MISS_COST.ToString() + "点)</color>\n")
						.Append("<color=\"" + COLOR_REPLACE + "\">置換：" + replaceCount.ToString())
						.Append(" x (-" + MISS_COST.ToString() + "点)</color>");
		UIScoreText.text = sbScore.ToString();
		UIDetailText.text = sbDetail.ToString();
	}

	/// <summary>
	/// オリジナルスコアの値取得
	/// </summary>
	private int GetOriginalScore(){
			return correctCount - (deleteCount + insertCount + replaceCount) * MISS_COST;
	}

	/// <summary>
	/// maipaso 形式のスコアの表示切替
	/// </summary>
	private void ShowMpScore(){
		(int score, int spScore) = GetMpScore();
		var sbScore = new StringBuilder();
		var sbDetail = new StringBuilder();
		sbScore.Append("スコア(M)：").Append(score.ToString());
		sbDetail.Append("正解数：" + correctCount.ToString() + " x " + CORRECT_SCORE.ToString() +"点 / ")
						.Append("特別点：" + spScore.ToString() + "\n")
						.Append("<color=\"" + COLOR_DELETE + "\">削除：" + deleteCount.ToString())
						.Append(" x (-" + MISS_COST_MP.ToString() + "点)</color> / ")
						.Append("<color=\"" + COLOR_INSERT + "\">余分：" + insertCount.ToString())
						.Append(" x (-" + MISS_COST_MP.ToString() + "点)</color>\n")
						.Append("<color=\"" + COLOR_REPLACE + "\">置換：" + replaceCount.ToString())
						.Append(" x (-" + MISS_COST_MP.ToString() + "点)</color>");
		UIScoreText.text = sbScore.ToString();
		UIDetailText.text = sbDetail.ToString();
	}

	/// <summary>
	/// maipaso 形式のスコア取得。特別点も取得する。
	/// </summary>
	private (int score, int spScore) GetMpScore(){
		int missCount = deleteCount + insertCount + replaceCount;
		int retSpScore = (missCount <= 3) ? Convert.ToInt32(Math.Ceiling(correctCount * (0.20 - 0.05 * missCount))) : 0;
		int retScore = correctCount - missCount * MISS_COST_MP + retSpScore;
		return (retScore, retSpScore);
	}

	/// <summary>
	/// 正解数、不正解数と不正解の内訳を Diff からカウント
	/// </summary>
	private void SetScoreDetail(List<Diff> diffs){
		correctCount = 0;
		deleteCount = 0;
		insertCount = 0;
		replaceCount = 0;
		foreach (Diff diff in diffs){
			string op = diff.op;
			string beforeText = diff.before;
			string afterText = diff.after;
			if (op.Equals(OP_EQUAL)){
				correctCount += beforeText.Length;
			}
			else if (op.Equals(OP_DELETE)){
				deleteCount += beforeText.Length;
			}
			else if (op.Equals(OP_INSERT)){
				insertCount += afterText.Length;
			}
			else if (op.Equals(OP_REPLACE)){
				replaceCount += beforeText.Length;
			}
		}
	}

	/// <summary>
	/// strA (原文) から strB (入力文) への Diff を取得
	/// </summary>
	private static List<Diff> GetDiff(string strA, string strB){
		var retBackTrace = new List<Diff>() { };

		// 1: 共通の prefix を探す
		int minLen = Math.Min(strA.Length, strB.Length);
		int commonPrefixIndex = -1;
		for (int i = 0; i < minLen; ++i){
			if (strA[i] == strB[i]){
				commonPrefixIndex = i;
			}
			else {
				break;
			}
		}
		string commonPrefix = (commonPrefixIndex == -1) ? "" : strA.Substring(0, commonPrefixIndex + 1);
		string restStrA = (commonPrefixIndex == -1) ? strA : strA.Substring(commonPrefixIndex + 1, strA.Length - commonPrefixIndex - 1);
		string restStrB = (commonPrefixIndex == -1) ? strB : strB.Substring(commonPrefixIndex + 1, strB.Length - commonPrefixIndex - 1);
		// restB が空 -> そこまで全部正解
		if (restStrB.Equals("")){
			retBackTrace.Add(new Diff(OP_EQUAL, commonPrefix, ""));
			return retBackTrace;
		}

		// 2: 入力した文章の最後n文字が課題文に一致するか？
		// ここでの suffix は厳密には suffix ではないが便宜上そう呼ぶことに

		// 入力された文字(strB)の後ろ最大何文字が課題文の一部に含まれるか二分探索
		int lb = 0, ub = restStrB.Length + 1;
		bool hasSuffix = false;
		while (ub - lb > 1){
			int mid = lb + (ub - lb) / 2;
			string subStr = restStrB.Substring(restStrB.Length - mid, mid);
			int idxSubStr = restStrA.IndexOf(subStr);
			if (idxSubStr == -1){
				ub = mid;
			}
			else {
				lb = mid;
				hasSuffix = true;
			}
		}
		int commonSuffixIndex = restStrB.Length - lb;
		string commonSuffix = restStrB.Substring(commonSuffixIndex, lb);

		// 点数をできるだけ大きくしたいので、rest の文字列長の差の絶対値が最小となるように切り取る
		var suffixIndexList = new List<int>();
		int trimSubstrIndex = restStrA.IndexOf(commonSuffix);
		if (hasSuffix){
			int idx = 0;
			int nextIdx;
			do {
				nextIdx = restStrA.IndexOf(commonSuffix, idx);
				idx = nextIdx + 1;
				if (nextIdx != -1){
					suffixIndexList.Add(nextIdx);
				}
			} while(nextIdx != -1);
			int diffAbsMin = Int32.MaxValue;
			var middleStrB = restStrB.Substring(0, commonSuffixIndex);
			foreach (int trimIdx in suffixIndexList){
				Debug.Log("trimidx:" + trimIdx.ToString());
				var middleStrA = restStrA.Substring(0, trimIdx);
				int diff = Math.Abs(middleStrA.Length - middleStrB.Length);
				if (diff <= diffAbsMin){
					trimSubstrIndex = trimIdx;
					diffAbsMin = diff;
				}
			}
		}

		// prefix と suffix をのぞいた残りの文字 (diff を取るべき文字列)
		restStrA = (commonSuffixIndex == restStrB.Length) ? restStrA : restStrA.Substring(0, trimSubstrIndex);
		restStrB = (commonSuffixIndex == restStrB.Length) ? restStrB : restStrB.Substring(0, commonSuffixIndex);

		// 3: 共通 suffix と prefix をのぞいた残りの文字の diff を求める
		// BackTrace で前方一致させるため、Reverse してから DP する

		// 編集距離を求める DP パート
		string src = new string(restStrA.Reverse().ToArray());
		string dst = new string(restStrB.Reverse().ToArray());
		var rows = src.Length + 1;
		var cols = dst.Length + 1;
		int[ , ] d = new int[rows, cols];
		for (int i = 0; i < rows; ++i){
			d[i, 0] = i;
		}
		for (int i = 0; i < cols; ++i){
			d[0, i] = i;
		}
		for (int i = 1; i < rows; ++i){
			for (int j = 1; j < cols; ++j){
				d[i, j] = Math.Min(d[i - 1, j - 1] + ((src[i - 1] == dst[j - 1]) ? 0 : 1),
										Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1));
			}
		}

		// バックトレース
		var tmpBackTrace = BackTrace(src, dst, d);

		// prefix, suffix と統合
		var prefixTrace = (commonPrefix.Equals("")) ? new List<Diff>() { }
												: new List<Diff>() {(new Diff (OP_EQUAL, commonPrefix, ""))};
		var suffixTrace = (commonSuffix.Equals("")) ? new List<Diff>() { }
												: new List<Diff>() {(new Diff (OP_EQUAL, commonSuffix, ""))};
		var trace = ConvertDiff(tmpBackTrace, restStrA, restStrB);
		retBackTrace.AddRange(prefixTrace);
		retBackTrace.AddRange(trace);
		retBackTrace.AddRange(suffixTrace);

		// 4. 最後の2つの Diff 例外処理 (得点をできるだけ大きくするため)

		// delete, equal だった場合
		var len = retBackTrace.Count;
		if (len >= 2 && (retBackTrace[len - 2].op).Equals(OP_DELETE) && (retBackTrace[len - 1].op).Equals(OP_EQUAL)){
			var diff2 = retBackTrace[len - 2];
			var diff1 = retBackTrace[len - 1];
			var delLen = diff2.before.Length;
			var eqLen = diff1.before.Length;
			// 脱字文字コスト + 正解数 よりも 余分文字コストのみの方がスコアが高くなる時置き換え
			if (MISS_COST * delLen > (MISS_COST + 1) * eqLen){
				retBackTrace.RemoveRange(len - 2, 2);
				retBackTrace.Add(new Diff(OP_INSERT, "", diff1.before));
			}
		}
		// replace, delete だった場合は置き換えて削除より余分文字として減点したほうが必ず得点が高い
		else if (len >= 2 && (retBackTrace[len - 2].op).Equals(OP_REPLACE) && (retBackTrace[len - 1].op).Equals(OP_DELETE)){
			var diff2 = retBackTrace[len - 2];
			retBackTrace.RemoveRange(len - 2, 2);
			retBackTrace.Add(new Diff(OP_INSERT, "", diff2.after));
		}
		return retBackTrace;
	}

	/// <summary>
	/// 編集グラフをバックトレース
	/// </summary>
	private static List<(string, (int, int))> BackTrace(string strA, string strB, int[ , ] matrix){
		const int INF = -1000;
		var ALen = strA.Length;
		var BLen = strB.Length;
		int row = ALen;
		int col = BLen;
		var trace = new List<(string, (int, int))>();
		while (row > 0 || col > 0){
			int cost = (row > 0 && col > 0 && (strA[row - 1] == strB[col - 1])) ? 0 : 1;
			int current = matrix[row, col];
			int costA = (row == 0) ? INF : matrix[row - 1, col];
			int costB = (row == 0 || col == 0) ? INF : matrix[row - 1, col - 1];
			int costC = (col == 0) ? INF : matrix[row, col - 1];
			// 置換 or 一致
			if (costB != INF && current == costB + cost){
				if (strA[row - 1] == strB[col - 1]){
					trace.Add((OP_EQUAL, (row - 1, col - 1)));
				}
				else {
					trace.Add((OP_REPLACE, (row - 1, col - 1)));
				}
				row--;
				col--;
			}
			// 挿入
			else if (costC != INF && current == costC + 1){
				trace.Add((OP_INSERT, (row, col - 1)));
				col--;
			}
			// 削除
			else if (costA != INF && current == costA + 1){
				trace.Add((OP_DELETE, (row - 1, col)));
				row--;
			}
		}
		// リバースした文字列のトレースをしたのでインデックスを変更
		var ret = new List<(string, (int, int))>();
		foreach (var p in trace){
				ret.Add((p.Item1, (ALen - p.Item2.Item1 - 1, BLen - p.Item2.Item2 - 1)));
		}
		return ret;
	}

	/// <summary>
	/// バックトレースした行列の座標から Diff へ変換
	/// </summary>
	private static List<Diff> ConvertDiff(List<(string op, (int idxA, int idxB))> opList, string compStrA, string compStrB){
		var ret = new List<Diff>() { };
		int i = 0;
		if (compStrA == ""){
			ret.Add(new Diff(OP_INSERT, "", compStrB));
			return ret;
		}
		else if(compStrB == ""){
			ret.Add(new Diff(OP_DELETE, compStrA, ""));
			return ret;
		}
		while (i < opList.Count){
			var current = opList[i];
			var currentOp = current.op;
			var targetStrA = (current.op).Equals(OP_INSERT) ? "" : compStrA[current.Item2.idxA].ToString();
			var targetStrB = (current.op).Equals(OP_DELETE) ? "" : compStrB[current.Item2.idxB].ToString();
			int j = 0;
			while (i + j + 1 < opList.Count){
				var next = opList[i + j + 1];
				var nextOp = next.op;
				if (nextOp == currentOp){
					j++;
					targetStrA += nextOp.Equals(OP_INSERT) ? "" : compStrA[next.Item2.idxA].ToString();
					targetStrB += nextOp.Equals(OP_DELETE) ? "" : compStrB[next.Item2.idxB].ToString();
				}
				else {
					break;
				}
			}
			if (currentOp.Equals(OP_DELETE)){
				ret.Add(new Diff(currentOp, targetStrA, ""));
			}
			else if (currentOp.Equals(OP_INSERT)){
				ret.Add(new Diff(currentOp, "", targetStrB));
			}
			else if (currentOp.Equals(OP_REPLACE)){
				ret.Add(new Diff(currentOp, targetStrA, targetStrB));
			}
			else if (currentOp.Equals(OP_EQUAL)){
				ret.Add(new Diff(currentOp, targetStrA, ""));
			}
			i += 1 + j;
		}
		return ret;
	}

	/// <summary>
	/// Diff から Html を生成
	/// 文字に色を付けて強調表示を行う
	/// </summary>
	private static string ConvertDiffToHtml (List<Diff> diffs){
		var sb = new StringBuilder();
		foreach (Diff diff in diffs) {
			string beforeText = diff.before.Replace("&", "&amp;").Replace("<", "&lt;")
			.Replace(">", "&gt;").Replace("\n", "&para;<br>");
			string afterText = diff.after.Replace("&", "&amp;").Replace("<", "&lt;")
			.Replace(">", "&gt;").Replace("\n", "&para;<br>");
			if ((diff.op).Equals(OP_EQUAL)){
				sb.Append(beforeText);
			}
			else if((diff.op).Equals(OP_INSERT)){
				sb.Append("<color=\"" + COLOR_INSERT + "\">").Append(afterText).Append("</color>");
			}
			else if((diff.op).Equals(OP_DELETE)){
				sb.Append("<color=\"" + COLOR_DELETE + "\">").Append(beforeText).Append("</color>");
			}
			else if((diff.op).Equals(OP_REPLACE)){
				sb.Append("<color=\"" + COLOR_REPLACE + "\">[").Append(beforeText).Append(",").Append(afterText).Append("]</color>");
			}
		}
		var html = sb.ToString();
		var ret = html.Replace("&para;<br>", "[NL]\n");
		return ret;
	}

	/// <summary>
	/// キーが押されたときなどのイベント処理
	/// </summary>
	void OnGUI() {
		Event e = Event.current;
		var isPushedCtrlKey = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape){
			if (!isFinished && isShowInfo){
				Finish();
			}
			else {
				ReturnConfig();
			}
		}
		else if (!isFinished && e.type == EventType.KeyDown && e.keyCode == KeyCode.V && isPushedCtrlKey){
			Debug.Log("Copy detected");
		}
		else if (isFinished && e.type == EventType.KeyDown){
			if (e.keyCode == KeyCode.M){
				ShowMpScore();
			}
			else if(e.keyCode == KeyCode.F){
				ShowOriginalScore();
			}
			else if(e.keyCode == KeyCode.R){
				Init();
			}
		}
	}

	/// <summary>
	/// Config 画面へ戻る
	/// </summary>
	private static void ReturnConfig(){
		SceneManager.LoadScene("SinglePlayConfigScene");
	}

	/// <summary>
	/// 文書データの読み込み
	/// </summary>
	private static string LoadSentenceData (string dataName){
		var str = "";
		try {
			var file = Resources.Load(dataName);
			str = file.ToString();
		}
		catch {
			return str;
		}
		return str;
	}
}
