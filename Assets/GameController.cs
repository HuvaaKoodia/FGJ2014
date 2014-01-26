﻿using UnityEngine;
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
    public SpriteRenderer fingerOfGodus;
    public bool fingering = false, saucetime = false;
    public Vector3 target_finger_pos, offset = new Vector3 (0, 10.0f);
    public float fingering_speed = 2000.0f;

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

    int diversity_basescore=10, population_balancer=10;

    int GetDiverseNationalityScore(){

        int amount=0;
        foreach (var n in Subs.EnumValues<Nationality>()){
            amount+=diversity_basescore-((int)GetAmountOfMaxPopulation(n));
        }  
        return amount;
    }

    int GetDiverseIdeologyScore(){
        int amount=0;
        foreach (var n in Subs.EnumValues<Ideology>()){
            amount+=diversity_basescore-((int)GetAmountOfMaxPopulation(n));
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

    // Use this for initialization
    void Start ()
    {
        GO=GameObject.FindGameObjectWithTag("GameOptions").GetComponent<GameOptions>();
        fingerOfGodus.enabled = false;

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
    
    // Update is called once per frame
    void Update ()
    {
        if (fingering) {
            if (fingerOfGodus.transform.localPosition.y > target_finger_pos.y + 1.0f) {
                fingerOfGodus.transform.localPosition = fingerOfGodus.transform.localPosition + new Vector3 (0.0f, -fingering_speed*Time.deltaTime, 0.0f);
            } else {
                saucetime = true;
            }
        }
        ScoreTimer.Update ();

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

            fingerOfGodus.enabled = true;
            fingering = true;
            target_finger_pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            fingerOfGodus.transform.localPosition = new Vector3 (0.0f, 0.0f, 5.0f) + Camera.main.ScreenToWorldPoint (Input.mousePosition) + offset;
            //Camera.main.ScreenToWorldPoint (Input.mousePosition)

        }
        if (Mathf.Abs (target_finger_pos.y - fingerOfGodus.transform.position.y) < 1.0f && fingering && saucetime) {
            fingering = false;
            saucetime = false;
            var units = Physics2D.OverlapCircleAll (target_finger_pos, FingerOfGodRadius, unit_mask);
            
            foreach (var u in units) {
                var unit = u.GetComponent<UnitMain> ();
                if (unit != null) {
                    unit.Die ();
                }
            }
            audio_src.PlayOneShot (audio_src.clip);
            fingerOfGodus.enabled=false;
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
