using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public ResourceStore ResStore;
	public List<UnitMain> Units=new List<UnitMain>();

	public BaseMain ABase,BBase,CBase,DBase;   

	// Use this for initialization
	void Start () {

		//generate units

		int a=Subs.GetRandom(10,20);
		for(int i=0;i<a;i++){
			Units.Add(ABase.AddUnit());
		}

		a=Subs.GetRandom(10,20);
		for(int i=0;i<a;i++){
			Units.Add(BBase.AddUnit());
		}

		a=Subs.GetRandom(10,20);
		for(int i=0;i<a;i++){
			Units.Add(CBase.AddUnit());
		}

		a=Subs.GetRandom(10,20);
		for(int i=0;i<a;i++){
			Units.Add(DBase.AddUnit());
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float GetPercentOfMaxPopulation(Nationality Nat){
		//DEV. Opti. save all results in a batch every 5 seconds of so.
		int amount=0;
		foreach(var u in Units){
			if (u.MyNationality==Nat){
				++amount;
			}
		}

		return amount/Units.Count;
	}
}
