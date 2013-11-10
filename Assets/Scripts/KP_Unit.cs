using UnityEngine;
using System.Collections;

public class KP_Unit : MonoBehaviour {
	
	public KP_Board board ;		//配置されているゲームのボード
	
	public Animator model1 ;	//humanチームのモデル
	public Animator model2 ;	//monsterチームのモデル
	MeshFilter modelMesh ;
	MeshRenderer modelMaterials ;
	public GameObject prefab;
	
	public int unitId ;			//ユニット種の番号
	public string unitName ;	//表示されるユニット名
	public int summonCost ;		//召喚に必要なコスト
	public int summonPower ;	//召喚できるコスト
	public int rank ;			//倒すのに必要な合計ランク（またはその攻撃力）
	
	public int team ;			//現在所属しているチーム（初期チームの値も保持するか？）
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
	virtual protected void Start () {
		prefab = (GameObject)Instantiate(prefab) ;
		prefab.transform.parent = transform ;
		prefab.name = "SpiritModel" ;
//		modelMesh = (MeshFilter)transform.gameObject.AddComponent("MeshFilter") ;
//		modelMaterials = (MeshRenderer)transform.gameObject.AddComponent("MeshRenderer") ;
//		modelMesh.mesh = ((MeshFilter)(prefab.GetComponent("MeshFilter"))).mesh ;
//		modelMaterials.materials = ((MeshRenderer)(prefab.GetComponent("MeshRenderer"))).materials ;
		InitializeUnit() ;
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		
	}
	
	virtual public void InitializeUnit () {
		
	}
	
	virtual public bool[,] GetMovableArea () {
		return null;
	}
	
	virtual public bool[,] GetSummonableArea () {
		return null;
	}
	
	virtual public void ShowMovableArea () {
		
	}
	
	virtual public void ShowSummonableArea () {
		
	}
	
	//ボード上でのユニットの位置を決めるとtransformの座標が移る
	public void SetPosition (int x, int y) {
		posx = x ;
		posy = y ;
		//Instantiate後 移動処理をしてからprefabの親子関係を登録している気がする
		transform.position = new Vector3((float)posx - board.areaWidth / 2.0f + 0.5f, 0.85f, -((float)posy - board.areaHeight / 2.0f + 0.5f)) ;
		//transform.Translate(new Vector3((float)x - board.areaWidth / 2.0f + 0.5f, 0.85f, (float)y - board.areaHeight / 2.0f + 0.5f));
		//transform.Translate(new Vector3(1.0f, 2.0f, -3.0f));
		prefab.transform.localPosition = Vector3.zero ;
	}
	
	void OnGUI () {
		GUI.Label(new Rect(50 * posx,12 * posy,100,100), "" + ((float)posx - board.areaWidth / 2.0f + 0.5f));
	}
	
}
