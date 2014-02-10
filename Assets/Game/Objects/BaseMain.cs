using UnityEngine;
using System.Collections;

public class BaseMain : MonoBehaviour {

	public GameController GC;
    public ResourceStore ResStore;
	public Nationality MyNationality;
	Timer SpawnTimer;

	public float SpawnChance=25,ControllerValue=25;
    public int min_spawn_amount=5,max_spawn_amount=10;

	// Use this for initialization
	void Start () {
		SpawnTimer=new Timer(Spawn);
		ResetSpawnRate();

        ResStore=GameObject.FindGameObjectWithTag("GameOptions").GetComponent<ResourceStore>();

        int a = Subs.GetRandom (min_spawn_amount, max_spawn_amount);
        for (int i=0; i<a; i++) {
            AddUnit ();
        }


	}
	
	// Update is called once per frame
	void Update () {
		SpawnTimer.Update();

	}

	public UnitMain AddUnit(){
		var unit=Instantiate(ResStore.UnitPrefab,transform.position+new Vector3(Subs.GetRandom(-5f,5f),Subs.GetRandom(-5f,5f)),Quaternion.identity) as UnitMain;
		unit.ResStore=ResStore;

		unit.MyNationality=MyNationality;
		//unit.transform.parent=GC.ResStore.UnitsContainer;

        if (GC!=null){
            GC.Units.Add(unit);
            unit.OnDeath+=GC.AddDeath;
        }

		return unit;
	}

	void ResetSpawnRate ()
	{
		SpawnTimer.Delay=Subs.GetRandom(3000,5000);
		SpawnTimer.Reset(true);
	}

	void Spawn(){
		ResetSpawnRate();

        float control=20f;
        if (GC!=null) control=GC.GetPercentOfMaxPopulation(MyNationality)*ControllerValue;
            
		if ((Subs.GetRandom(100)-control)<SpawnChance){
			AddUnit();
			
            if (GC!=null) GC.AddSpawn();
		}

	}
}
