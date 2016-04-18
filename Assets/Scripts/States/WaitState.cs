using UnityEngine;
using System.Collections;

public class WaitState : AFlyState
{

    private float flySwatterAggroRadius;

    public WaitState(int difficulty)
        : base(difficulty)
    {
        flySwatterAggroRadius = 12.0f;
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

    public override void OnSwatterAttackEnter(FlyBehavior fly, FlySwatter swatter)
    {
        if (Vector3.Distance(swatter.gameObject.transform.position, fly.gameObject.transform.position) < flySwatterAggroRadius)
        {
            fly.SetState(FlyBehavior.EFlyState.Move);
        }
    }

    public override void OnSwatterAttackExit(FlyBehavior fly, FlySwatter swatter)
    {

    }

    public override void OnSwatterAttackUpdate(FlyBehavior fly, FlySwatter swatter)
    {

    }
}
