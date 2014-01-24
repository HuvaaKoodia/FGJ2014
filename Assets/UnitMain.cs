using UnityEngine;
using System.Collections;

public class UnitMain: MonoBehaviour {

	public SpriteRenderer GraphicsSpriteRenderer;

	public enum Ideology {RED,GREEN,BLUE,YELLOW}
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

	Timer act_timer,speak_timer;
	public SpeechbubbleMain SpeechBubblePrefab;

	public GameObject Temp;

	public Sprite Talking,Listening;

	// Use this for initialization
	void Start () {
		act_timer=new Timer(Act);
		speak_timer=new Timer(5000,SpeakOver);
		speak_timer.Active=false;
		ResetActionTimer();

		MyIdeology=Subs.GetRandom(Subs.EnumValues<Ideology>());
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
				if (TalkingTo!=null){

				}
				else if (Talk_target!=null){
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
		TalkingTo.EndConversation();
		EndConversation();


		speak_timer.Active=false;

		//DEv.TODO change statistics


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
		act_timer.Active=false;
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

		if (SpeechBubble!=null)
			SpeechBubble.Close();

		ResetActionTimer();

		Temp.gameObject.SetActive(false);
	}

	void ResetActionTimer ()
	{
		act_timer.Delay=Subs.GetRandom(3000,5000);
		act_timer.Reset(true);
	}

	void OnTriggerStay2D(Collider2D col){
		if (moving||talking) return;
		if (col.gameObject.tag=="unit") {
			//walk away from here

			var dir=transform.position-col.transform.position;
			//if (dir.magnitude<0.05f){
			//	transform.Translate(new Vector3(Subs.GetRandom(-0.1f,0.1f),Subs.GetRandom(-0.1f,0.1f)));
			//}
			//else{
			if (dir.magnitude<CloseProximity)
				transform.Translate(dir.normalized*Time.deltaTime*MoveSpeed);
			//}
		}
	}
}
