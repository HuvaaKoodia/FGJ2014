using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public UnitMain UnitPrefab;
	public List<UnitMain> Units=new List<UnitMain>();



	// Use this for initialization
	void Start () {

		//generate units
		int a=Subs.GetRandom(10,20);

		for(int i=0;i<a;i++){
			var unit=Instantiate(UnitPrefab,new Vector3(Subs.GetRandom(-5f,5f),Subs.GetRandom(-5f,5f)),Quaternion.identity) as UnitMain;
			Units.Add(unit);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
