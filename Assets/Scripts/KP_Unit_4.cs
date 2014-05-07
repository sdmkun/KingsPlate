using UnityEngine;
using System.Collections;

public class KP_Unit_4 : KP_Unit {

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
		unitId = 4 ;
		if(team == 0) {
			unitName = "RANGER" ;
		} else {
			unitName = "LIZARDMAN" ;
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
		
		//ナイトの動き
		for(int vy = -2; vy <= 2; ++vy) {
			for(int vx = -2; vx <= 2; ++vx) {
				//vxまたはvyが0（十字方向）かvxとvyの絶対値が等しい（斜め）には移動できない->チェスのナイトの動きになる
				if(vx == 0 || vy == 0 || vx == vy || vx == -vy) {
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

}
