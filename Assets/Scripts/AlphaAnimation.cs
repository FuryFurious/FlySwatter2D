using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AlphaAnimation : MonoBehaviour {

    public Text myText;

    public float speedFactor = 1.0f;

    private float time = 0.0f;
    private float lastTime;

	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {
        float deltaTime = Time.realtimeSinceStartup - lastTime;
        time += deltaTime;

        Color curColor = myText.color;
        curColor.a = Mathf.Sin(time * speedFactor) * 0.5f + 0.5f;

        myText.color = curColor;
           

        lastTime = Time.realtimeSinceStartup;
	}
}
