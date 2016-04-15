using UnityEngine;
using System.Collections;

public abstract class AFlyState
{
    protected int difficulty;

    public AFlyState(int difficulty)
    {
        this.difficulty = difficulty;
    }

    public abstract void OnStateEnter(FlyBehavior fly, AFlyState oldState);

    public abstract void OnStateExit(FlyBehavior fly, AFlyState newState);

    public abstract void OnStateUpdate(FlyBehavior fly);
}
