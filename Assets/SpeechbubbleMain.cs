using UnityEngine;
using System.Collections;

public class SpeechbubbleMain : MonoBehaviour {


	public Sprite IdeologyIconRED,IdeologyIconGREEN,IdeologyIconBLUE,IdeologyIconYELLOW,StatusApprove,StatusDisapprove;
	public SpriteRenderer IconRenderer,StatusRenderer;

	public bool StatementPhase=true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	UnitMain Talker;

	public void SetTalker(UnitMain unit){
		Talker=unit;
		StatementPhase=true;

		if (unit.MyIdeology==Ideology.RED){IconRenderer.sprite=IdeologyIconRED;}
		if (unit.MyIdeology==Ideology.GREEN){IconRenderer.sprite=IdeologyIconGREEN;}
		if (unit.MyIdeology==Ideology.BLUE){IconRenderer.sprite=IdeologyIconBLUE;}
		if (unit.MyIdeology==Ideology.YELLOW){IconRenderer.sprite=IdeologyIconYELLOW;}

		IconRenderer.gameObject.SetActive(true);
		StatusRenderer.gameObject.SetActive(false);
	}

	public void SetResult(bool approve){
		StatementPhase=false;
		IconRenderer.gameObject.SetActive(false);
		StatusRenderer.gameObject.SetActive(true);
		if (approve)
			StatusRenderer.sprite=StatusApprove;
		else
			StatusRenderer.sprite=StatusDisapprove;
	}

	public void Close ()
	{
		Destroy(gameObject);
	}

	void ForceClose()
	{
		Destroy(gameObject);
		Talker.EndConversation();
	}

	public void PlayerApprove(){

	}

	
	public void PlayerDissapprove(){
		
	}
}
