using UnityEngine;
using System.Collections;

public class SpeechbubbleMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	UnitMain Talker;

	public void SetTalker(UnitMain unit){
		Talker=unit;
	}

	public void Close ()
	{
		Destroy(gameObject);
	}
}
