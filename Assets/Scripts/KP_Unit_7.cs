using UnityEngine;
using System.Collections;

public class KP_Unit_7 : KP_Unit {

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
		unitId = 7 ;
		if(team == 0) {
			unitName = "HUNTER" ;
		} else {
			unitName = "SATYR" ;
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

		for(int vy = -2; vy <= 2; vy += 2) {
			for(int vx = -2; vx <= 2; vx += 2) {
				//斜め移動はできない
				if( (vx == 0 && vy == 0) || (vx != 0 && vy != 0) ) {
					continue ;
				}
				//今のところ障害物は全て飛び越える
				for(x = posx + vx, y = posy + vy; (x >= 0 && x < board.areaWidth) && (y >= 0 && y < board.areaHeight) ; x += vx, y += vy) {
					if( board.GetMovableArea()[x, y] ) {		//何もなければ移動可能
						movableArea[x, y] = true ;
					} else if( IsThereAttackableEnemy(x, y) ) {	//敵ユニットなら攻撃可能エリアとなる
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

	//SNEAKスキル
	//ボード上の召喚可能なスペースがあるグリッドならどこでも召喚可能
	//自分のユニットに隣接していないグリッドにも,BARRIERエンチャントのグリッドにも召喚できる
	public override bool[,] GetSummonableArea () {
		return board.GetSummonableArea() ;
	}
	
}
