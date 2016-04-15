using UnityEngine;
using System.Collections;

public class DecayOverTime : MonoBehaviour {

    public float totalLifeTime;
    private float remainingLifeTime;

    public SpriteRenderer mySprite;

	// Use this for initialization
	void Start () {
        remainingLifeTime = totalLifeTime;
	}
	
	// Update is called once per frame
	void Update () 
    {
        remainingLifeTime -= Time.deltaTime;

        float t = remainingLifeTime / totalLifeTime;

        if (t < 0.0f)
        {
            Destroy(gameObject);
        }

        else
        {
            Color curColor = mySprite.color;

            curColor.a = t;

            mySprite.color = curColor;
        }
	}
}
