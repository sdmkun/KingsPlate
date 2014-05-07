using UnityEngine;
using System.Collections;

public class KP_Unit_8 : KP_Unit {

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
		unitId = 8 ;
		if(team == 0) {
			unitName = "EXORCIST" ;
		} else {
			unitName = "GHOST" ;
		}

		InitializeStatus() ;
		
		return;
	}

	public override void InitializeStatus () {
		summonCost = 1 ;
		rank = 1 ;
	}

	//ターン終了時効果:前方3マス（x方向に広がる）にBARRIERエンチャント（相手はそのグリッドに召喚できない）
	public void SetTurnend () {
		int x = posx ;
		int y = posy + ((team == 0) ? -1 : 1) ;
		for(int vx = -1; vx <= 1; ++vx ) {
			x = posx + vx ;
			if(x >= 0 && x < board.areaWidth && y >= 0 && y < board.areaHeight && !board.areaUnit[x, y]) {
				game.areaEnchant[1 - team][x, y] = KP_Game.ENCHANT.BARRIER ;
			}
		}
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
		
		y = posy + ((team == 0) ? -1 : 1) ;
		//ユニットの移動範囲に合わせて
		for(int vx = -1; vx <= 1; ++vx) {
			x = posx + vx ;
			if(x >= 0 && x < board.areaWidth) {
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
		
		return movableArea ;
	}
	
}
