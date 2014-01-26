using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Ideology
{
    RED,
    GREEN,
    BLUE,
    YELLOW
}

public enum Nationality
{
    Banana,
    Pear,
    Lemon,
    Grape
}

public class IdeologyData
{
    Ideology MyIdeology;
    float convert_chance = 25;
    float aggression;

    public float ConvertChance {
        get { return convert_chance;}
        set {
            convert_chance = Mathf.Clamp (value, 1, 100);
        }
    }

    public float Aggression {
        get { return aggression;}
        set {
            aggression = Mathf.Clamp (value, 1, 100);
        }
    }
}

public delegate void DeathAction (UnitMain d);

public class UnitMain: MonoBehaviour
{
    Color ideologyColor;
    public DeathAction OnDeath;
    public bool DebugGUIOn = false;
    public GameController GC;
    bool facingRight = true;
    public StateHandler handler;
    Nationality _nationality;

    public Nationality MyNationality {
        get {
            return _nationality;
        }
        set {
            _nationality = value;

            //DEV potatostuff
            /*if (value==Nationality.A) GraphicsSpriteRenderer.sprite=GC.ResStore.Unit_A;
        if (value==Nationality.B) GraphicsSpriteRenderer.sprite=GC.ResStore.Unit_B;
        if (value==Nationality.C) GraphicsSpriteRenderer.sprite=GC.ResStore.Unit_C;
        if (value==Nationality.D) GraphicsSpriteRenderer.sprite=GC.ResStore.Unit_D;
    */
        }

    }

    Ideology _ideology;

    public Ideology MyIdeology {
        get {
            return _ideology;
        }
        set {
            _ideology = value;

            if (value == Ideology.BLUE) {
                ideologyColor = Color.blue;
            }
        
            if (value == Ideology.GREEN) {
                ideologyColor = Color.green;
            }
        
            if (value == Ideology.YELLOW) {
                ideologyColor = Color.yellow;
            }
        
            if (value == Ideology.RED) {
                ideologyColor = Color.red;
            }
            handler.ChangeViews (MyNationality, MyIdeology);
        }
    }

    public void stopFighting ()
    {
        handler.anime.SetBool ("fighting", false);
        fighting = false;
        fighting_timer.Active = false;
    }

    public void splatterStorm ()
    {
        var obj = Instantiate (GC.ResStore.SplatPrefab, transform.position, Quaternion.identity) as GameObject;
        obj.transform.parent = GC.ResStore.MiscContainer;
        obj.GetComponent<SpriteRenderer> ().color = ideologyColor;
        splat_timer.Active = false;
    }

    public float convert_change_increase_multiplier = 1.1f,
        convert_change_decline_multiplier = 0.75f,
        depression_increase_multiplier = 1.1f,
        depression_decline_multiplier = 0.8f,
        KillAggressionDeductionMultiplier = 0.75f,
        VendettaConstant = 20,
        VendettaBaseMultiplier = 1.5f,
        VendettaRadius = 5,
        AggressionIncreaseIfDismissedMultiplier=1.25f
    ;
    public int InfluenceIncreasePerConversion = 5,
        ConversationStatementDelay = 5000,
        ConversationStatusDelay = 3000
    ;
    public Dictionary<Ideology,IdeologyData> IdeologyStats = new Dictionary<Ideology, IdeologyData> ();
    Timer act_timer, speak_timer, fighting_timer, splat_timer;
    public SpeechbubbleMain SpeechBubblePrefab;
    public GameObject Temp;
    public Sprite Talking, Listening;
    float depression = 10;

    public float Depression {
        get { return depression;}
        set {
            depression = Mathf.Clamp (value, 5, 100);
        }
    }

    int influence = 0;

    public int Influence {
        get { return influence;}
        set {
            influence = Mathf.Clamp (value, 0, 100);
        }
    }

