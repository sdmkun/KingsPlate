using UnityEngine;
using System.Collections;

public class KP_Board : MonoBehaviour {
	
	[HideInInspector]public int areaWidth ;			//マップ幅
	[HideInInspector]public int areaHeight ;		//マップ長さ
	[HideInInspector]public AREA[,] areaField ;		//マップに配置されている障害物など
	[HideInInspector]public KP_Unit[,] areaUnit ;	//マップに配置されているユニット

	public enum AREA {
		NONE = 0, WALL, RIVER, UNIT, NUM_MAX
	}
	public GameObject[] areaFieldPrefab = new GameObject[(int)AREA.NUM_MAX] ;
	protected GameObject[,] areaFieldObject ;
	/*
	 *	NONE:空き
	 *	WALL:飛び越せない進入不可エリア
	 *	RIVER:飛び越せる進入不可エリア
	 *	UNIT:配置されているユニット
	 *	NUM_MAX:エリア表現の総数
	 */
	
	//継承先クラスのStartで呼ぶこと
	virtual protected void Awake () {
		areaFieldObject = new GameObject[areaWidth, areaHeight] ;
		ApplyBoardSize() ;
		DisplayAreaField() ;
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		
	}
	
	//ボードのwidth,heightに合わせて盤面のサイズを変更
	protected void ApplyBoardSize () {
		transform.localScale = new Vector3(areaWidth, transform.localScale.y, areaHeight) ;
	}

	//ボードのエリア表現（障害物等）を表示
	protected void DisplayAreaField() {
		for(int y = 0; y < areaHeight; ++y) {
			for(int x = 0; x < areaWidth; ++x) {
				if(areaFieldObject[x, y]) {
					Destroy(areaFieldObject[x, y]) ;
				}
				if(areaFieldPrefab[(int)areaField[x, y]] != null) {
					areaFieldObject[x, y] = (GameObject)Instantiate( areaFieldPrefab[ (int)areaField[x, y] ] ) ;
					areaFieldObject[x, y].transform.position = new Vector3((float)x - areaWidth / 2.0f + 0.5f, areaFieldObject[x, y].transform.position.y, -((float)y - areaHeight / 2.0f + 0.5f)) ;
				}
			}
		}
	}
	
	virtual public bool[,] GetMovableArea () {
		return GetSummonableArea() ;
	}

	//グリッドに障害物がなくユニットが存在しなければ召喚可能
	virtual public bool[,] GetSummonableArea () {
		bool[,] summonableArea = new bool[areaWidth, areaHeight] ;
		
		for(int y = 0; y < areaHeight; ++y) {
			for(int x = 0; x < areaWidth; ++x) {
				if( areaField[x, y] == AREA.NONE && !areaUnit[x, y] ) {
					summonableArea[x, y] = true ;
				} else {
					summonableArea[x, y] = false ;
				}
			}
		}
		
		return summonableArea;
	}
}
