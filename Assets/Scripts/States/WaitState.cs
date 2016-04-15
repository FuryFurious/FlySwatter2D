using UnityEngine;
using System.Collections;

public class WaitState : AFlyState
{

    public WaitState(int difficulty)
        : base(difficulty)
    {

    }

    public override void OnStateEnter(FlyBehavior fly, AFlyState oldState)
    {
        fly.IsMoving = false;
    }

    public override void OnStateExit(FlyBehavior fly, AFlyState newState)
    {

    }

    public override void OnStateUpdate(FlyBehavior fly)
    {
        float rand = Random.value;

        if (rand < 0.01f)
        {
            fly.SetState(FlyBehavior.EFlyState.Move);
        }

        else if (rand < 0.025f)
        {
            fly.TriggerIdle();
        }
    }
}
