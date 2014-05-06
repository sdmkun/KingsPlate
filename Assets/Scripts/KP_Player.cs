using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KP_Player : MonoBehaviour {
	public int summonPower ;
	public int movePower ;
	public int team ;

	KP_UnitClicker unitClicker ;
	GameObject clickedUnit ;
	int maskUserUnit ;								//マスク値
	int maskUserPanel ;
	
	// Use this for initialization
	void Awake () {
		unitClicker = gameObject.AddComponent<KP_UnitClicker>() ;
		clickedUnit = null ;
		maskUserUnit = 1 << LayerMask.NameToLayer("UserUnit") ;
		maskUserPanel = 1 << LayerMask.NameToLayer("UserPanel") ;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public KP_Unit PlayMain() {
		clickedUnit = unitClicker.GetClickedObject(maskUserUnit) ;
		//自分のコントロールするユニットなら選択
		if(clickedUnit && clickedUnit.GetComponent<KP_Unit>().team == team) {
			//return new Point(clickedUnit.GetComponent<KP_Unit>().posx, clickedUnit.GetComponent<KP_Unit>().posy) ;
			return clickedUnit.GetComponent<KP_Unit>() ;
		}
		return null ;
	}

	public Point PlayMove() {
		//右クリックで選択をキャンセル
		if( Input.GetMouseButtonDown(1) ) {
			clickedUnit = null ;
			return new Point(-1, -1);
		}
		//移動先を返す
		clickedUnit = unitClicker.GetClickedObject(maskUserPanel) ;
		if(clickedUnit) {
			return new Point(clickedUnit.GetComponent<KP_Panel>().posx, clickedUnit.GetComponent<KP_Panel>().posy) ;
		}
		return null ;
	}
	
	public void PlayAttack() {
		
	}
	
	public void PlayAttackend() {
		
	}

	public void PlayMoveend() {
		
	}

	public Point PlaySummon() {
		//右クリックで選択をキャンセル
		if( Input.GetMouseButtonDown(1) ) {
			clickedUnit = null ;
			return new Point(-1, -1);
		}
		//移動先を返す
		clickedUnit = unitClicker.GetClickedObject(maskUserPanel) ;
		if(clickedUnit) {
			return new Point(clickedUnit.GetComponent<KP_Panel>().posx, clickedUnit.GetComponent<KP_Panel>().posy) ;
		}
		return null ;
	}

	public void PlaySummonend() {
		
	}

}
