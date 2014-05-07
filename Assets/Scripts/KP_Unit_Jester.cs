using UnityEngine;
using System.Collections;

public class KP_Unit_Jester : KP_Unit {

	// Use this for initialization
	protected override void Awake () {
		base.Awake() ;
	}
	
	// Update is called once per frame
	protected override void Update () {
		//base.Update() ;
	}
	
	public override void InitializeUnit () {
		base.InitializeUnit() ;
		unitId = 14 ;
		if(team == 0) {
			unitName = "TALBOT" ;
		} else {
			unitName = "GOBLIN" ;
		}

		InitializeStatus() ;
		
		return;
	}

	public override void InitializeStatus () {
		summonCost = 0 ;
		rank = 1 ;
	}
	
	public override bool[,] GetMovableArea () {
		bool[,] movableArea = new bool[board.areaWidth, board.areaHeight] ;
		int x ;
		int y ;
		
		//movableArea初期化
		for(y = 0; y < board.areaHeight; ++y) {
			for(x = 0; x < board.areaWidth; ++x) {
				movableArea[x, y] = false ;
			}
		}

		x = posx ;
		y = posy + ((team == 0) ? -1 : 1) ;
		//ユニットの移動範囲に合わせて
		if(y >= 0 && y < board.areaHeight) {
			if( board.GetMovableArea()[x, y] ) {		//何もなければ移動可能
				movableArea[x, y] = true ;
			} else if( IsThereAttackableEnemy(x, y) ) {	//敵ユニットなら攻撃可能エリアとなる
				movableArea[x, y] = true ;
			}
		}

		return movableArea ;
	}

}
