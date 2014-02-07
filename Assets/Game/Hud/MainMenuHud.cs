using UnityEngine;
using System.Collections;

public class MainMenuHud : MonoBehaviour {

    public GameObject CreditsPanel,HelpPanel,PlayPanel;

    public UIButton GM1B,GM2B,GM3B,GM4B;

    public UILabel SecondsLabel;

    GameOptions GO;

    void Start(){
        GO=GameObject.FindGameObjectWithTag("GameOptions").GetComponent<GameOptions>();

        UpdateTimeLabel();

        DisableAll();

        GM1B.OnHoverColorTweenOn=false;
        GM2B.OnHoverColorTweenOn=false;
        GM3B.OnHoverColorTweenOn=false;
        GM4B.OnHoverColorTweenOn=false;


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

        SelectOneGM(GM1B);
    }
    void GM2(){
        GO.NationalityMode=GameMode.Similar;
        GO.IdeologyMode=GameMode.Diverse;

        SelectOneGM(GM2B);
    }
    void GM3(){
        GO.NationalityMode=GameMode.Diverse;
        GO.IdeologyMode=GameMode.Similar;

        SelectOneGM(GM3B);
    }
    void GM4(){
        GO.NationalityMode=GameMode.Similar;
        GO.IdeologyMode=GameMode.Similar;

        SelectOneGM(GM4B);
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
        GO.GameTime+=60;
        UpdateTimeLabel();
    }

    void TimeRemove(){
        if (GO.GameTime>60)
            GO.GameTime-=60;
        UpdateTimeLabel();
    }

    void UpdateTimeLabel ()
    {
        SecondsLabel.text=HudController.GetFormattedTime(GO.GameTime);
    }

    void SelectOneGM (UIButton button)
    {
        GM1B.defaultColor=Color.white;
        GM2B.defaultColor=Color.white;
        GM3B.defaultColor=Color.white;
        GM4B.defaultColor=Color.white;

        button.defaultColor=Color.red;
    }
}
