/*
 * KP_Boardの継承クラスはレベルに相当
 * 本当はルール一つ一つをスクリプトコンポーネントにしてくっつけていく方法を取りたいが
 */

using UnityEngine;
using System.Collections;

public class KP_BoardLevel2 : KP_Board {

	// Use this for initialization
	protected override void Awake () {
		areaWidth = 6 ;
		areaHeight = 6 ;
		//マップ地形は外部ファイル（Excelとか）から読み込めるようにする予定
		areaField = new AREA[,]{	{AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE},
									{AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE},
									{AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE},
									{AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE},
									{AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE},
									{AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE},	} ;
		areaUnit = new KP_Unit[areaWidth, areaHeight] ;

		base.Awake() ;
	}
	
	// Update is called once per frame
	protected override void Update () {
		
	}
}
