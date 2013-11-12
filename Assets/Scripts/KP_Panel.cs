/*
 * 移動可能エリアなどを示すパネル（マス目）ブロックにアサインする
 * 色を変えたり簡単に消したりする（？）
 * 普段はレンダリングせず必要なときに色を変えて表示する
 * クリックしたらその座標を返してくれるといい（UnitClickerの役目か）
 */

using UnityEngine;
using System.Collections;

public class KP_Panel : MonoBehaviour {
	
	//MeshRenderer mesh ;
	public int posx ;			//ボード上の座標
	public int posy ;
	
	//パネルの色などを指定する表示タイプ
	public enum TYPE {
		NONE = 0, MOVE, ATTACK, SUMMON, NUM_MAX
	}
	
	// Use this for initialization
	void Awake () {
		//PanelにアタッチされたMeshRendererコンポーネントを取得
		//rendererでアクセスできるので不要か
		//mesh = GetComponent<MeshRenderer>() ;
		
		//レイヤの設定（レイヤ設定で名前を変えてしまうと設定できなくなるはずなので避けること）
		gameObject.layer = LayerMask.NameToLayer("UserPanel") ;
		
		//デフォルトで非表示（レンダリングを切る）
		//"SetActiveRecursively()でdeactivateする"方法もあるようだ
		renderer.enabled = false ;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetPosition (int x, int y, int areaWidth, int areaHeight) {
		posx = x ;
		posy = y ;
		transform.position = new Vector3((float)posx - areaWidth / 2.0f + 0.5f, 0.1f, -((float)posy - areaHeight / 2.0f + 0.5f)) ;
		return ;
	}
	
	//表示を有効にして色を変化させる
	public void EnableDisplay (TYPE type) {
		//表示を有効にする
		renderer.enabled = true ;

		switch(type) {
		case TYPE.MOVE :			//移動範囲は青色
			renderer.material.color = new Color(0.5f, 0.5f, 1.0f, 0.5f) ;
			break ;
		case TYPE.ATTACK :			//攻撃範囲は赤色
			renderer.material.color = new Color(1.0f, 0.5f, 0.5f, 0.5f) ;
			break ;
		case TYPE.SUMMON :			//召喚範囲は緑色
			renderer.material.color = new Color(0.5f, 1.0f, 0.5f, 0.5f) ;
			break ;
		default :
			break;
		}
		
		return ;
	}
	
	public void DisableDisplay () {
		renderer.enabled = false ;
		return ;
	}
}
