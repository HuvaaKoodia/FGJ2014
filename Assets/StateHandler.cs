using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateHandler : MonoBehaviour
{
		public enum stateOfMind
		{
				neural,
				sad,
				happy,
				mad
		}
		public Animator anime;
		public ResourceStore ResStore;
		public ForceRace force;
		int[] neutralFaces = new int[]{7,19,20,21,25,27,31,32,33};
		int[] sadFaces = new int[]{8,9,14,15,26};
		int[] happyFaces = new int[]{6,10,11,12,16,17,18,22,23,24,28,29,34,35};
		int[] madFaces = new int[]{0,1,2,3,4,5,30};

		public enum charStates
		{
				//grin_eyes
				angry,
				very_angry,
				slightly_angry,
				unpleased,
				pleasantly_angry,
				evil,
				agitated,
				//adore_eyes
				slightly_pleased,
				surprised_pleased,
				scared,
				sad,
				crazy_happy,
				excited,
				//derp_eyes
				pained,
				anguished,
				retarded,
				blind,
				overjoyed,
				laughing,
				//idle_eyes
				smiling,
				inAwe,
				cautious,
				neutral,
				chaotic_neutral,
				pumpedUp,
				//troll_eyes
				selfish_happy,
				oh_really,
				suspicious,
				disrespectful,
				trolling,
				puzzling,
				//egg_eyes
				extreme_happy,
				extreme_confused,
				shocked,
				serious,
				extreme_nuts,
				extreme_fanatic
		}
		;
		
		public int charaState = 0;
		public GameObject chara;
		// Use this for initialization
		void Start ()
		{
            GenerateFace(0);
		}
	
		// Update is called once per frame
		void Update ()
		{


//				if (Input.GetKeyDown (KeyCode.RightArrow)) {
//						charaState++;
//				} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
//						charaState--;
//				} else if (Input.GetKeyDown (KeyCode.Space)) {
//						charaState = Random.Range (0, 35);
//						UpdateState ();
//				}
		}

		public void ChangeViews (Nationality nationality, Ideology ideology)
		{

				int caseSwitcher = 0;

				switch (nationality) {
				case Nationality.Banana:
						caseSwitcher = 0;
						break;
				case Nationality.Lemon:
						caseSwitcher = 1;
						break;
				case Nationality.Grape:
			
						caseSwitcher = 2;
						break;
				case Nationality.Pear:
						caseSwitcher = 3;
						break;
				}
				switch (ideology) {
				case Ideology.BLUE:
						caseSwitcher += 0;
						break;
				case Ideology.GREEN:
						caseSwitcher += 10;
						break;
				case Ideology.RED:
			
						caseSwitcher += 20;
						break;
				case Ideology.YELLOW:
						caseSwitcher += 30;
						break;
				}
		Debug.Log (caseSwitcher);
				switch (caseSwitcher) {
				case 0:
						force.appearance = ResStore.BananaBlue;
						break;
				case 1:
						force.appearance = ResStore.LemonBlue;
						break;
				case 2:
						force.appearance = ResStore.GrapeBlue;
						break;
				case 3:
						force.appearance = ResStore.PearBlue;
						break;
				case 10:
						force.appearance = ResStore.BananaGreen;
						break;
				case 11:
						force.appearance = ResStore.LemonGreen;
						break;
				case 12:
						force.appearance = ResStore.GrapeGreen;
						break;
				case 13:
						force.appearance = ResStore.PearGreen;
						break;
				case 20:
						force.appearance = ResStore.BananaRed;
						break;
				case 21:
						force.appearance = ResStore.LemonRed;
						break;
				case 22:
						force.appearance = ResStore.GrapeRed;
						break;
				case 23:
						force.appearance = ResStore.PearRed;
						break;
				case 30:
						force.appearance = ResStore.BananaYellow;
						break;
				case 31:
						force.appearance = ResStore.LemonYellow;
						break;
				case 32:
						force.appearance = ResStore.GrapeYellow;
						break;
				case 33:
						force.appearance = ResStore.PearYellow;
						break;
				}
		
				force.SetSprites ();
		}

		void UpdateState ()
		{
				int mouth = charaState % 6;
				int eyes = charaState / 6;

				Debug.Log (charaState + " " + eyes + " " + mouth);
				switch (eyes) {
				case 0:
						force.eyeState.sprite = force.eyes_grin.sprite;
						break;
				case 1:
						force.eyeState.sprite = force.eyes_adore.sprite;
						break;
				case 2:
						force.eyeState.sprite = force.eyes_wtf.sprite;
						break;
				case 3:
						force.eyeState.sprite = force.eyes_idle.sprite;
						break;
				case 4:
						force.eyeState.sprite = force.eyes_troll.sprite;
						break;
				case 5:
						force.eyeState.sprite = force.eyes_fanatic.sprite;
						break;
				}
				switch (mouth) {
				case 0:
						force.mouthState.sprite = force.mouth_smile.sprite;
						break;
				case 1:
						force.mouthState.sprite = force.mouth_open.sprite;
						break;
				case 2:
						force.mouthState.sprite = force.mouth_derp.sprite;
						break;
				case 3:
						force.mouthState.sprite = force.mouth_idle.sprite;
						break;
				case 4:
						force.mouthState.sprite = force.mouth_troll.sprite;
						break;
				case 5:
						force.mouthState.sprite = force.mouth_howl.sprite;
						break;
			
						Debug.Log ("2:" + force.mouthState.sprite.name + " " + force.eyeState.sprite.name);
				}
				
	
		}
		
		
		/// <summary>
		/// 0 Neut 1 Sad 2 Happy 3 Mad
		/// </summary>
		/// <param name="i">The index.</param>
		public void GenerateFace (int i)
		{
				switch (i) {
				case 0:
						charaState = neutralFaces [Random.Range (0, neutralFaces.Length)];
						break;
				case 1:
						charaState = sadFaces [Random.Range (0, sadFaces.Length)];
						break;
				case 2:
						charaState = happyFaces [Random.Range (0, happyFaces.Length)];
						break;
				case 3:
			
						charaState = madFaces [Random.Range (0, madFaces.Length)];

						break;
				}
				charaState = Random.Range (0, 35);
				UpdateState ();
				UpdateState ();
		}
}
