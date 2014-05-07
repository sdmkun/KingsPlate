using UnityEngine;
using System.Collections;

public class KP_Unit_6 : KP_Unit {

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
		unitId = 6 ;
		if(team == 0) {
			unitName = "SCOUT" ;
		} else {
			unitName = "OAK" ;
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
		
		//ユニットの移動範囲に合わせて
		for(int vy = -1; vy <= 1; ++vy) {
			for(int vx = -1; vx <= 1; ++vx) {
				//横には移動できない
				if(vy == 0) {
					continue ;
				}
				x = posx + vx ;
				y = posy + vy ;
				if(x >= 0 && x < board.areaWidth && y >= 0 && y < board.areaHeight) {
					if( board.GetMovableArea()[x, y] ) {		//何もなければ移動可能
						movableArea[x, y] = true ;
					} else if( IsThereAttackableEnemy(x, y) ) {	//敵ユニットなら攻撃可能エリアとなる
						movableArea[x, y] = true ;
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
