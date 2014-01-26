using UnityEngine;
using System.Collections;

public class HudController : MonoBehaviour {

    public UILabel ScoreLabel,ScoreAddLabel,MultiLabel;
    Timer AddTimer;

    // Use this for initialization
	void Start () {
        AddTimer=new Timer(1000,HideAddLabel);
        HideAddLabel();
	}
	
	// Update is called once per frame
	void Update () {
        AddTimer.Update();
	}

    public void SetScore(int score){
        ScoreLabel.text="Score:\n"+score;
    }

    public void SetScoreAdd(int score,int multi){
        ScoreAddLabel.text="+"+score+"\nx"+multi;
        ScoreAddLabel.alpha=1;
        AddTimer.Reset(true);
    }
    
    public void SetMulti(int multi){
        MultiLabel.text="Multi:\n"+multi;
    }

    void HideAddLabel(){
        ScoreAddLabel.alpha=0;
        AddTimer.Active=false;
    }
}
