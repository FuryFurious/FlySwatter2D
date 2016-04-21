using UnityEngine;
using System.Collections;

public abstract class AFlyState
{
    protected int difficulty;
    protected FlyBehavior fly;
    protected FlySwatter swatter;

    public AFlyState(int difficulty, FlySwatter swatter, FlyBehavior fly)
    {
        this.fly = fly;
        this.swatter = swatter;
        this.difficulty = difficulty;
    }

    public abstract void OnSwatterAttackStarted(bool isActiveState);
    public abstract void OnSwatterAttackEnded(bool isActiveState);

    public abstract void OnStateEnter(AFlyState oldState);
    public abstract void OnStateExit(AFlyState newState);
    public abstract void OnStateUpdate();

    public abstract void OnStateFixedUpdate();
}
