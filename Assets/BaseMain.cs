using UnityEngine;
using System.Collections;

public class BaseMain : MonoBehaviour {

	public GameController GC;
	public Nationality MyNationality;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public UnitMain AddUnit(){
		var unit=Instantiate(GC.ResStore.UnitPrefab,transform.position+new Vector3(Subs.GetRandom(-5f,5f),Subs.GetRandom(-5f,5f)),Quaternion.identity) as UnitMain;
		unit.GC=GC;
		unit.MyNationality=MyNationality;
		unit.transform.parent=GC.ResStore.UnitsContainer;
		return unit;
	}
}
