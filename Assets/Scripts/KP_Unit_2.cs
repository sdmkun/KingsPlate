﻿using UnityEngine;
using System.Collections;

public class KP_Unit_2 : KP_Unit {
	
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
		unitId = 2 ;
		if(team == 0) {
			unitName = "FIGHTER" ;
		} else {
			unitName = "MINOTAUR" ;
		}
		
		summonCost = 1 ;
		summonPower = 0 ;
		rank = 1 ;
		
		return;
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
				if(vx == 0 && vy == 0) {
					continue ;
				}
				x = posx + vx ;
				y = posy + vy ;
				if(x >= 0 && x < board.areaWidth && y >= 0 && y < board.areaHeight) {
					if(board.areaField[x, y] == (int)KP_Board.AREA.NONE && !board.areaUnit[x, y]) {		//何もなければ移動可能
						movableArea[x, y] = true ;
					} else if(board.areaUnit[x, y] && board.areaUnit[x, y].team != team) {	//敵ユニットなら攻撃可能エリアとなる
						movableArea[x, y] = true ;
					}
				}
			}
		}
		
		return movableArea ;
	}
	
	public override bool[,] GetSummonableArea () {
		bool[,] summonableArea = base.GetSummonableArea() ;
		return summonableArea ;
	}
	
}
