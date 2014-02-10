using UnityEngine;
using System.Collections;

public class HudController : MonoBehaviour {

    public UILabel ScoreLabel,ScoreAddLabel,MultiLabel,TimeLabel,GO_scorelabel;
    Timer AddTimer;

    public GameObject GameOverPanel;

    public HudStatIconMain[] IdeologyIcons,NationalityIcons;

    // Use this for initialization
	void Start () {
        AddTimer=new Timer(3000,HideAddLabel);
        HideAddLabel();

        GameOverPanel.SetActive(false);
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

    public void SetIdeologyIconPercentage(int index,int per){
        IdeologyIcons[index].SetPercentage(per);
    }
    public void SetNationalityIconPercentage(int index,int per){
        NationalityIcons[index].SetPercentage(per);
    }

    public void SetTime(int seconds){
       TimeLabel.text=GetFormattedTime(seconds);
    }

    public static string GetFormattedTime(int seconds){
        int s=seconds%60;
        int m=seconds/60;
        return string.Format("{0}:{1:00}",m,s);
    }

    public void SetGameover (int score)
    {
        GameOverPanel.SetActive(true);
        GO_scorelabel.text="Score:\n"+score;
    }

    public void BackPressed(){
        Application.LoadLevel("MainMenuScene");
    }
}
