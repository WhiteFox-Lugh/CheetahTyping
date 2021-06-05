﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultShowScript : MonoBehaviour {
	private const double Weight = 1.0 + 2284815.0 / 2406151.0;
	private const int ratingDiffMin = 1;
	private List<Color> ratingColor = new List<Color> {
		new Color(128f / 255f, 128f / 255f, 128f / 255f, 1f),
		new Color(0f / 255f, 0f / 255f, 0f / 255f, 1f),
		new Color(139f / 255f, 69f / 255f, 19f / 255f, 1f),
		new Color(30f / 255f, 30f / 255f, 255f / 255f, 1f),
		new Color(0f / 255f, 191f / 255f, 255f / 255f, 1f),
		new Color(60f / 255f, 179f / 255f, 113f / 255f, 1f),
		new Color(241f / 255f, 211f / 255f, 5f / 255f, 1f),
		new Color(255f / 255f, 140f / 255f, 0f / 255f, 1f),
		new Color(213f / 255f, 30f / 255f, 30f / 255f, 1f),
		new Color(188f / 255f, 87f / 255f, 242f / 255f, 1f)
	};
	private int[] ratingPartition = {
		1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000,
		5500, 6500, 7500, 8500, 10000000
		};
	private bool isLogin;
	private bool isNewRecord;
	private int typeNum;
	private int mistypeNum;
	private double kpm;
	private double accuracy;
	private int tasks;
	private int oldRating;
	private int newRating;
	private int ratingDiff;
	private int gameMode;
	private Dictionary<string, int> mistypeKey;
	private int score;
	public Material ratingRainbow;
	public Material ratingGold;
	public Material ratingSilver;
	public Material ratingCopper;
	// UI
	public Text UIScore;
	public Text UITypeNum;
	public Text UIMisType;
	public Text UIAccuracy;
	public Text UIMistypeKey;
	public Text UIKpm;
	public TextMeshProUGUI UIRating;
	public Text UIRatingDelta;
	public Button ShareTwitterText;
	public Button ShareTwitterImg;

	// Use this for initialization
	void Start () {
		GetData();
		SetUI();
		CalculateScore();
		UpdateHiscore();
		ShowResult();
		GetTweetText();
	}

	// Update is called once per frame
	void Update () {
	}

	// Get data from TypingSoft.cs
	void GetData() {
		typeNum = TypingSoft.CorrectTypeNum;
		mistypeNum = TypingSoft.MisTypeNum;
		accuracy = TypingSoft.Accuracy;
		kpm = TypingSoft.Kpm;
		mistypeKey = TypingSoft.MisTypeDictionary;
		gameMode = TypingSoft.GameMode;
		tasks = TypingSoft.Tasks;
	}

	void UpdateHiscore(){
	}

	void SetUI(){
		UIScore.text = "";
		UITypeNum.text = "";
		UIMisType.text = "";
		UIAccuracy.text = "";
		UIMistypeKey.text = "";
		UIKpm.text = "";
		UIRating.text = "---";
		UIRatingDelta.text = "+0";
	}

	void CalculateScore(){
		score = GetScore();
	}

	void ShowResult() {
		int colorNum = 0;
		int nextRank = 0;
		UIScore.text = score.ToString();
		UITypeNum.text = typeNum.ToString();
		UIMisType.text = mistypeNum.ToString();
		UIAccuracy.text = accuracy.ToString("0.00") + "%";
		ShowMistypeKey();
		UIKpm.text = kpm.ToString("0.00") + " key / min";
		UIRating.text = (!isLogin) ? "---" : newRating.ToString();
		for (int i = 0; i < ratingPartition.Length; ++i){
			if(newRating < ratingPartition[i]){
				colorNum = i;
				nextRank = ratingPartition[i];
				break;
			}
		}
		if(colorNum <= 9){
			UIRating.color = ratingColor[colorNum];
		}
		else if(colorNum == 10){
			UIRating.fontMaterial = ratingCopper;
		}
		else if(colorNum == 11){
			UIRating.fontMaterial = ratingSilver;
		}
		else if(colorNum == 12){
			UIRating.fontMaterial = ratingGold;
		}
		else if(colorNum <= 13){
			UIRating.fontMaterial = ratingRainbow;
		}
		UIRatingDelta.text = (!isLogin) ? "--" : 
			("(+" + ratingDiff.ToString() + ") : Next Rank : " +
			((nextRank <= 10000) ? nextRank.ToString() : " --- "));
	}

	double GetAccuracy (){
		return 1.0 * typeNum / (typeNum + mistypeNum);
	}

	double tasksNumPenalty(){
		if(tasks >= 20){
			return 1.0;
		}
		return (-15.0 / (tasks + 10.0) + 1.5);
	}

	int GetScore(){
		return (int)(tasksNumPenalty() * kpm * (1.05 + Math.Max(-1.0, Weight * Math.Log(GetAccuracy()))));
	}

	void ShowMistypeKey(){
		var sortedMistypeKey = mistypeKey.OrderByDescending((str) => str.Value);
		int i = 0;
		foreach (var str in sortedMistypeKey){
			if(i >= 4){
				break;
			}
			if(i != 0){
				UIMistypeKey.text += ", ";
			}
			UIMistypeKey.text += str.Key + "(" + str.Value.ToString() + "回)";
			++i;
		}
	}

	void KeyCheck(KeyCode k){
		if(KeyCode.Return == k){
			SceneManager.LoadScene("SinglePlayConfigScene");
		}
	}
	void OnGUI() {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.type != EventType.KeyUp && e.keyCode != KeyCode.None
		&& !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)){
			KeyCheck(e.keyCode);
		}
    }

	public void OnClickTweetButton(){
		string tweetText = GetTweetText();
		string url = "";
		string hashTag = "FoxTyping";
		OpenTweetWindow(tweetText, hashTag, url);
	}

	string GetTweetText(){
		string ret = "";
		// const string template = "FoxTyping で_User_難易度 _Difficulty_ でスコア _Score_ をゲット！";
		// ret = template;
		// ret = ret.Replace("_User_", ((isLogin) ? (" " + UserAuth.currentPlayerName + " が") : ("")));
		// ret = ret.Replace("_Score_", score.ToString());
		return ret;
	}

	[DllImport("__Internal")]
    private static extern void OpenTweetWindow(string text, string hashtags, string url);
}
