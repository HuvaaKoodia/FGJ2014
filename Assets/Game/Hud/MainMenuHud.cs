using UnityEngine;
using System.Collections;

public class MainMenuHud : MonoBehaviour {

    public GameObject CreditsPanel,HelpPanel,PlayPanel;

    public UILabel SecondsLabel;

    GameOptions GO;

    void Start(){
        GO=GameObject.FindGameObjectWithTag("GameOptions").GetComponent<GameOptions>();

        UpdateTimeLabel();

        DisableAll();
    }

    void PlayClick(){
        ToggleOne(PlayPanel);
    }

    void StartClick(){
        Application.LoadLevel("GameScene");
    }
    
    void HelpClick(){
        ToggleOne(HelpPanel);
    }

    void CreditsClick(){
        ToggleOne(CreditsPanel);
    }

    void QuitClick(){
        Application.Quit();
    }

    //gamemodes
    void GM1(){
        GO.NationalityMode=GameMode.Diverse;
        GO.IdeologyMode=GameMode.Diverse;
    }
    void GM2(){
        GO.NationalityMode=GameMode.Similar;
        GO.IdeologyMode=GameMode.Diverse;
    }
    void GM3(){
        GO.NationalityMode=GameMode.Diverse;
        GO.IdeologyMode=GameMode.Similar;
    }
    void GM4(){
        GO.NationalityMode=GameMode.Similar;
        GO.IdeologyMode=GameMode.Similar;
    }

    void DisableAll(){
        CreditsPanel.gameObject.SetActive(false);
        HelpPanel.gameObject.SetActive(false);
        PlayPanel.gameObject.SetActive(false);
    }

    void ToggleOne(GameObject target){
        bool active=target.activeSelf;
        DisableAll();
        target.SetActive(!active);
    }

    void TimeAdd(){
        GO.GameTime+=30;
        UpdateTimeLabel();
    }

    void TimeRemove(){
        if (GO.GameTime>30)
            GO.GameTime-=30;
        UpdateTimeLabel();
    }

    void UpdateTimeLabel ()
    {
        SecondsLabel.text=(GO.GameTime)+ "s";
    }
}
