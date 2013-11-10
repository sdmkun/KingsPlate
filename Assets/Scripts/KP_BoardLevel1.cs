/*
 * KP_Boardの継承クラスはルールに相当
 * 本当はルール一つ一つをスクリプトコンポーネントにしてくっつけていく方法を取りたいが
 */

using UnityEngine;
using System.Collections;

public class KP_BoardLevel1 : KP_Board {

	// Use this for initialization
	protected override void Start () {
		areaWidth = 4 ;
		areaHeight = 4 ;
		//マップ地形は外部ファイル（Excelとか）から読み込めるようにする予定
		areaField = new AREA[,]{	{AREA.NONE,	AREA.NONE,	AREA.WALL,	AREA.NONE},
									{AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.WALL},
									{AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE},
									{AREA.NONE,	AREA.NONE,	AREA.NONE,	AREA.NONE} } ;
		base.Start() ;
	}
	
	// Update is called once per frame
	protected override void Update () {
		
	}
	
	public override int[,] GetMovableArea () {
		return base.GetMovableArea() ;
	}
	
	public override int[,] GetSummonableArea () {
		return base.GetSummonableArea() ;
	}
}
