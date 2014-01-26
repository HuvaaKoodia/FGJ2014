using UnityEngine;
using System.Collections;

public class MainMenuHud : MonoBehaviour {

    public GameObject CreditsPanel,HelpPanel,PlayPanel;

    void Start(){
        DisableAll();
    }

    void PlayClick(){
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

    void DisableAll(){
        CreditsPanel.gameObject.SetActive(false);
        HelpPanel.gameObject.SetActive(false);
        //PlayPanel.gameObject.SetActive(false);
    }

    void ToggleOne(GameObject target){
        bool active=target.activeSelf;
        DisableAll();
        target.SetActive(!active);
    }
}
