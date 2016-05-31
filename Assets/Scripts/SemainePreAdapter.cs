using UnityEngine;
using System.Collections;

public static class SemainePreAdapter 
{
    public enum SemaineEvent { GameStarted, GameEnded, RoundStarted, RoundEnded, OnFlyKill, OnFlyMiss,          Count }

    private static SemaineKeyCombination[] combinations;

    private class SemaineKeyCombination
    {
        public SemaineEvent sEvent;
        public byte[] strokes;

        public SemaineKeyCombination(SemaineEvent sEvent, params byte[] strokes)
        {
            this.sEvent = sEvent;
            this.strokes = strokes;
        }
    }

    public static void Init()
    {
        combinations = new SemaineKeyCombination[(int)SemaineEvent.Count];

        CreateCombination(SemaineEvent.GameStarted,     NativeMethods.KEY_L_CONTROL, NativeMethods.KEY_F5);
        CreateCombination(SemaineEvent.GameEnded,       NativeMethods.KEY_L_CONTROL, NativeMethods.KEY_F6);
        CreateCombination(SemaineEvent.RoundStarted,    NativeMethods.KEY_L_CONTROL, NativeMethods.KEY_F1);
        CreateCombination(SemaineEvent.RoundEnded,      NativeMethods.KEY_L_CONTROL, NativeMethods.KEY_F2);
        CreateCombination(SemaineEvent.OnFlyKill,       NativeMethods.KEY_L_CONTROL, NativeMethods.KEY_F3);
        CreateCombination(SemaineEvent.OnFlyMiss,       NativeMethods.KEY_L_CONTROL, NativeMethods.KEY_F4);
    }

    private static void CreateCombination(SemaineEvent sEvent, params byte[] keystrokes)
    {
        combinations[(int)sEvent] = new SemaineKeyCombination(sEvent, keystrokes);
    }


    private static void SendKeyCombination(params byte[] strokes)
    {
        for (int i = 0; i < strokes.Length; i++)
        {
            NativeMethods.KeyPress(strokes[i]);
            NativeMethods.KeyRelease(strokes[i]);
        }
    }

    public static void SendSemaineEvent(SemaineEvent sEvent)
    {
#if !UNITY_EDITOR
        SendKeyCombination(combinations[(int)sEvent].strokes);
#endif
    }


}
