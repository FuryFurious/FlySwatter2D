using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class RoundInfo
{
    public float roundTime;
    public string text;
    public string continueText;

    public RoundInfo(float time, string text, string continueText)
    {
        this.roundTime = time;
        this.text = text;
        this.continueText = continueText;
    }
	
}
