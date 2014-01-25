using UnityEngine;
using System.Collections;

public class SpeechbubbleMain : MonoBehaviour {
	public Sprite IdeologyIconRED,IdeologyIconGREEN,IdeologyIconBLUE,IdeologyIconYELLOW,StatusApprove,StatusDisapprove;
	public SpriteRenderer IconRenderer,StatusRenderer,BonusRenderer;

	public bool StatementPhase=true;

	// Use this for initialization
	void Start () {
        BONUS_ON=false;
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
		BonusRenderer.gameObject.SetActive(false);
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
		Talker.ForceStopTalking();
	}

	public void PlayerApprove(){
		BONUS();
	}

	
	public void PlayerDissapprove(){
		ForceClose();
	}

    public bool BONUS_ON;

	public void BONUS ()
	{
        BONUS_ON=true;
		BonusRenderer.gameObject.SetActive(true);
	}
}
