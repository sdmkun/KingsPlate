using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KP_GUIController : MonoBehaviour {
	public GUISkin guiSkin ;		//使うフォントを入れておく

	GUIStyleState black ;
	GUIStyleState white ;

	GUIStyle guiGuide ;
	string guideStr ;
	Rect guideRect ;

	GUIStyle guiInfo ;
	List<string> infoList ;
	List<float> infoTimer ;
	float infoTime ;

	int turnPlayer ;
	GUIStyle guiTurnPlayer ;

	Rect rect ;

	// Use this for initialization
	void Awake () {
		if( infoList == null ) {
			Initialize() ;
		}
	}

	public void Initialize () {
		black = new GUIStyleState() ;
		white = new GUIStyleState() ;
		white.textColor = new Color(1.0f, 1.0f, 0.9f) ;
		
		//右下でプレイヤに行動を促すメッセージ
		guiGuide = new GUIStyle() ;
		guiGuide.fontSize = 28 ;
		guideRect = new Rect(Screen.width - 480, Screen.height - 80, 300, 50) ;
		
		//情報を一定時間表示する
		guiInfo = new GUIStyle() ;
		infoList = new List<string>() ;
		infoTimer = new List<float>() ;
		infoTime = 10.0f ;
		
		guiTurnPlayer = new GUIStyle() ;
		guiTurnPlayer.fontSize = 36 ;
		turnPlayer = -1 ;
		
		//メッセージ表示の座標指定
		rect = new Rect(0, 0, 300, 50) ;
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < infoTimer.Count; ++i ) {
			infoTimer[i] += Time.deltaTime ;
			if(infoTimer[i] >= infoTime) {	//for文内でRemoveして良いのだろうか
				infoList.RemoveAt(i) ;
				infoTimer.RemoveAt(i) ;
			}
		}
	}

	public void SetGuide (string str) {
		guideStr = str ;
	}

	public void SetInfo (string str) {
		infoList.Add(str) ;
		infoTimer.Add(0.0f) ;
	}

	public void SetTurnPlayer (int arg) {
		turnPlayer = arg ;
	}

	void OnGUI () {
		GUI.skin = guiSkin ;

		GUI.Label(guideRect, guideStr, guiGuide);

		for(int i = 0; i < infoList.Count; ++i) {
			rect.x = 10 ;
			rect.y = Screen.height - 100 + i * 12 ;
			GUI.Label(rect, infoList[i], guiInfo);
		}

		rect.y = 24 ;
		if(turnPlayer == 0) {
			rect.x = 24 ;
			GUI.Label(rect, "Player1 TURN", guiTurnPlayer);
		} else if(turnPlayer == 1) {
			rect.x = Screen.width / 2 + 24 ;
			guiTurnPlayer.normal = white ;
			GUI.Label(rect, "Player2 TURN", guiTurnPlayer);
		}
		guiTurnPlayer.normal = black ;
	}
}
