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
	
	[HideInInspector]public KP_Player[] players ;
	[HideInInspector] public int playerNum ;		//上のplayersから取得するので代入する必要はない
	
	public KP_Board board ;							//Prefabが入る
	
	[HideInInspector]public List<KP_Panel> panels ;		//(0,0),(1,0),(2,0)...と並ぶ（計算してランダムアクセス可能）
	public KP_Panel panel ;							//Instantiate用
	
	public KP_UnitFactory factory;
	
	[HideInInspector]public KP_UnitClicker unitClicker ;
	GameObject clickedUnit ;						//クリックされたGameObject
	GameObject selectedUnit ;						//選択中ユニット（キャンセルするまで保持）
	int maskUserUnit ;								//マスク値
	int maskUserPanel ;
	
	[HideInInspector]public int turnPlayer ;
	[HideInInspector]public PHASE turnPhase;
	
	string unitName;
	
	public enum PHASE {
		UPKEEP = 0, MAIN, MOVE, ATTACK, ATTACKEND, MOVEEND, SUMMON, SUMMONEND, TURNEND, NUM_MAX
	}
	
	public delegate void SetPhase() ;	//フェイズ（ターン中の行動単位）が移る時の処理　パラメータ初期化や効果適用
	public delegate void PlayPhase() ;	//フェイズ中の処理
	public SetPhase[] setPhase ;
	public PlayPhase[] playPhase ;
	
	// Use this for initialization
	void Awake () {
		//ボード生成（スクリプトで配置しないとStart()の呼ばれるタイミングが親からになってしまう）
		//Instantiate()した直後にStart()が呼ばれるわけではない？
		board = (KP_Board)Instantiate(board) ;
		board.transform.parent = transform ;
		
		//ユニット生成
		factory = (KP_UnitFactory)GetComponent("KP_UnitFactory") ;
		
		players = new KP_Player[2] ;
		players[0] = gameObject.AddComponent<KP_Player>() ;
		players[1] = gameObject.AddComponent<KP_Player>() ;
		
		//AddComponentはStart()を呼んでくれないので
		players[0].Initialize() ;
		players[1].Initialize() ;
		
		playerNum = players.Length ;
		turnPlayer = 0 ;
		
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
		
		//パネル生成
		panels = new List<KP_Panel>() ;
		KP_Panel panelInstance ;
		for(int y = 0; y < board.areaHeight; ++y) {
			for(int x = 0; x < board.areaWidth; ++x) {
				panelInstance = (KP_Panel)Instantiate(panel) ;
				panels.Add(panelInstance) ;
				panelInstance.SetPosition(x, y, board.areaWidth, board.areaHeight) ;		//このようにアクセスするのだ
				panelInstance.transform.parent = transform ;
			}
		}
		panels[0 + 1 * board.areaWidth].EnableDisplay(KP_Panel.TYPE.MOVE) ;
		panels[1 + 2 * board.areaWidth].EnableDisplay(KP_Panel.TYPE.ATTACK) ;
		panels[2 + 3 * board.areaWidth].EnableDisplay(KP_Panel.TYPE.SUMMON) ;
		
		//クリック選択コンポーネント登録
		unitClicker = GetComponent<KP_UnitClicker>() ;
		maskUserUnit = LayerMask.NameToLayer("UserUnit") ;
		maskUserPanel = LayerMask.NameToLayer("UserPanel") ;
		
		//配列にデリゲートでフェイズのメソッドを格納
		setPhase = new SetPhase[(int)PHASE.NUM_MAX] ;
		setPhase[(int)PHASE.UPKEEP] = new SetPhase(SetUpkeep);
		setPhase[(int)PHASE.MAIN] = new SetPhase(SetMain);
		setPhase[(int)PHASE.MOVE] = new SetPhase(SetMove);
		setPhase[(int)PHASE.ATTACK] = new SetPhase(SetAttack);
		setPhase[(int)PHASE.ATTACKEND] = new SetPhase(SetAttackend);
		setPhase[(int)PHASE.MOVEEND] = new SetPhase(SetMoveend);
		setPhase[(int)PHASE.SUMMON] = new SetPhase(SetSummon);
		setPhase[(int)PHASE.SUMMONEND] = new SetPhase(SetSummonend);
		setPhase[(int)PHASE.TURNEND] = new SetPhase(SetTurnend);
		playPhase = new PlayPhase[(int)PHASE.NUM_MAX] ;
		playPhase[(int)PHASE.UPKEEP] = new PlayPhase(PlayUpkeep) ;
		playPhase[(int)PHASE.MAIN] = new PlayPhase(SetMain);
		playPhase[(int)PHASE.MOVE] = new PlayPhase(SetMove);
		playPhase[(int)PHASE.ATTACK] = new PlayPhase(SetAttack);
		playPhase[(int)PHASE.ATTACKEND] = new PlayPhase(SetAttackend);
		playPhase[(int)PHASE.MOVEEND] = new PlayPhase(SetMoveend);
		playPhase[(int)PHASE.SUMMON] = new PlayPhase(SetSummon);
		playPhase[(int)PHASE.SUMMONEND] = new PlayPhase(SetSummonend);
		playPhase[(int)PHASE.TURNEND] = new PlayPhase(SetTurnend);
		//アップキープフェイズから開始
		ChangePhase(PHASE.UPKEEP) ;
	}
	
	// Update is called once per frame
	void Update () {
		playPhase[(int)turnPhase]() ;
	}
	
	public void ChangeTurn() {
		turnPlayer = (++turnPlayer) % playerNum ;
	}
	
	public void ChangePhase(PHASE phase) {
		turnPhase = phase ;
		
		//各playerのunitFieldに対してphaseの効果を促すSendMessage()を送る
		
		setPhase[(int)turnPhase]() ;
	}
	
	//UPKEEP PHASE
	//フェイズが移る時点で呼ばれるメソッド群
	public void SetUpkeep() {
		ChangePhase(PHASE.MAIN) ;
	}
	
	//フェイズ中毎フレーム呼ばれるメソッド群
	public void PlayUpkeep() {
		
	}
	
	//MAIN PHASE
	public void SetMain() {
		
	}
	
	public void PlayMain() {
		clickedUnit = unitClicker.GetClickedObject(maskUserUnit) ;
		if(clickedUnit) {
			selectedUnit = clickedUnit ;
			ChangePhase(PHASE.MOVE) ;
		}
	}
	
	//MOVE PHASE
	public void SetMove() {
		
	}
	
	public void PlayMove() {
		//プレイヤから移動先を受け取る（無効ならRejectする）
	}
	
	//ATTACK PHASE
	public void SetAttack() {
		
	}
	
	public void PlayAttack() {
		
	}
	
	//ATTACKEND PHASE
	public void SetAttackend() {
		
	}
	
	public void PlayAttackend() {
		
	}
	
	//MOVEEND PHASE
	public void SetMoveend() {
		
	}
	
	public void PlayMoveend() {
		
	}
	
	//SUMMON PHASE
	public void SetSummon() {
		
	}
	
	public void PlaySummon() {
		
	}
	
	//SUMMONEND PHASE
	public void SetSummonend() {
		
	}
	
	public void PlaySummonend() {
		
	}
	
	//TURNEND PHASE
	public void SetTurnend() {
		
	}
	
	public void PlayTurnend() {
		
	}
	
	void OnGUI () {
		/*
		 * //最後にクリックされたユニットの名前を返す
		if( GetComponent<KP_UnitClicker>().GetClickedObject( ~(LayerMask.NameToLayer("UserUnit") | LayerMask.NameToLayer("UserPanel")) ) ) {
			unitName = GetComponent<KP_UnitClicker>().GetClickedObject(~(LayerMask.NameToLayer("UserUnit") | LayerMask.NameToLayer("UserPanel"))).name ;
		}
		GUI.Label(new Rect(50 * 0,12 * 0,200,100), "unitName " + unitName );
		*/
	}
	
}
