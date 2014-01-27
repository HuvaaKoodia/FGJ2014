using UnityEngine;
using System.Collections;

public class BaseMain : MonoBehaviour {

	public GameController GC;
	public Nationality MyNationality;
	Timer SpawnTimer;

	public float SpawnChance=25,ControllerValue=25;


	// Use this for initialization
	void Start () {
		SpawnTimer=new Timer(Spawn);
		ResetSpawnRate();
	}
	
	// Update is called once per frame
	void Update () {
		SpawnTimer.Update();

	}

	public UnitMain AddUnit(){
		var unit=Instantiate(GC.ResStore.UnitPrefab,transform.position+new Vector3(Subs.GetRandom(-5f,5f),Subs.GetRandom(-5f,5f)),Quaternion.identity) as UnitMain;
		unit.GC=GC;
        GC.Units.Add(unit);
		unit.MyNationality=MyNationality;
		unit.transform.parent=GC.ResStore.UnitsContainer;
		unit.OnDeath+=GC.AddDeath;
		return unit;
	}

	void ResetSpawnRate ()
	{
		SpawnTimer.Delay=Subs.GetRandom(3000,5000);
		SpawnTimer.Reset(true);
	}

	void Spawn(){
		ResetSpawnRate();

        var control=GC.GetPercentOfMaxPopulation(MyNationality)*ControllerValue;
		if ((Subs.GetRandom(100)-control)<SpawnChance){
			AddUnit();
			
			GC.AddSpawn();
		}

	}
}
