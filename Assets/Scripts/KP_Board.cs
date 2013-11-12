using UnityEngine;
using System.Collections;

public class KP_Board : MonoBehaviour {
	
	public int areaWidth;		//マップ幅
	public int areaHeight;		//マップ長さ
	public AREA[,] areaField;		//マップに配置されている障害物など
	public KP_Unit[,] areaUnit;	//マップに配置されているユニット
	
	public enum AREA {
		NONE = 0, WALL, RIVER, UNIT, NUM_MAX
	}
	/*
	 *	NONE:空き
	 *	wall:飛び越せない進入不可エリア
	 *	river:飛び越せる進入不可エリア
	 *	unit:配置されているユニット
	 *	num_max:エリア表現の総数
	 */
	
	//継承先クラスのStartで呼ぶこと
	virtual protected void Awake () {
		ApplyBoardSize() ;
	}
	
	// Update is called once per frame
	virtual protected void Update () {
	
	}
	
	//ボードのwidth,heightに合わせて盤面のサイズを変更
	protected void ApplyBoardSize () {
		transform.localScale = new Vector3(areaWidth, transform.localScale.y, areaHeight) ;
	}
	
	virtual public int[,] GetMovableArea () {
		return (int[,])areaField.Clone() ;
	}
	
	virtual public int[,] GetSummonableArea () {
		int[,] summonableArea = new int[areaWidth, areaHeight] ;
		areaField.CopyTo(summonableArea, 0) ;
		
		for(int j = 0; j < areaHeight; ++j) {
			for(int i = 0; i < areaWidth; ++i) {
				if(areaUnit[i,j] != null) {
					summonableArea[i,j] = (int)AREA.UNIT;
				}
			}
		}
		
		return summonableArea;
	}
}
