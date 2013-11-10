using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KP_Player : MonoBehaviour {
	
	public List<KP_Unit> unitHand ;
	public List<KP_Unit> unitField ;
	public int summonPower ;
	public int movePower ;
	
	// Use this for initialization
	void Start () {
		Initialize() ;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Initialize () {
		unitHand = new List<KP_Unit>() ;
		unitField = new List<KP_Unit>() ;
	}
}
