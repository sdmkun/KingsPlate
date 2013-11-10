/*
 * GameManagerに持たせるスクリプトコンポーネント
 * ボードとプレイヤを繋ぐ
 * ボードスクリプトにマップを読み込ませたりユニット配置をしたり
 * フェイズ管理をしたり
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KP_Game : MonoBehaviour {
	
	public KP_Player[] players ;
	[HideInInspector] public int playerNum ;			//上のplayersから取得するので代入する必要はない
	public KP_Board board ;
	public KP_UnitFactory factory;
	public int turnPlayer ;
	public PHASE turnPhase;
	
	public enum PHASE {
		UPKEEP = 0, MAIN, MOVE, ATTACK, ATTACKEND, MOVEEND, SUMMON, SUMMONEND, TURNEND, NUM_MAX
	}
	
	public delegate void SetPhase() ;	//フェイズ（ターン中の行動単位）が移る時の処理　パラメータ初期化や効果適用
	public delegate void PlayPhase() ;	//フェイズ中の処理
	public SetPhase[] setPhase ;
	public PlayPhase[] playPhase ;
	
	// Use this for initialization
	void Start () {
		factory = (KP_UnitFactory)GetComponent("KP_UnitFactory") ;
		
		players = new KP_Player[2] ;
		players[0] = gameObject.AddComponent<KP_Player>() ;
		players[1] = gameObject.AddComponent<KP_Player>() ;
		
		//AddComponentはStart()を呼んでくれないので
		players[0].Initialize() ;
		players[1].Initialize() ;
		
		playerNum = players.Length ;
		turnPlayer = 0 ;
		
		//SetPosition()を呼ぶタイミングが早すぎる疑惑
		//
		KP_Unit unit = factory.GetUnitInstance(0) ;
		unit.team = 0 ;
		unit.SetPosition(0, 1) ;
		players[0].unitField.Add(unit) ;
		
		unit = factory.GetUnitInstance(0) ;
		unit.team = 0 ;
		unit.SetPosition(1, 1) ;
		players[0].unitField.Add(unit) ;
		
		unit = factory.GetUnitInstance(0) ;
		unit.team = 1 ;
		unit.SetPosition(0, 0) ;
		players[1].unitField.Add(unit) ;
		
		unit = factory.GetUnitInstance(0) ;
		unit.team = 1 ;
		unit.SetPosition(1, 0) ;
		players[1].unitField.Add(unit) ;
		
		setPhase = new SetPhase[(int)PHASE.NUM_MAX] ;
		setPhase[(int)PHASE.UPKEEP] = new SetPhase(SetUpkeep);
		playPhase = new PlayPhase[(int)PHASE.NUM_MAX] ;
		playPhase[(int)PHASE.UPKEEP] = new PlayPhase(PlayUpkeep) ;
		turnPhase = PHASE.UPKEEP ;
	}
	
	// Update is called once per frame
	void Update () {
		playPhase[(int)turnPhase]() ;
	}
	
	public void ChangeTurn() {
		turnPlayer = (++turnPlayer) % playerNum ;
	}
	
	public void ChangePhase(PHASE phase) {
		turnPhase = phase;
		setPhase[(int)turnPhase]() ;
	}
	
	public void SetUpkeep() {
		
	}
	
	public void PlayUpkeep() {
		
	}
	
}
