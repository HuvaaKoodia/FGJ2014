using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Ideology {RED,GREEN,BLUE,YELLOW}
public enum Nationality {A,B,C,D}

public class IdeologyData{
	Ideology MyIdeology;

	public float convert_chance=20;
	public float aggression=0;
}

public class UnitMain: MonoBehaviour {

	public SpriteRenderer GraphicsSpriteRenderer;


	Nationality MyNationality;
	Ideology _ideology;
	
	Ideology MyIdeology{
		get{
			return _ideology;
		}
		set{
			_ideology=value;

			if (value==Ideology.BLUE){
				GraphicsSpriteRenderer.color=Color.blue;
			}
			
			if (value==Ideology.GREEN){
				GraphicsSpriteRenderer.color=Color.green;
			}
			
			if (value==Ideology.YELLOW){
				GraphicsSpriteRenderer.color=Color.yellow;
			}
			
			if (value==Ideology.RED){
				GraphicsSpriteRenderer.color=Color.red;
			}
		}
	}

	public float convert_change_increase_multiplier=1.1f,
	convert_change_decline_multiplier=0.75f,
	depression_increase_multiplier=1.1f,
	depression_decline_multiplier=0.8f
	;
	public int InfluenceIncreasePerConversion=5;

	public Dictionary<Ideology,IdeologyData> IdeologyStats=new Dictionary<Ideology, IdeologyData>();

	Timer act_timer,speak_timer;
	public SpeechbubbleMain SpeechBubblePrefab;

	public GameObject Temp;

	public Sprite Talking,Listening;

	float depression=10;
	public float Depression
	{
		get {return depression;}
		set{
			depression=Mathf.Clamp(value,5,100);
		}
	}
	
	int influence=0;
	public int Influence
	{
		get {return influence;}
		set{
			influence=Mathf.Clamp(value,0,100);
		}
	}

	// Use this for initialization
	void Start () {
		act_timer=new Timer(Act);
		speak_timer=new Timer(5000,SpeakOver);
		speak_timer.Active=false;
		ResetActionTimer();

		MyIdeology=Subs.GetRandom(Subs.EnumValues<Ideology>());

		foreach (var emun in Subs.EnumValues<Ideology>()){
			IdeologyStats.Add(emun,new IdeologyData());
		}
	}
	
	// Update is called once per frame
	void Update (){
		act_timer.Update();
		speak_timer.Update();

		if (Talk_target!=null){
			MoveTo(Talk_target.transform.position);
		}

		if (moving){
			if (Vector3.Distance(transform.position,MoveTarget)<CloseToRange){
				moving=false;
				if (talking){

				}
				else
				if (Talk_target!=null){
					//talk target reached
					TalkTo(Talk_target);
				}
				else{
					//reset 
					ResetActionTimer();
				}
			}
			else{
				var dir=MoveTarget-transform.position;
				transform.Translate(dir.normalized*Time.deltaTime*MoveSpeed);
			}
		}
	}

	public float CloseToRange=0.5f,MoveSpeed=3f,CloseProximity=0.1f;

	bool moving=false,talking=false;
	public bool TALKING{get{return talking;}}

	Vector3 MoveTarget;
	UnitMain Talk_target,TalkingTo;
	SpeechbubbleMain SpeechBubble;

	public static int  unit_mask=LayerMask.NameToLayer("Unit");
	
	void Act(){
		act_timer.Active=false;

		if (Subs.GetRandom(100)<10){

			//find target
			var units=Physics2D.OverlapCircleAll(transform.position,2,unit_mask);
			
			if (units.Length>0){
				var talk_to=Subs.GetRandom(units);
				Talk_target=talk_to.gameObject.GetComponent<UnitMain>();
				if (Talk_target==this){
					Talk_target=null;
				}
			}
		}
		else if (Subs.GetRandom(100)<50){
			//move

			//find target
			MoveTo(transform.position+ new Vector3(Subs.GetRandom(-2,2),Subs.GetRandom(-2,2)));
		}
		else{
			//idle
			ResetActionTimer();
		}
	}

	void SpeakOver(){

		//DEv.TODO change statistics
		
		if (TalkingTo.MyIdeology==MyIdeology){
			DecreaseOtherIdeologyChances();
			Depression*=depression_decline_multiplier;
		}
		else{
			TryToConvertTarget(TalkingTo);
		}
		
		TalkingTo.EndConversation();
		EndConversation();


		speak_timer.Active=false;
	}

