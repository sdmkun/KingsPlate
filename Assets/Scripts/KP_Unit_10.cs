using UnityEngine;
using System.Collections;

public class KP_Unit_10 : KP_Unit {

	//隣接する自分のキングの数
	protected int adjacentKingNum ;

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
		unitId = 10 ;
		if(team == 0) {
			unitName = "CAVALIER" ;
		} else {
			unitName = "DURAHAN" ;
		}

		InitializeStatus() ;
		
		return;
	}

	public override void InitializeStatus () {
		summonCost = 1 ;
		rank = 1 ;
		adjacentKingNum = 0 ;
	}

	//アップキープ効果:キングが隣にいるごとにランクが1上がる
	public void SetUpkeep () {
		int x, y ;
		for(int vy = -1; vy <= 1; ++vy) {
			for(int vx = -1; vx <= 1; ++vx) {
				if(vx == 0 && vy == 0) {
					continue ;
				}
				x = posx + vx ;
				y = posy + vy ;
				if(x >= 0 && x < board.areaWidth && y >= 0 && y < board.areaHeight &&
				   					board.areaUnit[x, y] && board.areaUnit[x, y].team == team && board.areaUnit[x, y].unitId == 13) {
					++adjacentKingNum ;
				}
			}
		}
		rank += adjacentKingNum ;
	}

	//ターン終了時（ユニット移動or召喚後）にランク再計算
	public void SetTurnend () {
		rank -= adjacentKingNum ;
		adjacentKingNum = 0 ;
		int x, y ;
		for(int vy = -1; vy <= 1; ++vy) {
			for(int vx = -1; vx <= 1; ++vx) {
				if(vx == 0 && vy == 0) {
					continue ;
				}
				x = posx + vx ;
				y = posy + vy ;
				if(x >= 0 && x < board.areaWidth && y >= 0 && y < board.areaHeight &&
				   board.areaUnit[x, y] && board.areaUnit[x, y].team == team && board.areaUnit[x, y].unitId == 13) {
					++adjacentKingNum ;
				}
			}
		}
		rank += adjacentKingNum ;
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
				//vxまたはvyが0（水平/垂直移動）なら移動しない
				if(vx == 0 || vy == 0) {
					continue ;
				}
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
}
