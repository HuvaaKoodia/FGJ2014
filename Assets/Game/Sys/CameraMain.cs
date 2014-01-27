using UnityEngine;
using System.Collections;

public class CameraMain : MonoBehaviour {

    public float CameraMax=30,CameraMin=1,CameraSpeed=1,ZoomSpeed=10;
    public BoxCollider2D Limits;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        var multi=Camera.main.orthographicSize/CameraMax;
        var axis=Input.GetAxisRaw("Horizontal");

        if (axis!=0){
            transform.Translate(Vector2.right*axis*CameraSpeed*multi*Time.deltaTime);
            ClampPos();
        }
        axis=Input.GetAxisRaw("Vertical");
        if (axis!=0){
            transform.Translate(Vector2.up*axis*CameraSpeed*multi*Time.deltaTime);
            ClampPos();
        }

        axis=Input.GetAxisRaw("Mouse ScrollWheel");

        if (axis!=0){
            Camera.main.orthographicSize=Mathf.Clamp(Camera.main.orthographicSize-axis*ZoomSpeed,CameraMin,CameraMax);

        }
	}

    void ClampPos(){
        transform.position=new Vector3(
            Mathf.Clamp(transform.position.x,-Limits.transform.localScale.x*0.5f,Limits.transform.localScale.x*0.5f),
            Mathf.Clamp(transform.position.y,-Limits.transform.localScale.y*0.5f,Limits.transform.localScale.y*0.5f),
            transform.position.z
            );
    }
}