	void MoveTo(Vector3 target){
		moving=true;
		MoveTarget=target;
	}

	public void TalkTo(UnitMain target)
	{
		if (target.TALKING) return;
		Talk_target=null;

		SetTalking(target);
		target.ListenTo(this);

		//Temp.gameObject.SetActive(true);
		//Temp.GetComponent<SpriteRenderer>().sprite=Talking;

		StartConversation();
	}

	private void SetTalking(UnitMain target){
		TalkingTo=target;
		talking=true;
		
		moving=false;
	}

	public void ListenTo (UnitMain target)
	{
		SetTalking(target);

		MoveTo(target.transform.position+Vector3.right*2);

		//Temp.gameObject.SetActive(true);
		//Temp.GetComponent<SpriteRenderer>().sprite=Listening;
	}

	void StartConversation()
	{
		SpeechBubble=Instantiate(SpeechBubblePrefab,transform.position+new Vector3(0.3f,-0.2f),Quaternion.identity) as SpeechbubbleMain;
		SpeechBubble.SetTalker(this);

		speak_timer.Reset(true);
	}

	void EndConversation(){
		talking=false;
		TalkingTo=null;

		if (SpeechBubble!=null){
			SpeechBubble.Close();
			SpeechBubble=null;
		}

		ResetActionTimer();

		Temp.gameObject.SetActive(false);
	}

	void ResetActionTimer ()
	{
		act_timer.Delay=Subs.GetRandom(1000,3000);
		act_timer.Reset(true);
	}

	void OnTriggerStay2D(Collider2D col){
		if (moving||talking) return;
		if (col.gameObject.tag=="unit") {
			//walk away from here

			var dir=transform.position-col.transform.position;

			if (dir.magnitude<CloseProximity)
				transform.Translate(dir.normalized*Time.deltaTime*MoveSpeed);
		}
	}

	List<int> SocialHistory=new List<int>();

	public void AddSocialEvent(int e){
		SocialHistory.Add(e);
		if (SocialHistory.Count>20){
			SocialHistory.RemoveAt(SocialHistory.Count-1);
		}
	}


	void TryToConvertTarget (UnitMain target)
	{
		var chance=Subs.GetRandom(100);
		Debug.Log("Convert chance: "+(chance-Influence)+", "+target.IdeologyStats[MyIdeology].convert_chance);
		if (chance-Influence<target.IdeologyStats[MyIdeology].convert_chance){
			//convert infidel
			AddSocialEvent(1);
			DecreaseOtherIdeologyChances();
			Influence+=InfluenceIncreasePerConversion;

			target.AddSocialEvent(1);
			target.DecreaseOtherIdeologyChances();
			target.MyIdeology=MyIdeology;
		}
		else{
			//fail conversion
			AddSocialEvent(-1);
			aggroIncrease(target);
		
			if (Subs.GetRandom(100)<IdeologyStats[target.MyIdeology].aggression){
				//tappo
				return;
			}

			target.IdeologyStats[MyIdeology].convert_chance*=convert_change_increase_multiplier;

			if (MyNationality==target.MyNationality){
				Depression*=depression_increase_multiplier;
				exileCheck();
			}
			else{
				target.Depression*=depression_increase_multiplier;
				target.exileCheck();
			}
		}
	}

	public void DecreaseOtherIdeologyChances ()
	{
		foreach (var idea in IdeologyStats){
			if (idea.Key!=MyIdeology){
				idea.Value.convert_chance=(int)(idea.Value.convert_chance*convert_change_decline_multiplier);
			}
		}
	}

	void aggroIncrease (UnitMain target)
	{
		float multi=2;
		if (MyNationality==target.MyNationality){
			multi=1;
		}
		int SocialMyA=GetSocialAmount(1);
		int SocialOtherA=GetSocialAmount(-1);
		int MaxSocialAmount=SocialMyA+SocialOtherA;

		var add=multi*((1-(Mathf.Abs(SocialMyA-SocialOtherA)/MaxSocialAmount))*MaxSocialAmount*0.5f);
		IdeologyStats[target.MyIdeology].aggression+=add;
	

		Debug.Log("Aggro add:"+add);
	}

	void exileCheck ()
	{
		if (Subs.GetRandom(100)<Depression){
			Exile();
		}
	}

	void Exile(){

	}

	int GetSocialAmount(int index){
			var amount=0;
		foreach (var i in SocialHistory){
			if (i==index)
					amount++;
		}
			return amount;
	}
}