    // Use this for initialization
    void Start ()
    {
        fighting_timer = new Timer (2000, stopFighting);
        fighting_timer.Active = false;
        splat_timer = new Timer (2000, splatterStorm);
        splat_timer.Active = false;
        handler.ResStore = GC.ResStore;
        unit_mask = 1 << LayerMask.NameToLayer ("Unit");

        act_timer = new Timer (Act);
        speak_timer = new Timer (5000, SpeakOver);
        speak_timer.Active = false;
        ResetActionTimer ();

        MyIdeology = Subs.GetRandom (Subs.EnumValues<Ideology> ());
                
        foreach (var emun in Subs.EnumValues<Ideology>()) {
            var idea = new IdeologyData ();
            IdeologyStats.Add (emun, idea);
            if (emun != MyIdeology) {
                idea.Aggression = Subs.GetRandom (0, 20);
                idea.ConvertChance = Subs.GetRandom (10, 30);
            }
        }
    
        UpdateGFXPos ();
    }

    void UpdateGFXPos ()
    {
        handler.transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.y / 1);
    }

        
    // Update is called once per frame
    void Update ()
    {
        if (!handler.anime.GetBool ("dead")) {
            act_timer.Update ();
            speak_timer.Update ();
            fighting_timer.Update ();
                        

            UpdateFacingTalking ();
            handler.anime.SetBool ("talking", TALKING);
            handler.anime.SetBool ("walking", moving);

            if (!TALKING && !fighting) {
                if (Target_Base != null) {
                    MoveTo (Target_Base.transform.position, TargetBaseRange);
                }

                if (Talk_target != null) {
                    MoveTo (Talk_target.transform.position, BasicMoveTargetRange);
                }
            }

            if (moving && !fighting) {
        
                UpdateGFXPos ();
                if (Vector3.Distance (transform.position, MoveTarget) < MoveTargetRange) {
                    ResetMovement ();
                    if (Talk_target != null) {
                        //talk target reached
                        StartConversation (Talk_target);
                    } else {
                        Target_Base = null;
                        ResetActionTimer ();
                    }
                } else {
                    var dir = MoveTarget - transform.position;
                    transform.Translate (dir.normalized * Time.deltaTime * MoveSpeed);
                }
            }
        } else {
            splat_timer.Update ();
        }
    }

    public float BasicMoveTargetRange = 0.5f, MoveSpeed = 3f, CloseProximity = 0.1f;
    bool moving = false, talking = false, fighting = false, dead = false;

    public bool TALKING{ get { return talking; } }

    Vector3 MoveTarget;
    UnitMain Talk_target, TalkingTo;
    SpeechbubbleMain SpeechBubble;
    public int unit_mask;

    void Act ()
    {
        if (fighting || dead) {
            return;
        }
        if (ForceWalkingToCivilization ())
            return;

        if (moving || TALKING || Talk_target != null)
            return;

        if (Subs.GetRandom (100) < 10) {

            //find target
            var units = Physics2D.OverlapCircleAll (transform.position, 2, unit_mask);
    
            if (units.Length > 1) {
                List<UnitMain> unitsShits = new List<UnitMain> ();
                UnitMain foo;
                foreach (var v in units) {
                    foo = v.GetComponent<UnitMain> ();
                    if (!foo.dead && foo != this) {
                        unitsShits.Add (foo);
                    }
                }
                while (true) {
                    UnitMain talk_to = Subs.GetRandom (unitsShits);
                    Talk_target = talk_to;
                    if (Talk_target != this) {
                        return;
                    }
                }
            }
        }
        if (Subs.GetRandom (100) < 50) {
            //move

            //find target
            MoveTo (transform.position + new Vector3 (Subs.GetRandom (-2f, 2f), Subs.GetRandom (-2f, 2f)), BasicMoveTargetRange);
            return;
        }

        //idle
        ResetActionTimer ();
    }

    void SpeakOver ()
    {
        if (SpeechBubble == null) {
            EndConversation ();
            speak_timer.Active = false;
            return;
        }

        if (SpeechBubble.StatementPhase) {
            //change statistics
            bool approve = true;
            bool bonus = SpeechBubble.BONUS_ON;
            if (TalkingTo.MyIdeology == MyIdeology) {
                TalkingToSameBelieverBonuses(bonus);
                TalkingToSameBelievers(TalkingTo,bonus);
               
            } else {
                approve = TryToConvertTarget (TalkingTo, bonus);
            }

            if (SpeechBubble != null) {
                UpdateConversationStatus (approve);
            }
        } else {
            TalkingTo.EndConversation ();
            EndConversation ();

            speak_timer.Active = false;
        }
    }

    void MoveTo (Vector3 target, float range)
    {
        MoveTargetRange = range;
        moving = true;
        MoveTarget = target;
        UpdateFacingMoving ();

    }

    void UpdateFacingMoving ()
    {
        if (MoveTarget.x - transform.position.x > 0 && !facingRight) {
            Flip ();
        } else if (MoveTarget.x - transform.position.x < 0 && facingRight) {
            Flip ();
        }
    }

    void UpdateFacingTalking ()
    {
        if (talking && !moving) {

            if (SpeechBubble == null && facingRight) {
                Flip ();
            } else if (SpeechBubble != null && !facingRight) {
                Flip ();
            }
        }
    }

    void Flip ()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void TalkTo (UnitMain target)
    {
        TalkingTo = target;
        Talk_target = null;
        talking = true;

        moving = false;
    }

    public void ListenTo (UnitMain target)
    {
        TalkTo (target);
        MoveTo (target.transform.position + Vector3.right * 2, 0.05f);
    }

    void StartConversation (UnitMain target)
    {
        if (target.TALKING || target.fighting)
            return;
        
        TalkTo (target);
        target.ListenTo (this);

        SpeechBubble = Instantiate (SpeechBubblePrefab, transform.position + new Vector3 (1, 2, handler.transform.position.z), Quaternion.identity) as SpeechbubbleMain;
        SpeechBubble.SetTalker (this);
        SpeechBubble.transform.parent = GC.ResStore.MiscContainer;
    
        speak_timer.Delay = ConversationStatementDelay;
        speak_timer.Reset (true);
    }

    void UpdateConversationStatus (bool approve)
    {
        SpeechBubble.SetResult (approve);
        
        speak_timer.Delay = ConversationStatusDelay;
        speak_timer.Reset (true);
    }

    public void EndConversation ()
    {
        ResetTalking ();

        if (SpeechBubble != null) {
            SpeechBubble.Close ();
            SpeechBubble = null;
        }

        ResetActionTimer ();
    }

    public void ForceStopTalking ()
    {
        TalkingTo.EndConversation ();
        EndConversation ();

        //start dissing everyone
        foreach(var i in IdeologyStats){
            i.Value.Aggression*=AggressionIncreaseIfDismissedMultiplier;
        }
    }

    void ResetTalking ()
    {
        talking = false;
        TalkingTo = null;
    }

    void ResetActionTimer ()
    {
        act_timer.Delay = Subs.GetRandom (1000, 3000);
        act_timer.Reset (true);
    }

    void ResetMovement ()
    {
        MoveTargetRange = BasicMoveTargetRange;
        moving = false;
    }

    List<int> SocialHistory = new List<int> ();

    public void AddSocialEvent (int e)
    {
        SocialHistory.Add (e);
        if (SocialHistory.Count > 20) {
            SocialHistory.RemoveAt (SocialHistory.Count - 1);
        }
    }

    bool CheckAggression (UnitMain target)
    {
        var c = Subs.GetRandom (100);
        var a = IdeologyStats [target.MyIdeology].Aggression;
        Debug.Log ("Aggression check: " + c + " < " + a);
        return c < a;
    }

    void Attack (UnitMain target)
    {
        handler.anime.SetBool ("fighting", true);
        fighting = true;
        target.fighting = true; 
                
        fighting_timer.Active = true;
        target.fighting_timer.Active = true;
        target.handler.anime.SetBool ("fighting", true);

        EndConversation ();
        target.EndConversation ();
        if (Subs.GetRandom (100) < 50) {
            target.Die (this);
            IdeologyStats [target.MyIdeology].Aggression *= KillAggressionDeductionMultiplier;
            Debug.LogWarning ("DEATH!");
        } else {
            Die (target);
            target.IdeologyStats [MyIdeology].Aggression *= KillAggressionDeductionMultiplier;
            Debug.LogWarning ("DEATH!");
        }

    }

    void Die (UnitMain killedBy)
    {
                
        dead = true;
        splat_timer.Active = true;
        handler.anime.SetBool ("dead", true);
                
        VendettaAOE (killedBy);
        if (OnDeath != null)
            OnDeath (this);
        
        EndConversation ();
        Destroy (gameObject, 4f);

        DebugGUIOn=false;
    }

    public void Die ()
    {
        if (OnDeath != null)
            OnDeath (this);
        splatterStorm ();
                
        EndConversation ();
        Destroy (gameObject);

        DebugGUIOn=false;
        
    }

    bool TryToConvertTarget (UnitMain target, bool bonus)
    {
        var bonus_influence = 0;
        if (bonus)
            bonus_influence = 100;

        var chance = Subs.GetRandom (100);

        Debug.Log ("Convert chance: " + (chance - Influence) + ", " + target.IdeologyStats [MyIdeology].ConvertChance);
        if (chance - Influence - bonus_influence < target.IdeologyStats [MyIdeology].ConvertChance) {
            //convert infidel
            AddSocialEvent (1);
            DecreaseOtherIdeologyChances (false);
            Influence += InfluenceIncreasePerConversion;

            target.AddSocialEvent (1);
            target.DecreaseOtherIdeologyChances (false);
            target.MyIdeology = MyIdeology;
            target.IdeologyStats [MyIdeology].Aggression *= 0.5f;
            return true;
        } else {
            //fail conversion
            AddSocialEvent (-1);
            aggroIncrease (target);

            target.AddSocialEvent (-1);
            target.aggroIncrease (this);

            //check attacks
            if (CheckAggression (target)) {
                Attack (target);
            } else if (target.CheckAggression (this)) {
                Attack (target);
            } else {
                target.IdeologyStats [MyIdeology].ConvertChance *= convert_change_increase_multiplier;
            
                //check exile
                if (MyNationality == target.MyNationality) {
                    Depression *= depression_increase_multiplier;
                    ExileCheck ();
                } else {
                    target.Depression *= depression_increase_multiplier;
                    target.ExileCheck ();
                }
            }
            return false;
        }   
    }

    void TalkingToSameBelievers(UnitMain target,bool boost){
        AddSocialEvent (1);
        target.AddSocialEvent (1);
        
        if (CheckAggression (target)) {
            Attack (target);
        } else if (target.CheckAggression (this)) {
            Attack (target);
        }
    }

    void TalkingToSameBelieverBonuses(bool boost){
        DecreaseOtherIdeologyChances (boost);
        IncreaseOtherIdeologyAggression (boost);
        Depression *= depression_decline_multiplier;
        IdeologyStats [MyIdeology].Aggression *= ConfirmationBiasOwnAggressionDecreaseMulti;
        //debug
    }

    public int ConfirmationBiasAggressionThreshold=35;
    public float ConfirmationBiasOwnAggressionDecreaseMulti=0.8f,ConfirmationBiasOtherAggressionIncrease=0.25f;

    public void IncreaseOtherIdeologyAggression (bool boost)
    {
        var multi = 1;
        if (boost) multi = 2;
        
        foreach (var idea in IdeologyStats) {
            if (idea.Key != MyIdeology) {
                if (idea.Value.Aggression>=ConfirmationBiasAggressionThreshold)
                    idea.Value.Aggression =idea.Value.Aggression* ConfirmationBiasOtherAggressionIncrease * multi;
            }
        }
    }

    public void DecreaseOtherIdeologyChances (bool boost)
    {
        var multi = 1;
        if (boost) multi = 2;

        foreach (var idea in IdeologyStats) {
            if (idea.Key != MyIdeology) {
                idea.Value.ConvertChance *= convert_change_decline_multiplier * multi;
            }
        }
    }

    void aggroIncrease (UnitMain target)
    {
        float multi = 2;
        if (MyNationality == target.MyNationality) {
            multi = 1;
        }
        int SocialMyA = GetSocialAmount (1);
        int SocialOtherA = GetSocialAmount (-1);
        int MaxSocialAmount = SocialMyA + SocialOtherA;

        var add = multi * ((1 - (Mathf.Abs (SocialMyA - SocialOtherA) / MaxSocialAmount)) * MaxSocialAmount * 0.5f);
        IdeologyStats [target.MyIdeology].Aggression += add;


        Debug.Log ("Aggro add:" + add);
    }

    void ExileCheck ()
    {
        if (Subs.GetRandom (100) < Depression) {
            GOTOExile ();
        }
    }

    void GOTOExile ()
    {
        handler.GenerateFace (1);
        var closest_base = FindClosesBase ();
        var bases = GameObject.FindGameObjectsWithTag ("base");
    
        List<BaseMain> other_bases = new List<BaseMain> ();

        foreach (var b in bases) {
            if (b == closest_base)
                continue;
            other_bases.Add (b.GetComponent<BaseMain> ());
        }

        WalkTowardsBase (Subs.GetRandom (other_bases));
    }

    int GetSocialAmount (int index)
    {
        var amount = 0;
        foreach (var i in SocialHistory) {
            if (i == index)
                amount++;
        }
        return amount;
    }

    BaseMain FindClosesBase ()
    {
        var bases = GameObject.FindGameObjectsWithTag ("Base");

        GameObject bas = null;
        float min = 1000000;

        foreach (var b in bases) {
            var dis = Vector3.Distance (transform.position, b.transform.position);
            if (dis < min) {
                bas = b;
                min = dis;
            }
        }
        return bas.GetComponent<BaseMain> ();
    }
    
    bool must_walk_to_civilization = false;
    BaseMain Target_Base;
    float MoveTargetRange, TargetBaseRange;

    void WalkTowardsBase (BaseMain Base)
    {
        Target_Base = Base;
        TargetBaseRange = Subs.GetRandom (2f, 5f);
    }

    void VendettaAOE (UnitMain killedBy)
    {
        //find target
        var units = Physics2D.OverlapCircleAll (transform.position, VendettaRadius, unit_mask);
    
        if (units.Length > 1) {
            foreach (var u in units) {
                var unit = u.GetComponent<UnitMain> ();
                if (unit != null && unit != this && unit.MyIdeology == MyIdeology) {
                    unit.IdeologyStats [killedBy.MyIdeology].Aggression *= VendettaBaseMultiplier + VendettaConstant;
                }
            }
        }
    }


