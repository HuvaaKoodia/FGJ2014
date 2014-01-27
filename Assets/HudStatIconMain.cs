using UnityEngine;
using System.Collections;

public class HudStatIconMain : MonoBehaviour {

    public UILabel StatLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetPercentage(int percentage){
        StatLabel.text=percentage+"%";
    }
}
