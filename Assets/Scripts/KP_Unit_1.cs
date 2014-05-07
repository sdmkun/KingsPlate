using UnityEngine;
using System.Collections;

public class KP_Unit_1 : KP_Unit {

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
		unitId = 1 ;
		if(team == 0) {
			unitName = "SPIRIT" ;
		} else {
			unitName = "DEMON" ;
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
		
		//キングには攻撃できない
		for(int vy = -1; vy <= 1; ++vy) {
			for(int vx = -1; vx <= 1; ++vx) {
				if(vx == 0 && vy == 0) {
					continue ;
				}
				for(x = posx + vx, y = posy + vy; (x >= 0 && x < board.areaWidth) && (y >= 0 && y < board.areaHeight) ; x += vx, y += vy) {
					if( board.GetMovableArea()[x, y] ) {		//何もなければ移動可能
						movableArea[x, y] = true ;
					} else if(IsThereAttackableEnemy(x, y) && board.areaUnit[x, y].unitId != 13) {		//キングでない敵ユニットなら攻撃可能エリアとなる
						movableArea[x, y] = true ;
						break ;
					} else {
						break ;
					}
				}
			}
		}
		
		return movableArea ;
	}
	
}
