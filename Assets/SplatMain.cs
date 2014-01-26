using UnityEngine;
using System.Collections;

public class SplatMain : MonoBehaviour {

    public SpriteRenderer SR;
    public Sprite RED,GREEN,BLUE,YELLOW;

	// Use this for initialization
    public void SetIdeology(Ideology Ide){
        if (Ide==Ideology.BLUE)
            SR.sprite=BLUE;
        if (Ide==Ideology.GREEN)
            SR.sprite=GREEN;
        if (Ide==Ideology.RED)
            SR.sprite=RED;
        if (Ide==Ideology.YELLOW)
            SR.sprite=YELLOW;
    }
}
