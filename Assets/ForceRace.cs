using UnityEngine;
using System.Collections;

public class ForceRace : MonoBehaviour
{
	
		public CharacterAppearance appearance;
		public SpriteRenderer chest, right_arm, left_arm, eyes_grin, eyes_adore, eyes_idle, eyes_wtf, eyes_troll, eyes_fanatic, mouth_idle, mouth_smile, mouth_open, mouth_derp, mouth_troll, mouth_howl, eyes_closed, left_leg, right_leg;
		public SpriteRenderer mouthState, eyeState;

		void Start ()
		{
				SetSprites ();
		}

		public void SetSprites ()
		{
				chest.sprite = appearance.chest;
				right_arm.sprite = appearance.right_arm;
				left_arm.sprite = appearance.left_arm;
				eyes_grin.sprite = appearance.eyes_grin;
				eyes_adore.sprite = appearance.eyes_adore;
				eyes_idle.sprite = appearance.eyes_idle;
				eyes_wtf.sprite = appearance.eyes_wtf;
				eyes_troll.sprite = appearance.eyes_troll;
				eyes_fanatic.sprite = appearance.eyes_fanatic;
				mouth_idle.sprite = appearance.mouth_idle;
				mouth_smile.sprite = appearance.mouth_smile;
				mouth_open.sprite = appearance.mouth_open;
				mouth_derp.sprite = appearance.mouth_derp;
				mouth_troll.sprite = appearance.mouth_troll;
				mouth_howl.sprite = appearance.mouth_howl;
				eyes_closed.sprite = appearance.eyes_closed;
				left_leg.sprite = appearance.leg;
				right_leg.sprite = appearance.leg;
		
				eyeState.sprite = appearance.eyes_idle;
				mouthState.sprite = appearance.mouth_idle;
		}

		
}
