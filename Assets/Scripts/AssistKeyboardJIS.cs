using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Shapes2D;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AssistKeyboardJIS : MonoBehaviour {
	[SerializeField] GameObject AKParent;
	[SerializeField] GameObject AFLParent;
	[SerializeField] GameObject AFRParent;
	// key_name -> GameObject のマップ
	private static Dictionary<string, GameObject> AKKeys = new Dictionary<string, GameObject>();
	// finger_name -> GameObject のマップ
	private static Dictionary<string, GameObject> AFingers = new Dictionary<string, GameObject>();
	// string -> key_name[]
	private static Dictionary<char, string[]> keyMapping = new Dictionary<char, string[]>() {
		{'0', new string[1] {"Key_0"}},
		{'1', new string[1] {"Key_1"}},
		{'2', new string[1] {"Key_2"}},
		{'3', new string[1] {"Key_3"}},
		{'4', new string[1] {"Key_4"}},
		{'5', new string[1] {"Key_5"}},
		{'6', new string[1] {"Key_6"}},
		{'7', new string[1] {"Key_7"}},
		{'8', new string[1] {"Key_8"}},
		{'9', new string[1] {"Key_9"}},
		{'a', new string[1] {"Key_A"}},
		{'b', new string[1] {"Key_B"}},
		{'c', new string[1] {"Key_C"}},
		{'d', new string[1] {"Key_D"}},
		{'e', new string[1] {"Key_E"}},
		{'f', new string[1] {"Key_F"}},
		{'g', new string[1] {"Key_G"}},
		{'h', new string[1] {"Key_H"}},
		{'i', new string[1] {"Key_I"}},
		{'j', new string[1] {"Key_J"}},
		{'k', new string[1] {"Key_K"}},
		{'l', new string[1] {"Key_L"}},
		{'m', new string[1] {"Key_M"}},
		{'n', new string[1] {"Key_N"}},
		{'o', new string[1] {"Key_O"}},
		{'p', new string[1] {"Key_P"}},
		{'q', new string[1] {"Key_Q"}},
		{'r', new string[1] {"Key_R"}},
		{'s', new string[1] {"Key_S"}},
		{'t', new string[1] {"Key_T"}},
		{'u', new string[1] {"Key_U"}},
		{'v', new string[1] {"Key_V"}},
		{'w', new string[1] {"Key_W"}},
		{'x', new string[1] {"Key_X"}},
		{'y', new string[1] {"Key_Y"}},
		{'z', new string[1] {"Key_Z"}},
		{'A', new string[2] {"Key_A", "Key_RShift"}},
		{'B', new string[2] {"Key_B", "Key_RShift"}},
		{'C', new string[2] {"Key_C", "Key_RShift"}},
		{'D', new string[2] {"Key_D", "Key_RShift"}},
		{'E', new string[2] {"Key_E", "Key_RShift"}},
		{'F', new string[2] {"Key_F", "Key_RShift"}},
		{'G', new string[2] {"Key_G", "Key_RShift"}},
		{'H', new string[2] {"Key_H", "Key_LShift"}},
		{'I', new string[2] {"Key_I", "Key_LShift"}},
		{'J', new string[2] {"Key_J", "Key_LShift"}},
		{'K', new string[2] {"Key_K", "Key_LShift"}},
		{'L', new string[2] {"Key_L", "Key_LShift"}},
		{'M', new string[2] {"Key_M", "Key_LShift"}},
		{'N', new string[2] {"Key_N", "Key_LShift"}},
		{'O', new string[2] {"Key_O", "Key_LShift"}},
		{'P', new string[2] {"Key_P", "Key_LShift"}},
		{'Q', new string[2] {"Key_Q", "Key_RShift"}},
		{'R', new string[2] {"Key_R", "Key_RShift"}},
		{'S', new string[2] {"Key_S", "Key_RShift"}},
		{'T', new string[2] {"Key_T", "Key_RShift"}},
		{'U', new string[2] {"Key_U", "Key_LShift"}},
		{'V', new string[2] {"Key_V", "Key_RShift"}},
		{'W', new string[2] {"Key_W", "Key_RShift"}},
		{'X', new string[2] {"Key_X", "Key_RShift"}},
		{'Y', new string[2] {"Key_Y", "Key_LShift"}},
		{'Z', new string[2] {"Key_Z", "Key_RShift"}},
		{' ', new string[1] {"Key_Space"}},
		{'-', new string[1] {"Key_Hyphen"}},
		{',', new string[1] {"Key_Comma"}},
		{'.', new string[1] {"Key_Period"}},
		{';', new string[1] {"Key_Semicolon"}},
		{':', new string[1] {"Key_Colon"}},
		{'[', new string[1] {"Key_LBracket"}},
		{']', new string[1] {"Key_RBracket"}},
		{'@', new string[1] {"Key_At"}},
		{'/', new string[1] {"Key_Slash"}},
		{'!', new string[2] {"Key_1", "Key_RShift"}},
		{'?', new string[2] {"Key_Slash", "Key_LShift"}}
	};

	// string -> key_name[]
	private static Dictionary<string, (int, char)> keyFingering = new Dictionary<string, (int, char)>() {
		{"Key_1", (5, 'L')},
		{"Key_Q", (5, 'L')},
		{"Key_A", (5, 'L')},
		{"Key_Z", (5, 'L')},
		{"Key_2", (4, 'L')},
		{"Key_W", (4, 'L')},
		{"Key_S", (4, 'L')},
		{"Key_X", (4, 'L')},
		{"Key_3", (3, 'L')},
		{"Key_E", (3, 'L')},
		{"Key_D", (3, 'L')},
		{"Key_C", (3, 'L')},
		{"Key_4", (2, 'L')},
		{"Key_R", (2, 'L')},
		{"Key_F", (2, 'L')},
		{"Key_V", (2, 'L')},
		{"Key_5", (2, 'L')},
		{"Key_T", (2, 'L')},
		{"Key_G", (2, 'L')},
		{"Key_B", (2, 'L')},
		{"Key_6", (2, 'R')},
		{"Key_Y", (2, 'R')},
		{"Key_H", (2, 'R')},
		{"Key_N", (2, 'R')},
		{"Key_7", (2, 'R')},
		{"Key_U", (2, 'R')},
		{"Key_J", (2, 'R')},
		{"Key_M", (2, 'R')},
		{"Key_8", (3, 'R')},
		{"Key_I", (3, 'R')},
		{"Key_K", (3, 'R')},
		{"Key_Comma", (3, 'R')},
		{"Key_9", (4, 'R')},
		{"Key_O", (4, 'R')},
		{"Key_L", (4, 'R')},
		{"Key_Period", (4, 'R')},
		{"Key_0", (5, 'R')},
		{"Key_P", (5, 'R')},
		{"Key_Semicolon", (5, 'R')},
		{"Key_Slash", (5, 'R')},
		{"Key_Hyphen", (5, 'R')},
		{"Key_At", (5, 'R')},
		{"Key_Colon", (5, 'R')},
		{"Key_LBracket", (5, 'R')},
		{"Key_RBracket", (5, 'R')},
		{"Key_Space", (1, 'B')},
		{"Key_RShift", (5, 'R')},
		{"Key_LShift", (5, 'R')}
	};
	// キーの色
	private static Color colorGray = new Color(180f / 255f, 180f / 255f, 180f / 255f, 1);
	private static Color colorWhite = new Color(1, 1, 1, 1);
	private static Color colorPink = new Color(1, 110f / 255f, 163f / 255f, 1);
	private static Color colorLightPink = new Color(1, 194f / 255f, 217f / 255f, 1);
	private static Color colorOrange = new Color(251f / 255f, 183f / 255f, 67f / 255f, 1);
	private static Color colorLightOrange = new Color(1, 220f / 255f, 160f / 255f, 1);
	private static Color colorGreen = new Color(49f / 255f, 183f / 255f, 67f / 255f, 1);
	private static Color colorLightGreen = new Color(180f / 255f, 1, 190f / 255f, 1);
	private static Color colorBlue = new Color(58f / 255f, 195f / 255f, 216f / 255f, 1);
	private static Color colorLightBlue = new Color(141f / 255f, 240f / 255f, 1, 1);
	private static Color colorViolet = new Color(207f / 255f, 124f / 255f, 1, 1);
	private static Color colorLightViolet = new Color(234f / 255f, 198f / 255f, 1, 1);

	/// <summary>
	/// 初期化処理
	/// </summary>
	void Awake() {
		GetAllKeys();
		GetAllFingers();
		SetAllKeyColorWhite();
		SetAllFingerColorWhite();
  }

	/// <summary>
	/// キーのオブジェクトを取得する
	/// </summary>
	private void GetAllKeys() {
		AKKeys = new Dictionary<string, GameObject>();
		for (int i = 0; i < AKParent.transform.childCount; ++i){
			var keyboardRows = AKParent.transform.GetChild(i);
			for (int j = 0; j < keyboardRows.transform.childCount; ++j){
				var obj = keyboardRows.transform.GetChild(j).gameObject;
				AKKeys.Add(obj.name, obj);
			}
		}
	}

		/// <summary>
	/// 指のオブジェクトを取得する
	/// </summary>
	private void GetAllFingers() {
		AFingers = new Dictionary<string, GameObject>();
		for (int i = 0; i < AFLParent.transform.childCount; ++i){
			var obj = AFLParent.transform.GetChild(i).gameObject;
			AFingers.Add(obj.name, obj);
		}
		for (int i = 0; i < AFRParent.transform.childCount; ++i){
			var obj = AFRParent.transform.GetChild(i).gameObject;
			AFingers.Add(obj.name, obj);
		}
	}

	/// <summary>
	/// 指定したキーの色を白に設定する
	/// <param name="keyName">キー名</param>
	/// </summary>
	private void SetKeyColorWhite(string keyName){
		var shape = AKKeys[keyName].GetComponent<Shape>();
		shape.settings.outlineColor = colorGray;
		shape.settings.fillColor = colorWhite;
	}

	/// <summary>
	/// 指定したキーの色を変更する
	/// <param name="keyName">キー名</param>
	/// </summary>
	private void SetKeyColorHighlight(string keyName){
		var shape = AKKeys[keyName].GetComponent<Shape>();
		switch(keyFingering[keyName].Item1){
			case 1:
				shape.settings.outlineColor = colorViolet;
				shape.settings.fillColor = colorLightViolet;
				break;
			case 2:
				shape.settings.outlineColor = colorBlue;
				shape.settings.fillColor = colorLightBlue;
				break;
			case 3:
				shape.settings.outlineColor = colorGreen;
				shape.settings.fillColor = colorLightGreen;
				break;
			case 4:
				shape.settings.outlineColor = colorOrange;
				shape.settings.fillColor = colorLightOrange;
				break;
			case 5:
				shape.settings.outlineColor = colorPink;
				shape.settings.fillColor = colorLightPink;
				break;
		}
	}

	/// <summary>
	/// 指定した指の色を変更する
	/// <param name="keyName">キー名</param>
	/// </summary>
	private void SetFingerColorHighlight(string keyName){
		var fingering = keyFingering[keyName];
		if (fingering.Item2 == 'B') {
			var obj = AFingers["L1"].GetComponent<Shape>();
			obj.settings.fillColor = colorViolet;
			obj = AFingers["R1"].GetComponent<Shape>();
			obj.settings.fillColor = colorViolet;
		}
		else {
			var objName = fingering.Item2.ToString() + fingering.Item1.ToString();
			var obj = AFingers[objName].GetComponent<Shape>();
			switch(fingering.Item1){
				case 1:
					obj.settings.outlineColor = colorViolet;
					obj.settings.fillColor = colorLightViolet;
					break;
				case 2:
					obj.settings.outlineColor = colorBlue;
					obj.settings.fillColor = colorLightBlue;
					break;
				case 3:
					obj.settings.outlineColor = colorGreen;
					obj.settings.fillColor = colorLightGreen;
					break;
				case 4:
					obj.settings.outlineColor = colorOrange;
					obj.settings.fillColor = colorLightOrange;
					break;
				case 5:
					obj.settings.outlineColor = colorPink;
					obj.settings.fillColor = colorLightPink;
					break;
			}
		}
	}

	/// <summary>
	/// 全てのキーの色を白にする
	/// </summary>
	public void SetAllKeyColorWhite(){
		foreach (var kvp in keyMapping){
			var keyList = new List<string>(kvp.Value);
			foreach (var keyName in keyList){
				SetKeyColorWhite(keyName);
			}
		}
	}

	/// <summary>
	/// 全ての指の色を白にする
	/// </summary>
	public void SetAllFingerColorWhite(){
		foreach (var kvp in AFingers){
			var obj = kvp.Value.GetComponent<Shape>();
			obj.settings.fillColor = colorWhite;
		}
	}

	/// <summary>
	/// 次に打つべき文字と指をハイライトする
	/// <param name="nextHighlightChar">次に打つ文字</param>
	/// </summary>
	public void SetNextHighlight(char nextHighlightChar){
		SetAllKeyColorWhite();
		SetAllFingerColorWhite();
		var keyList = new List<string>(keyMapping[nextHighlightChar]);
		foreach (var keyName in keyList){
			SetKeyColorHighlight(keyName);
			SetFingerColorHighlight(keyName);
		}
	}
}
