using UnityEngine;
using System.Collections;

public class AutoFade : MonoBehaviour {

	public SpriteRenderer Target;
	public float speed=0.1f;

	// Use this for initialization
	void Start () {
		//transform.Rotate(Vector3.forward, Subs.GetRandom(0,360));
	}
	
	// Update is called once per frame
	void Update () {
		Target.color=new Color(Target.color.r,Target.color.g,Target.color.b,Target.color.a-Time.deltaTime*speed);

		if (Target.color.a<=0){
			Destroy(gameObject);
		}
	}
}
