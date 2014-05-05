/*
 * GameManagerに持たせるスクリプトコンポーネント
 * ボードとプレイヤを繋ぐ
 * ボードスクリプトにマップを読み込ませたりユニット配置をしたり
 * フェイズ管理をしたり
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
	
	public KP_Board board ;							//Prefabが入る
	
	[HideInInspector]public List<KP_Panel> panels ;		//(0,0),(1,0),(2,0)...と並ぶ（計算してランダムアクセス可能）
	public KP_Panel panel ;							//Instantiate用
	private Point point ;
	
	public KP_UnitFactory factory;

	[HideInInspector]public int turnPlayer ;
	public PHASE turnPhase;

	KP_Unit selectedUnit ;
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
		factory = GetComponent<KP_UnitFactory>() ;
		
		players = new KP_Player[2] ;
		players[0] = gameObject.AddComponent<KP_Player>() ;
		players[1] = gameObject.AddComponent<KP_Player>() ;

		playerNum = players.Length ;
		turnPlayer = 0 ;

		PlaceUnit(0, 0, 0, 1) ;
		PlaceUnit(0, 0, 1, 1) ;
		PlaceUnit(0, 1, 0, 0) ;
		PlaceUnit(0, 1, 1, 0) ;

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
		playPhase[(int)PHASE.MAIN] = new PlayPhase(PlayMain);
		playPhase[(int)PHASE.MOVE] = new PlayPhase(PlayMove);
		playPhase[(int)PHASE.ATTACK] = new PlayPhase(PlayAttack);
		playPhase[(int)PHASE.ATTACKEND] = new PlayPhase(PlayAttackend);
		playPhase[(int)PHASE.MOVEEND] = new PlayPhase(PlayMoveend);
		playPhase[(int)PHASE.SUMMON] = new PlayPhase(PlaySummon);
		playPhase[(int)PHASE.SUMMONEND] = new PlayPhase(PlaySummonend);
		playPhase[(int)PHASE.TURNEND] = new PlayPhase(PlayTurnend);
		//アップキープフェイズから開始
		ChangePhase(PHASE.UPKEEP) ;
	}
	
	// Update is called once per frame
	void Update () {
		playPhase[(int)turnPhase]() ;
	}
	
	public void ChangeTurn() {
		turnPlayer = (++turnPlayer) % playerNum ;
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
		ChangePhase(PHASE.MAIN) ;
	}
	
	//フェイズ中毎フレーム呼ばれるメソッド群
	public void PlayUpkeep() {
		
	}
	
	//MAIN PHASE
	public void SetMain() {
		
	}
	
	public void PlayMain() {
		//プレイヤが座標を選択して返してくる
		point = players[turnPlayer].PlayMain() ;
		if( point != null ) {
			selectedUnit = board.areaUnit[point.x, point.y] ;
			ChangePhase(PHASE.MOVE) ;
			return ;
		}
	}
	
	//MOVE PHASE
	public void SetMove() {
		point = null ;
		bool[,] movableArea ;
		if( selectedUnit.GetComponent<KP_Unit>() ) {
//			Debug.Log ( selectedUnit.GetComponent<KP_Unit>().GetMovableArea() ) ;
			movableArea = selectedUnit.GetComponent<KP_Unit>().GetMovableArea() ;
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
				  									 	!(selectedUnit.GetComponent<KP_Unit>().GetMovableArea()[point.x, point.y]) ) {
				selectedUnit = null ;
				ChangePhase(PHASE.MAIN) ;
				return ;
			} else {	//移動（攻撃）可能な座標であれば移動or攻撃判定
				if(board.areaField[point.x, point.y] == KP_Board.AREA.NONE) {
					MoveUnit(selectedUnit, point.x, point.y) ;
					ChangePhase(PHASE.MOVEEND) ;
				} else {
					ChangePhase (PHASE.ATTACK) ;
				}
			}
		}
	}
	
	//ATTACK PHASE
	public void SetAttack() {
		MoveUnit(selectedUnit, point.x, point.y) ;
		ChangePhase(PHASE.ATTACKEND) ;
	}
	
	public void PlayAttack() {
		
	}
	
	//ATTACKEND PHASE
	public void SetAttackend() {
		ChangePhase(PHASE.MOVEEND) ;
	}
	
	public void PlayAttackend() {
		
	}
	
	//MOVEEND PHASE
	public void SetMoveend() {
		selectedUnit = null ;
		ChangePhase(PHASE.TURNEND) ;
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
		ChangeTurn () ;
	}
	
	public void PlayTurnend() {
		
	}

	//ユニットを生成して配置する
	private void PlaceUnit(int type, int team, int x, int y) {
		KP_Unit unit = factory.GetUnitInstance(type) ;
		unit.team = team ;
		unit.SetPosition(x, y) ;
		unit.InitializeUnit() ;		//team情報などの設定
		players[team].unitField.Add(unit) ;
		board.areaUnit[x, y] = unit ;
	}

	//ユニットを移動する 移動先に既に他のユニットがいれば取得する<=未実装
	private void MoveUnit(KP_Unit unit, int x, int y) {
		if(board.areaUnit[x, y]) {
			Destroy(board.areaUnit[x, y].gameObject) ;
		}
		board.areaUnit[selectedUnit.posx, selectedUnit.posy] = null ;
		board.areaUnit[x, y] = selectedUnit ;
		selectedUnit.SetPosition(x, y) ;
	}
	
	void OnGUI () {
		/*
		//最後にクリックされたユニットの名前を返す
		if( GetComponent<KP_UnitClicker>().GetClickedObject( ~(LayerMask.NameToLayer("UserUnit") | LayerMask.NameToLayer("UserPanel")) ) ) {
			unitName = GetComponent<KP_UnitClicker>().GetClickedObject(~(LayerMask.NameToLayer("UserUnit") | LayerMask.NameToLayer("UserPanel"))).name ;
		}
		GUI.Label(new Rect(50 * 0,12 * 0,200,100), "unitName " + unitName );
		*/
	}
	
}
