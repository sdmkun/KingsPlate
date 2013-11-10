using UnityEngine;
using System.Collections;

public class KP_UnitClicker : MonoBehaviour {
	
	Ray ray ;
	RaycastHit hit ;
	public Camera myCamera ;
	
	public GameObject clickedObject;
	
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			ray = myCamera.ScreenPointToRay(Input.mousePosition);
			//layerMaskを使用することで当たり判定を行いたいオブジェクトを選択できる
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				clickedObject = hit.collider.gameObject;
			}
		}
	}
	
	
}
