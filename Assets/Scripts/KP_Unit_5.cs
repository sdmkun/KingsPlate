using UnityEngine;
using System.Collections;

public class KP_Unit_5 : KP_Unit {

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
		unitId = 5 ;
		if(team == 0) {
			unitName = "ARCHER" ;
		} else {
			unitName = "HARPY" ;
		}

		InitializeStatus() ;
		
		return;
	}

	public override void InitializeStatus () {
		summonCost = 1 ;
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

		bool jumped ;	//敵ユニット1体またはWALLを一つ飛び越えることができる
		for(int vy = -1; vy <= 1; ++vy) {
			if(vy == 0) {
				continue ;
			}
			jumped = false ;
			for(x = posx , y = posy + vy; y >= 0 && y < board.areaHeight; y += vy) {
				if( board.GetMovableArea()[x, y] ) {		//何もなければ移動可能
					movableArea[x, y] = true ;
				} else if( IsThereAttackableEnemy(x, y) ) {	//敵ユニットなら攻撃可能エリアとなる
					movableArea[x, y] = true ;
					if( !jumped ) {
						jumped = true ;
					} else {
						break ;
					}
				} else {
					if( !jumped ) {
						jumped = true ;
					} else {
						break ;
					}
				}
			}
		}
		
		return movableArea ;
	}
	
}
