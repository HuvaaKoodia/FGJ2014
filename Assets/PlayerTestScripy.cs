using UnityEngine;
using System.Collections;

public class PlayerTestScripy : MonoBehaviour
{

		bool facingRight = true;
		Rigidbody2D rig;
		// Use this for initialization
		Animator anim;

		void Start ()
		{
				rig = GetComponent<Rigidbody2D> ();
				anim = GetComponent<Animator> ();
		}
	
		// Update is called once per frame
		/*void FixedUpdate ()
		{

				anim.SetBool ("Pressed A", false);
				if (Input.GetKey (KeyCode.Space)) {
						anim.SetBool ("Pressed A", true);
				}
				float move = Input.GetAxis ("Horizontal");
				anim.SetFloat ("Speed", Mathf.Abs (move));
			
		rig.velocity = new Vector2 (move * 7.5f, rig.velocity.y);		

				if (move > 0 && !facingRight) {
						Flip ();
				} else if (move < 0 && facingRight) {
						Flip ();
				}
		}
*/
		void Flip ()
		{
				facingRight = !facingRight;
				Vector3 theScale = transform.localScale;
				theScale.x *= -1;
				transform.localScale = theScale;
		}
}