#region Misc

    void OnTriggerStay2D (Collider2D col)
    {
        if (moving || talking || Talk_target != null)
            return;
        if (col.gameObject.tag == "unit") {

            var other = col.gameObject.GetComponent<UnitMain> ();

            if (other.moving)
                return;

            var dir = transform.position - col.transform.position;
        
            if (dir.magnitude < CloseProximity)
                transform.Translate (dir.normalized * Time.deltaTime * MoveSpeed);
        }
    }

    void OnTriggerExit2D (Collider2D col)
    {
        if (col.gameObject.tag == "worldlimits") {
            //walk towards closes base
            must_walk_to_civilization = true;
        }
    }

    bool ForceWalkingToCivilization ()
    {
        if (must_walk_to_civilization) {
            var bas = FindClosesBase ();
            WalkTowardsBase (bas);
            return true;
        }
        return false;
    }

    string guitext;

    void OnGUI ()
    {
        if (!DebugGUIOn)
            return;

        guitext = "Ideology stats:\n";
        foreach (var i in IdeologyStats) {
            guitext += i.Key + ": Agg=" + i.Value.Aggression + "% Convert: " + i.Value.ConvertChance + "%";
            guitext += "\n";
        }
        GUI.Box (new Rect (10, 10, 300, 400), guitext);
    }
    #endregion

}
