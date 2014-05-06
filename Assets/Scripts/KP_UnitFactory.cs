using UnityEngine;
using System.Collections;

public class KP_UnitFactory : MonoBehaviour {
	
	public KP_Unit[] units ;
	
	// Use this for initialization
	void Awake () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//ユニットを複製して返す
	public KP_Unit GetUnitInstance (int unitId) {
		KP_Unit unit = (KP_Unit)Instantiate(units[unitId]) ;
		unit.transform.parent = transform ;
		unit.game = GetComponent<KP_Game>() ;
		unit.board = GetComponent<KP_Game>().board ;
		return unit ;
	}
}
