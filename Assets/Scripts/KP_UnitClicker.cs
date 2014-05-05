using UnityEngine;
using System.Collections;

public class KP_UnitClicker : MonoBehaviour {
	
	Ray ray ;
	RaycastHit hit ;
	public Camera myCamera ;
	
	public GameObject clickedObject ;
	
	
	
	// Use this for initialization
	void Awake () {
		myCamera = Camera.main ;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	//指定したレイヤーマスクで左クリックされたオブジェクトを返す(なければnull)
	public GameObject GetClickedObject(int mask) {
		clickedObject = null ;
		if (Input.GetMouseButtonDown(0)) {
			ray = myCamera.ScreenPointToRay(Input.mousePosition) ;
			//layerMaskを使用することで当たり判定を行いたいオブジェクトを選択できる
			//layerMaskで"立っていない"ビットのレイヤに対し当たり判定を調べる
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
				clickedObject = hit.collider.gameObject ;
			}
		}
		return clickedObject ;
	}
	
}
