using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public ResourceStore ResStore;
	public List<UnitMain> Units=new List<UnitMain>();
    public HudController Hud;

	public BaseMain ABase,BBase,CBase,DBase;   

	public int AmountOfDeaths=0,AmountOfSpawns=0,AmountOfDeathsLastMin=0,AmountOfSpawnsLastMin=0;
    public float SecondsAfterStart=0,FingerOfGodRadius=2.5f;
	float LastMin=0;

	public void AddDeath(UnitMain unit){
		++AmountOfDeaths;
		++AmountOfDeathsLastMin;

        Units.Remove(unit);
	}
	public void AddSpawn(){
		++AmountOfSpawns;
		++AmountOfSpawnsLastMin;
	}

    Timer ScoreTimer;

    int score=0,score_last=0,multi=2;

    void AddScore(int amount){
        score+=amount*multi;
        score_last+=amount;

    }

    void HudScoresUpdate(){
        //calculate scores
        AddScore(1000);

        Hud.SetScore(score);
        Hud.SetMulti(multi);
        Hud.SetScoreAdd(score_last,multi);

        score_last=0;
    }

	// Use this for initialization
	void Start () {
        ScoreTimer=new Timer(5000,HudScoresUpdate);
        Hud.SetScore(score);
        Hud.SetMulti(multi);


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
        ScoreTimer.Update();

		SecondsAfterStart+=Time.deltaTime;
		LastMin+=Time.deltaTime;
		if (LastMin>60){
			LastMin=0;
			AmountOfDeathsLastMin=AmountOfSpawnsLastMin=0;
		}

		//input

		int mask=1<<LayerMask.NameToLayer("SpeechBubble");
        int unit_mask=1<<LayerMask.NameToLayer("Unit");

		if (Input.GetMouseButtonDown(0)){
			var hit=Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,1,mask);
			
			if (hit.collider!=null){
				var bubble=hit.collider.gameObject.GetComponent<SpeechbubbleMain>();
				if (bubble.StatementPhase)
					bubble.PlayerApprove();
			}
		}

		if (Input.GetMouseButtonDown(1)){
			var hit=Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,1,mask);
			
			if (hit.collider!=null){
				var bubble=hit.collider.gameObject.GetComponent<SpeechbubbleMain>();
				if (bubble.StatementPhase)
					bubble.PlayerDissapprove();
			}
		}

        if (Input.GetKey(KeyCode.Space)){
            var units = Physics2D.OverlapCircleAll (Camera.main.ScreenToWorldPoint(Input.mousePosition), FingerOfGodRadius, unit_mask);

            foreach (var u in units) {
                var unit = u.GetComponent<UnitMain> ();
                if (unit != null) {
                    unit.Die();
                }
            }

        }

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Q)){
            var hit=Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,1,unit_mask);

            UnitMain unit=null;
            if (hit.collider!=null){

                unit = hit.collider.GetComponent<UnitMain> ();
                unit.DebugGUIOn=!unit.DebugGUIOn;

            }
            foreach(var u in Units){
                if (u!=unit) u.DebugGUIOn=false;
            }
        }
#endif

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.E)){
            DebugGUIOn=!DebugGUIOn;
        }
#endif
	}

    public bool DebugGUIOn=false;

    public float GetAmountOfMaxPopulation(Nationality Nat){
        //DEV. Opti. save all results in a batch every 5 seconds of so.
        int amount=0;
        foreach(var u in Units){
            if (u.MyNationality==Nat){
                ++amount;
            }
        }
        
        return amount;
    }

	public float GetPercentOfMaxPopulation(Nationality Nat){
		return GetAmountOfMaxPopulation(Nat)/Units.Count;
	}

    string guitext="";

    void OnGUI ()
    {
        if (!DebugGUIOn)
            return;
        
        guitext = "Game stats:\n";
        guitext+="Amount of Fruit= "+Units.Count;
        guitext+="\n";
        foreach (var i in Subs.EnumValues<Nationality>()) {
            guitext += i + ": Amount=" + GetAmountOfMaxPopulation(i) + ", Percent= " + GetPercentOfMaxPopulation(i) + "%";
            guitext += "\n";
        }
        GUI.Box (new Rect (300, 10, 300, 400), guitext);
    }
}
