using UnityEngine;
using System.Collections;

public class KP_Unit_Ace1 : KP_Unit {

	// Use this for initialization
	protected override void Awake () {
		base.Awake() ;
		InitializeUnit() ;
	}
	
	// Update is called once per frame
	protected override void Update () {
		//base.Update() ;
	}
	
	public override void InitializeUnit () {
		unitId = 1 ;
		if(team == 1) {
			unitName = "SPIRIT" ;
		} else {
			unitName = "DEMON" ;
		}
		prefab.renderer.material.color = new Color(team == 1 ? 1.0f : 0.2f, 0.2f, team == 0 ? 1.0f : 0.2f, 1.0f) ;

		summonCost = 1 ;
		summonPower = 0 ;
		rank = 1 ;
		
		return;
	}
	
	public override bool[,] GetMovableArea () {
		int[,] area = board.GetMovableArea() ;
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
				if(vx == 0 && vy == 0) {
					continue ;
				}
				for(x = posx + vx, y = posy + vy; (x >= 0 && x < board.areaWidth) && (y >= 0 && y < board.areaHeight) ; x += vx, y += vy) {
					if(area[x, y] == (int)KP_Board.AREA.NONE) {		//何もなければ移動可能
						movableArea[x, y] = true ;
					} else if(board.areaUnit[x, y] && board.areaUnit[x, y].team != team) {	//敵ユニットなら攻撃可能エリアとなる
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
	
	public override bool[,] GetSummonableArea () {
		int[,] area = board.GetSummonableArea() ;
		bool[,] summonableArea = new bool[board.areaWidth, board.areaHeight] ;
		
		for(int y = 0; y < board.areaHeight; ++y) {
			for(int x = 0; x < board.areaWidth; ++x) {
				if(area[x, y] == (int)KP_Board.AREA.NONE) {
					summonableArea[x, y] = true ;
				} else {
					summonableArea[x, y] = false ;
				}
			}
		}
		
		return summonableArea ;
	}
	
}
