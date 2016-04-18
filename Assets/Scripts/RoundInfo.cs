using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class RoundInfo
{
    public float roundTime;
    public string text;

    public RoundInfo(float time, string text)
    {
        this.roundTime = time;
        this.text = text;
    }
	
}
