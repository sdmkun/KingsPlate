using UnityEngine;
using System.Collections;

public class KP_Unit : MonoBehaviour {

	public KP_Game game ;		//配置されているゲーム
	public KP_Board board ;		//配置されているゲームのボード
	
	public GameObject model1 ;	//humanチームのモデル
	public GameObject model2 ;	//monsterチームのモデル（その内に配列化）
	private GameObject modelInstance ;	//インスタンス化したモデルのプレハブ

	public int unitId ;			//ユニット種の番号
	public string unitName ;	//表示されるユニット名
	public int summonCost ;		//召喚に必要なコスト
	public int summonPower ;	//召喚できるコスト
	public int rank ;			//倒すのに必要な合計ランク（またはその攻撃力）
	
	public int team ;			//現在所属しているチーム（初期チームの値も保持するか？）
	public bool isCaptured ;	//持ち駒か
	public int posx ;			//ボード上の位置(左:0)
	public int posy ;			//ボード上の位置(上:0)
	public int dir ;			//ボード上での向き(不明)

	/*
	public enum AREA_MOVABLE {
		NONE = 0, MOVE, ATTACK, NUM_MAX
	}
	
	public enum AREA_SUMMONABLE {
		NONE = 0, SUMMON, NUM_MAX
	}
	*/
	
	// Use this for initialization
	virtual protected void Awake () {

		modelInstance = (GameObject)Instantiate(model1) ;
		modelInstance.transform.parent = transform ;
		//modelInstance.name = "SpiritModel" ;
		//コライダコンポーネント追加（クリックの当たり判定用）
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>() ;
		meshCollider.sharedMesh = modelInstance.GetComponent<MeshFilter>().mesh;
		meshCollider.convex = true;	//メッシュコライダを凸型にする

		//レイヤーマスク設定
		gameObject.layer = LayerMask.NameToLayer("UserUnit") ;
//		modelMesh = (MeshFilter)transform.gameObject.AddComponent("MeshFilter") ;
//		modelMaterials = (MeshRenderer)transform.gameObject.AddComponent("MeshRenderer") ;
//		modelMesh.mesh = ((MeshFilter)(prefab.GetComponent("MeshFilter"))).mesh ;
//		modelMaterials.materials = ((MeshRenderer)(prefab.GetComponent("MeshRenderer"))).materials ;
		isCaptured = false ;
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		
	}
	
	virtual public void InitializeUnit () {
		modelInstance.renderer.material.color = new Color(team == 1 ? 1.0f : 0.2f, 0.2f, team == 0 ? 1.0f : 0.2f, 1.0f) ;
	}
	
	virtual public bool[,] GetMovableArea () {
		return null;
	}

	//デフォルトでは自軍ユニットの周囲のみ召喚可能
	virtual public bool[,] GetSummonableArea () {
		bool[,] summonableArea = new bool[board.areaWidth, board.areaHeight] ;
		int x, y ;
		for(y = 0; y < board.areaHeight; ++y) {
			for(x = 0; x < board.areaWidth; ++x) {
				summonableArea[x, y] = false ;
			}
		}
		
		for(y = 0; y < board.areaHeight; ++y) {
			for(x = 0; x < board.areaWidth; ++x) {
				if(board.areaUnit[x, y] && board.areaUnit[x, y].team == team) {
					for(int vy = -1; vy <= 1; ++vy) {
						for(int vx = -1; vx <= 1; ++vx) {
							if(vx == 0 && vy == 0) {
								continue ;
							}
							if(x + vx >= 0 && x + vx < board.areaWidth && y + vy >= 0 && y + vy < board.areaHeight && !board.areaUnit[x + vx, y + vy]) {
								summonableArea[x + vx, y + vy] = true ;
							}
						}
					}
				}
			}
		}
		return summonableArea ;
	}
	
	//ボード上でのユニットの位置を決めるとtransformの座標が移る
	public void SetPosition (int x, int y) {
		posx = x ;
		posy = y ;
		transform.position = new Vector3((float)posx - board.areaWidth / 2.0f + 0.5f, 0.85f, -((float)posy - board.areaHeight / 2.0f + 0.5f)) ;
		//prefab.transform.localPosition = Vector3.zero ;
	}

	//ユニットを破壊するときは（Destroyではなく）このメソッドを呼ぶ
	virtual public void Die () {
		Destroy(gameObject) ;
	}
	
	void OnGUI () {

		//GUI.Label(new Rect(50 * posx,12 * posy,100,100), "" + ((float)posx - board.areaWidth / 2.0f + 0.5f));
	}
	
}
