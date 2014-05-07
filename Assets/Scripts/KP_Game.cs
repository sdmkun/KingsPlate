/*
 * GameManagerに持たせるスクリプトコンポーネント
 * ボードとプレイヤを繋ぐ
 * ボードスクリプトにマップを読み込ませたりユニット配置をしたり
 * フェイズ管理をしたりプレイヤのリソース管理をしたり
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//ボード上の座標を指定するクラス 構造体だと参照渡しにならないため
public class Point {
	public int x = 0 ;
	public int y = 0 ;
	public Point(int argx, int argy) {
		x = argx ;
		y = argy ;
	}
}

public class KP_Game : MonoBehaviour {
	
	[HideInInspector]public KP_Player[] players ;
	[HideInInspector]public int playerNum ;		//上のplayersから取得するので代入する必要はない
	[HideInInspector]public bool[] winners ;	//勝利フラグ
	[HideInInspector]public List<KP_Unit>[] unitsCaptured ;	//持ち駒Unit
	
	public KP_Board board ;							//Prefabが入る
	
	[HideInInspector]public List<KP_Panel> panels ;		//(0,0),(1,0),(2,0)...と並ぶ（計算してランダムアクセス可能）
	public KP_Panel panel ;							//Instantiate用
	private Point point ;
	
	public KP_UnitFactory factory;

	[HideInInspector]public int turnPlayer ;
	public PHASE turnPhase;
	public int summonPower ;	//召喚リソース
	public bool summoned ;		//召喚フラグ 一度召喚を行ったターンは（召喚リソースが残っていても）移動は行えない

	private bool replayFlag ;	//リプレイ要求フラグ（シーン読み込み）

	KP_Unit selectedUnit ;
	string unitName;
	
	public enum PHASE {
		UPKEEP = 0, MAIN, MOVE, ATTACK, ATTACKEND, MOVEEND, SUMMON, SUMMONEND, TURNEND, GAMEEND, NUM_MAX
	}
	
	public delegate void SetPhase() ;	//フェイズ（ターン中の行動単位）が移る時の処理　パラメータ初期化や効果適用
	public delegate void PlayPhase() ;	//フェイズ中の処理
	public SetPhase[] setPhase ;
	public PlayPhase[] playPhase ;

	public enum ENCHANT {
		NONE = 0, BARRIER, FREEZE, NUM_MAX
	}
	[HideInInspector]public ENCHANT[][,] areaEnchant ;	//マップのマスにかけられた特殊効果（相手のエリアに効果をかける）
	/*
	 * BARRIER:ユニットを配置できない
	 * FREEZE:このエリア上のユニットは移動できない
	 */

	[HideInInspector]public int[][,] checkedRank ;		//支援効果（効いている総rank）の数（自分のエリアに効果をかける） これがrankを上回ると取ることができる

	public KP_GUIController guiController ;



	// Use this for initialization
	void Awake () {
		board = (KP_Board)Instantiate(board) ;
		board.transform.parent = transform ;
		
		//ユニット生成
		factory = GetComponent<KP_UnitFactory>() ;
		
		players = new KP_Player[2] ;
		players[0] = gameObject.AddComponent<KP_Player>() ;
		players[0].team = 0 ;
		players[1] = gameObject.AddComponent<KP_Player>() ;
		players[1].team = 1 ;

		playerNum = players.Length ;
		turnPlayer = 0 ;
		winners = new bool[playerNum] ;	//既定値はfalse
		unitsCaptured = new List<KP_Unit>[playerNum] ;
		unitsCaptured[0] = new List<KP_Unit>() ;
		unitsCaptured[1] = new List<KP_Unit>() ;
		replayFlag = false ;

		PlaceUnit(13,0, 0, 5) ;
		PlaceUnit(4, 0, 1, 5) ;
		PlaceUnit(6, 0, 2, 5) ;
		PlaceUnit(5, 0, 0, 4) ;
		PlaceUnit(3, 0, 1, 4) ;
		PlaceUnit(2, 0, 0, 3) ;

		PlaceUnit(13,1, 5, 0) ;
		PlaceUnit(4, 1, 4, 0) ;
		PlaceUnit(6, 1, 3, 0) ;
		PlaceUnit(5, 1, 5, 1) ;
		PlaceUnit(3, 1, 4, 1) ;
		PlaceUnit(2, 1, 5, 2) ;

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

		//エンチャントボード
		areaEnchant = new ENCHANT[playerNum][,] ;
		areaEnchant[0] = new ENCHANT[board.areaWidth, board.areaHeight] ;
		areaEnchant[1] = new ENCHANT[board.areaWidth, board.areaHeight] ;

		//支援効果ボード
		checkedRank = new int[playerNum][,] ;
		checkedRank[0] = new int[board.areaWidth, board.areaHeight] ;
		checkedRank[1] = new int[board.areaWidth, board.areaHeight] ;

		//ウィンドウに情報を表示するGUIController
		guiController = GetComponent<KP_GUIController>() ;
		guiController.Initialize() ;
		guiController.SetTurnPlayer(turnPlayer) ;
		guiController.SetInfo("Game Start !") ;

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
		setPhase[(int)PHASE.GAMEEND] = new SetPhase(SetGameend);
		playPhase = new PlayPhase[(int)PHASE.NUM_MAX] ;
		playPhase[(int)PHASE.UPKEEP] = new PlayPhase(PlayUpkeep) ;
		playPhase[(int)PHASE.MAIN] = new PlayPhase(PlayMain);
		playPhase[(int)PHASE.MOVE] = new PlayPhase(PlayMove);
		playPhase[(int)PHASE.ATTACK] = new PlayPhase(PlayAttack);
		playPhase[(int)PHASE.ATTACKEND] = new PlayPhase(PlayAttackend);
		playPhase[(int)PHASE.MOVEEND] = new PlayPhase(PlayMoveend);
		playPhase[(int)PHASE.SUMMON] = new PlayPhase(PlaySummon);
		playPhase[(int)PHASE.SUMMONEND] = new PlayPhase(PlaySummonend);
		playPhase[(int)PHASE.TURNEND] = new PlayPhase(PlayTurnend);
		playPhase[(int)PHASE.GAMEEND] = new PlayPhase(PlayGameend);

		//アップキープフェイズから開始
		ChangePhase(PHASE.UPKEEP) ;
	}
	
	// Update is called once per frame
	void Update () {
		playPhase[(int)turnPhase]() ;
	}
	
	public void ChangeTurn() {
		turnPlayer = (++turnPlayer) % playerNum ;
		guiController.SetTurnPlayer(turnPlayer) ;
		ChangePhase(PHASE.UPKEEP) ;
	}
	
	public void ChangePhase(PHASE phase) {
		turnPhase = phase ;
		
		//各playerのunitFieldに対してphaseの効果を促すSendMessage()を送る
		
		setPhase[(int)turnPhase]() ;
	}
	
	//UPKEEP PHASE
	//フェイズが移る時点で呼ばれるメソッド群
	public void SetUpkeep() {
		summonPower = 0 ;
		summoned = false ;

		//アクティブプレイヤのKP_Unitのステータス初期化,アップキープ処理
		for(int y = 0; y < board.areaHeight; ++y) {
			for(int x = 0; x < board.areaWidth; ++x) {
				if(board.areaUnit[x, y] && board.areaUnit[x, y].team == turnPlayer) {
					board.areaUnit[x, y].InitializeStatus() ; ;
				}
			}
		}
		CallUnitSkill("SetUpkeep") ;

		//アクティブプレイヤの支援効果計算
		CalculateCheckedRank() ;

		ChangePhase(PHASE.MAIN) ;
	}
	
	//フェイズ中毎フレーム呼ばれるメソッド群
	public void PlayUpkeep() {
		
	}
	
	//MAIN PHASE
	public void SetMain() {
		CallUnitSkill("SetMain") ;
		guiController.SetGuide("Select Unit") ;
	}
	
	public void PlayMain() {
		//プレイヤが移動するユニットまたは召喚するユニットを選択して返してくる
		selectedUnit = players[turnPlayer].PlayMain() ;
		if( selectedUnit != null && selectedUnit.team == turnPlayer ) {
			if( !selectedUnit.isCaptured ) {
				if( !summoned && areaEnchant[turnPlayer][selectedUnit.posx, selectedUnit.posy] != ENCHANT.FREEZE ) {
					//選択したUnitが自分のユニットでボード上にあり（キャプチャ状態でなく）このターン召喚を行っておらずFREEZE状態でない場合 移動
					ChangePhase(PHASE.MOVE) ;
					return ;
				}
			} else {
				//選択したUnitがキャプチャ状態の自分のユニットで召喚可能なコストの場合 召喚
				if(selectedUnit.summonCost <= summonPower) {
					ChangePhase(PHASE.SUMMON) ;
					return ;
				}
			}
			selectedUnit = null ; ;
		}
	}
	
	//MOVE PHASE
	public void SetMove() {
		CallUnitSkill("SetMove") ;
		point = null ;
		bool[,] movableArea ;
		if( selectedUnit ) {
//			Debug.Log ( selectedUnit.GetMovableArea() ) ;
			movableArea = selectedUnit.GetMovableArea() ;
			for(int y = 0; y < board.areaHeight; ++y) {
				for(int x = 0; x < board.areaWidth; ++x) {
					if(movableArea[x, y]) {
						if(board.areaUnit[x, y]) {
							panels[x + y * board.areaWidth].EnableDisplay(KP_Panel.TYPE.ATTACK) ;
						} else {
							panels[x + y * board.areaWidth].EnableDisplay(KP_Panel.TYPE.MOVE) ;
						}
					} else {
						panels[x + y * board.areaWidth].DisableDisplay() ;
					}
				}
			}
		}
		guiController.SetGuide("Select square\n where to move") ;
	}
	
	public void PlayMove() {
		//プレイヤが座標を選択して返してくる
		point = players[turnPlayer].PlayMove() ;
		if( point != null ) {
			for(int y = 0; y < board.areaHeight; ++y) {
				for(int x = 0; x < board.areaWidth; ++x) {
					panels[x + y * board.areaWidth].DisableDisplay() ;
				}
			}
			//右クリックで選択をキャンセル（選択座標がエリア外or無効な座標）
			if( point.x < 0 || point.x > board.areaWidth || point.y < 0 || point.y > board.areaHeight ||
				  									 	!(selectedUnit.GetMovableArea()[point.x, point.y]) ) {
				selectedUnit = null ;
				ChangePhase(PHASE.MAIN) ;
				return ;
			} else {	//移動（攻撃）可能な座標であれば移動or攻撃判定
				if(board.areaField[point.x, point.y] == KP_Board.AREA.NONE) {
					MoveUnit(selectedUnit, point.x, point.y) ;
					ChangePhase(PHASE.MOVEEND) ;
				} else {
					ChangePhase(PHASE.ATTACK) ;
				}
			}
		}
	}
	
	//ATTACK PHASE
	public void SetAttack() {
		CallUnitSkill("SetAttack") ;
		MoveUnit(selectedUnit, point.x, point.y) ;
		ChangePhase(PHASE.ATTACKEND) ;
	}
	
	public void PlayAttack() {
		
	}
	
	//ATTACKEND PHASE
	public void SetAttackend() {
		CallUnitSkill("SetAttackend") ;
		ChangePhase(PHASE.MOVEEND) ;
	}
	
	public void PlayAttackend() {
		
	}
	
	//MOVEEND PHASE
	public void SetMoveend() {
		CallUnitSkill("SetMoveend") ;
		selectedUnit = null ;
		ChangePhase(PHASE.TURNEND) ;
	}
	
	public void PlayMoveend() {
		
	}
	
	//SUMMON PHASE
	public void SetSummon() {
		CallUnitSkill("SetSummon") ;
		point = null ;
		bool[,] summonableArea ;
		if( selectedUnit ) {
			//			Debug.Log ( selectedUnit.GetMovableArea() ) ;
			summonableArea = selectedUnit.GetSummonableArea() ;
			for(int y = 0; y < board.areaHeight; ++y) {
				for(int x = 0; x < board.areaWidth; ++x) {
					if(summonableArea[x, y]) {
						panels[x + y * board.areaWidth].EnableDisplay(KP_Panel.TYPE.SUMMON) ;
					} else {
						panels[x + y * board.areaWidth].DisableDisplay() ;
					}
				}
			}
		}
		guiController.SetGuide("Select square\n where to summon") ;
	}
	
	public void PlaySummon() {
		//プレイヤが座標を選択して返してくる
		point = players[turnPlayer].PlayMove() ;
		if( point != null ) {
			for(int y = 0; y < board.areaHeight; ++y) {
				for(int x = 0; x < board.areaWidth; ++x) {
					panels[x + y * board.areaWidth].DisableDisplay() ;
				}
			}
			//右クリックで選択をキャンセル（選択座標がエリア外or無効な座標）
			if( point.x < 0 || point.x > board.areaWidth || point.y < 0 || point.y > board.areaHeight ||
			   !(selectedUnit.GetSummonableArea()[point.x, point.y]) ) {
				selectedUnit = null ;
				ChangePhase(PHASE.MAIN) ;
				return ;
			} else {	//召喚可能な座標であれば召喚
				if(board.areaField[point.x, point.y] == KP_Board.AREA.NONE) {
					SummonUnit(selectedUnit, turnPlayer, point.x, point.y) ;
					ChangePhase(PHASE.SUMMONEND) ;
				}
			}
		}
	}
	
	//SUMMONEND PHASE
	public void SetSummonend() {
		CallUnitSkill("SetSummonend") ;
		ChangePhase(PHASE.TURNEND) ;
	}
	
	public void PlaySummonend() {
		
	}
	
	//TURNEND PHASE
	public void SetTurnend() {
		InitializeEnchant() ;
		CallUnitSkill("SetTurnend") ;

		//勝敗判定（現時点で引き分けはない）
		for(int i = 0; i < playerNum; ++i) {
			if(winners[i]) {
				guiController.SetInfo( "The Winner is Player " + (i + 1) );
				guiController.SetGuide("") ;
				ChangePhase(PHASE.GAMEEND) ;
				return ;
			}
		}

		ChangeTurn () ;
	}
	
	public void PlayTurnend() {
		
	}

	public void SetGameend() {
		guiController.SetInfo("Game is over") ;
	}

	public void PlayGameend() {
		if(replayFlag) {
			Application.LoadLevel(0) ;
		}
	}

	//ユニットを生成して配置する
	public void PlaceUnit(int unitId, int team, int x, int y) {
		KP_Unit unit = factory.GetUnitInstance(unitId) ;
		unit.team = team ;
		unit.SetPosition(x, y) ;
		unit.InitializeUnit() ;		//team情報などの設定
		board.areaUnit[x, y] = unit ;
	}

	//ユニットを移動する 移動先に既に他のユニットがいれば取得する
	public void MoveUnit(KP_Unit unit, int x, int y) {
		if(board.areaUnit[x, y]) {
			guiController.SetInfo("Player" + (turnPlayer + 1) + "'s " + unit.unitName + " beats " + board.areaUnit[x, y].unitName + " on X" + x + "Y" + y) ;
			CaptureUnit(board.areaUnit[x, y].unitId, unit.team) ;
			board.areaUnit[x, y].Die() ;
		} else {
			guiController.SetInfo("Player" + (turnPlayer + 1) + "'s " + unit.unitName + " moves to X" + x + "Y" + y) ;
		}
		board.areaUnit[selectedUnit.posx, selectedUnit.posy] = null ;
		board.areaUnit[x, y] = selectedUnit ;
		selectedUnit.SetPosition(x, y) ;
	}

	//ユニットを持ち駒に加える 
	public void CaptureUnit(int unitId, int team) {
		KP_Unit unit = factory.GetUnitInstance(unitId) ;
		unit.team = team ;
		unit.isCaptured = true ;
		unit.InitializeUnit() ;
		unitsCaptured[team].Add(unit) ;
		for(int i = 0; i < unitsCaptured[team].Count; ++i) {
			unit = unitsCaptured[team][i] ;
			unit.transform.position = new Vector3(3.2f, 0.0f, i * (0.5f - team) + 4.5f * (team - 0.5f) ) ;
			unit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) ;
		}
	}

	//持ち駒を打つ
	public void SummonUnit(KP_Unit unit, int team, int x, int y) {
		guiController.SetInfo("Player" + (team + 1) + " summons " + unit.unitName + " to X" + x + "Y" + y) ;
		PlaceUnit(unit.unitId, team, x, y) ;
		unitsCaptured[team].Remove(unit) ;		//Dieではないが,Destroyされるか分からない
		Destroy(unit.gameObject) ;
	}

	//ユニットにフェイズ毎の特殊スキル(Set[phase]関数)を呼ぶ KingsFieldではボード上のユニットのみスキルを発動させる
	private void CallUnitSkill(string funcName) {
		for(int y = 0; y < board.areaHeight; ++y) {
			for(int x = 0; x < board.areaWidth; ++x) {
				if(board.areaUnit[x, y] && board.areaUnit[x, y].team == turnPlayer) {
					board.areaUnit[x, y].SendMessage(funcName, SendMessageOptions.DontRequireReceiver) ;
				}
			}
		}
	}

	//プレイヤのエンチャントボードを初期化する ターンの終わりに行う
	private void InitializeEnchant () {
		for(int y = 0; y < board.areaHeight; ++y) {
			for(int x = 0; x < board.areaWidth; ++x) {
				areaEnchant[turnPlayer][x, y] = ENCHANT.NONE ;
			}
		}
	}

	//支援効果を計算する 自分のターンのはじめに計算する
	private void CalculateCheckedRank () {
		bool[,] movableArea ;
		//一時的に支援効果を上げておく（全範囲移動可能と仮定する）
		for(int y = 0; y < board.areaHeight; ++y) {
			for(int x = 0; x < board.areaWidth; ++x) {
				checkedRank[turnPlayer][x, y] = 1000 ;
			}
		}

		int[,] tempCheckedRank = new int[board.areaWidth, board.areaHeight] ;
		for(int y = 0; y < board.areaHeight; ++y) {
			for(int x = 0; x < board.areaWidth; ++x) {
				if(board.areaUnit[x, y] && board.areaUnit[x, y].team == turnPlayer) {
					movableArea = board.areaUnit[x, y].GetMovableArea() ;
					for(int ty = 0; ty < board.areaHeight; ++ty) {
						for(int tx = 0; tx < board.areaWidth; ++tx) {
							if( movableArea[tx, ty] ) {
								tempCheckedRank[tx, ty] += board.areaUnit[x, y].rank ;
							}
						}
					}
				}
			}
		}
		checkedRank[turnPlayer] = tempCheckedRank ;
	}
	
	void OnGUI () {
		if(turnPhase == PHASE.GAMEEND) {
			replayFlag = GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 100, 100, 20), "Replay") ;
		}
	}
	
}
