using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

    public ResourceStore ResStore;
    public List<UnitMain> Units = new List<UnitMain> ();
    public HudController Hud;
    public BaseMain ABase, BBase, CBase, DBase;
    public AudioSource audio_src;
    GameOptions GO;
    public int AmountOfDeaths = 0, AmountOfSpawns = 0, AmountOfDeathsLastMin = 0, AmountOfSpawnsLastMin = 0;
    public float SecondsAfterStart = 0, FingerOfGodRadius = 2.5f;
    float LastMin = 0;

    float GameTime;

    void Start ()
    {
        GO=GameObject.FindGameObjectWithTag("GameOptions").GetComponent<GameOptions>();

        GameTime=GO.GameTime;

        ScoreTimer = new Timer (5000, HudScoresUpdate);
        Hud.SetScore (score);
        Hud.SetMulti (multi);

        //generate units
        
        int a = Subs.GetRandom (5, 10);
        for (int i=0; i<a; i++) {
            ABase.AddUnit ();
        }
        
        a = Subs.GetRandom (5, 10);
        for (int i=0; i<a; i++) {
            BBase.AddUnit ();
        }
        
        a = Subs.GetRandom (5, 10);
        for (int i=0; i<a; i++) {
            CBase.AddUnit ();
        }
        
        a = Subs.GetRandom (5, 10);
        for (int i=0; i<a; i++) {
            DBase.AddUnit ();
        }
    }

    bool gameover=false;

    void Update ()
    {
        if(gameover) return;

        GameTime-=Time.deltaTime;

        if(GameTime<0){
            gameover=true;
            Hud.SetGameover();
            GameTime=0;
        }

        Hud.SetTime((int)GameTime);

        ScoreTimer.Update ();
        UpdateStatIcons();
        
        SecondsAfterStart += Time.deltaTime;
        LastMin += Time.deltaTime;
        if (LastMin > 60) {
            LastMin = 0;
            AmountOfDeathsLastMin = AmountOfSpawnsLastMin = 0;
        }
        
        //input
        
        int mask = 1 << LayerMask.NameToLayer ("SpeechBubble");
        int unit_mask = 1 << LayerMask.NameToLayer ("Unit");
        
        if (Input.GetMouseButtonDown (0)) {
            var hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, 1, mask);
            
            if (hit.collider != null) {
                var bubble = hit.collider.gameObject.GetComponent<SpeechbubbleMain> ();
                if (bubble.StatementPhase)
                    bubble.PlayerApprove ();
            }
        }
        
        if (Input.GetMouseButtonDown (1)) {
            var hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, 1, mask);
            
            if (hit.collider != null) {
                var bubble = hit.collider.gameObject.GetComponent<SpeechbubbleMain> ();
                if (bubble.StatementPhase)
                    bubble.PlayerDissapprove ();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space)){
            
            var units = Physics2D.OverlapCircleAll (Camera.main.ScreenToWorldPoint (Input.mousePosition), FingerOfGodRadius, unit_mask);
            
            foreach (var u in units) {
                var unit = u.GetComponent<UnitMain> ();
                if (unit != null) {
                    unit.Die ();
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

    public void AddDeath (UnitMain unit)
    {
        ++AmountOfDeaths;
        ++AmountOfDeathsLastMin;

        Units.Remove (unit);
    }

    public void AddSpawn ()
    {
        ++AmountOfSpawns;
        ++AmountOfSpawnsLastMin;
    }

    Timer ScoreTimer;
    int score = 0, score_last = 0, multi = 2;

    void AddScore (int amount)
    {
        score += amount * multi;
        score_last += amount;
    }

    //scores
    int diversity_basescore=10, population_balancer=10;

    int GetDiverseNationalityScore(){

        int amount=0;
        foreach (var n in Subs.EnumValues<Nationality>()){
            amount+=diversity_basescore-Mathf.Abs((int)GetAmountOfMaxPopulation(n)-(Units.Count/4));
        }  
        return amount;
    }

    int GetDiverseIdeologyScore(){
        int amount=0;
        foreach (var n in Subs.EnumValues<Ideology>()){
            amount+=diversity_basescore-Mathf.Abs((int)GetAmountOfMaxPopulation(n)-(Units.Count/4));
        }  
        return amount;
    }

    int GetMulti(){
        return Units.Count/population_balancer;
    }

    void HudScoresUpdate(){
        //calculate scores

        multi=GetMulti();

        int temp=0;

        if (GO.NationalityMode==GameMode.Diverse&&GO.IdeologyMode==GameMode.Diverse){
            temp+=GetDiverseIdeologyScore();
            temp+=GetDiverseNationalityScore();
            temp*=multi;
        }

        if (GO.NationalityMode==GameMode.Similar&&GO.IdeologyMode==GameMode.Diverse){
            temp+=GetDiverseIdeologyScore();

            int max=0;
            foreach(var n in Subs.EnumValues<Nationality>()){
                int amount=(int)GetAmountOfMaxPopulation(n);
                if (max<amount){
                    max=amount;
                }
            }
            temp+=max;
            
            temp*=multi;
        }

        if (GO.NationalityMode==GameMode.Diverse&&GO.IdeologyMode==GameMode.Diverse){
            temp+=GetDiverseIdeologyScore();
            
            int max=0;
            foreach(var n in Subs.EnumValues<Nationality>()){
                int amount=(int)GetAmountOfMaxPopulation(n);
                if (max<amount){
                    max=amount;
                }
            }
            temp+=max;
            
            temp*=multi;
        }
       
        AddScore (temp);

        Hud.SetScore (score);
        Hud.SetMulti (multi);
        Hud.SetScoreAdd (score_last, multi);

        score_last = 0;
    }

    void UpdateStatIcons(){
        foreach (var i in Subs.EnumValues<Ideology>()){
            Hud.SetIdeologyIconPercentage((int)i,(int)(GetPercentOfMaxPopulation(i)*100));
        }
        foreach (var i in Subs.EnumValues<Nationality>()){
            Hud.SetNationalityIconPercentage((int)i,(int)(GetPercentOfMaxPopulation(i)*100));
        }
    }



    public bool DebugGUIOn = false;

    public float GetAmountOfMaxPopulation (Nationality Nat)
    {
        //DEV. Opti. save all results in a batch every 5 seconds of so.
        int amount = 0;
        foreach (var u in Units) {
            if (u.MyNationality == Nat) {
                ++amount;
            }
        }
        
        return amount;
    }

    public float GetPercentOfMaxPopulation (Nationality Nat)
    {
        if (Units.Count==0) return 0;
        return GetAmountOfMaxPopulation (Nat) / Units.Count;
    }

    public float GetAmountOfMaxPopulation (Ideology ide)
    {
        //DEV. Opti. save all results in a batch every 5 seconds of so.
        int amount = 0;
        foreach (var u in Units) {
            if (u.MyIdeology == ide) {
                ++amount;
            }
        }
        
        return amount;
    }
    
    public float GetPercentOfMaxPopulation (Ideology ide)
    {
        if (Units.Count==0) return 0;
        return GetAmountOfMaxPopulation (ide) / Units.Count;
    }

    string guitext = "";

    void OnGUI ()
    {
        if (!DebugGUIOn)
            return;
        
        guitext = "Game stats:\n";
        guitext += "Amount of Fruit= " + Units.Count;
        guitext += "\nNationality:\n";
        foreach (var i in Subs.EnumValues<Nationality>()) {
            guitext += i + ": Amount=" + GetAmountOfMaxPopulation (i) + ", Percent= " + GetPercentOfMaxPopulation (i) * 100f + "%";
            guitext += "\n";
        }
        guitext += "\nIdealogy:\n";
        foreach (var i in Subs.EnumValues<Ideology>()) {
            guitext += i + ": Amount=" + GetAmountOfMaxPopulation (i) + ", Percent= " + GetPercentOfMaxPopulation (i) * 100f + "%";
            guitext += "\n";
        }
        GUI.Box (new Rect (Screen.width - 300, 10, 300, 400), guitext);
    }
}