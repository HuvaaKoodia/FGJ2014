using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public ResourceStore ResStore;
	public List<UnitMain> Units=new List<UnitMain>();

	public BaseMain ABase,BBase,CBase,DBase;   

	public int AmountOfDeaths=0,AmountOfSpawns=0,AmountOfDeathsLastMin=0,AmountOfSpawnsLastMin=0;
	public float SecondsAfterStart=0;
	float LastMin=0;

	public void AddDeath(){
		++AmountOfDeaths;
		++AmountOfDeathsLastMin;
	}
	public void AddSpawn(){
		++AmountOfSpawns;
		++AmountOfSpawnsLastMin;
	}

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
		SecondsAfterStart+=Time.deltaTime;
		LastMin+=Time.deltaTime;
		if (LastMin>60){
			LastMin=0;
			AmountOfDeathsLastMin=AmountOfSpawnsLastMin=0;
		}

		//input

		int mask=1<<LayerMask.NameToLayer("SpeechBubble");

		if (Input.GetMouseButtonDown(0)){
			var hit=Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,1,mask);
			
			if (hit.collider!=null){
				var bubble=hit.collider.gameObject.GetComponent<SpeechbubbleMain>();
				if (bubble.StatementPhase)
					bubble.BONUS();
			}
		}

		if (Input.GetMouseButtonDown(1)){
			var hit=Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,1,mask);
			
			if (hit.collider!=null){
				var bubble=hit.collider.gameObject.GetComponent<SpeechbubbleMain>();
				if (bubble.StatementPhase)
					bubble.ForceClose();
			}
		}
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
