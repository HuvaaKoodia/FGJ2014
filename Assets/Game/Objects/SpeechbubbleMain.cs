using UnityEngine;
using System.Collections;

public class SpeechbubbleMain : MonoBehaviour
{
    public GameObject IdeologyIconRED, IdeologyIconGREEN, IdeologyIconBLUE, IdeologyIconYELLOW;
    public Sprite StatusApprove, StatusDisapprove;
    public SpriteRenderer StatusRenderer, BonusRenderer;
    public AudioClip approveSound, rejectSound;
    public AudioSource audio_src;
    public bool StatementPhase = true, approved = false;
    public GameObject GraphicsParent;

    // Use this for initialization
    void Start ()
    {
        BONUS_ON = false;
    }
     
    // Update is called once per frame
    void Update ()
    {
        
    }

    UnitMain Talker;

    public void SetTalker (UnitMain unit)
    {
        Talker = unit;
        StatementPhase = true;

        if (unit.MyIdeology == Ideology.RED) {
            IdeologyIconRED.SetActive (true);
        }
        if (unit.MyIdeology == Ideology.GREEN) {
            IdeologyIconGREEN.SetActive (true);
        }
        if (unit.MyIdeology == Ideology.BLUE) {
            IdeologyIconBLUE.SetActive (true);
        }
        if (unit.MyIdeology == Ideology.YELLOW) {
            IdeologyIconYELLOW.SetActive (true);
        }

        StatusRenderer.gameObject.SetActive (false);
        BonusRenderer.gameObject.SetActive (false);
    }

    public void SetResult (bool approve)
    {
        StatementPhase = false;

        IdeologyIconRED.SetActive (false);
        IdeologyIconGREEN.SetActive (false);
        IdeologyIconBLUE.SetActive (false);
        IdeologyIconYELLOW.SetActive (false);

        StatusRenderer.gameObject.SetActive (true);
        if (approve)
            StatusRenderer.sprite = StatusApprove;
        else
            StatusRenderer.sprite = StatusDisapprove;
        approved = approve;
    }

    public void Close ()
    {
        if (approved) {
            audio_src.clip = approveSound;
            audio_src.loop = false;
            audio_src.Play();
        } else if (!approved) {
            audio_src.clip = rejectSound;
            audio_src.loop = false;
            audio_src.Play();

        }

        GraphicsParent.SetActive(false);
        //    Destroy(gameObject);
    }

    void ForceClose ()
    {
        Close ();
        Talker.ForceStopTalking ();
    }

    public void PlayerApprove ()
    {
        BONUS ();
    }
    
    public void PlayerDissapprove ()
    {
        ForceClose ();
    }

    public bool BONUS_ON;

    public void BONUS ()
    {
        BONUS_ON = true;
        BonusRenderer.gameObject.SetActive (true);
    }
}
