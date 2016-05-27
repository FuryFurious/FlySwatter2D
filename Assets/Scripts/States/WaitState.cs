using UnityEngine;
using System.Collections;

public class WaitState : AFlyState
{

    private float flySwatterAggroRadius;

    private float timeToNewAction;

    private float minTimeToNewAction = 0.5f;
    private float maxTimeToNewAction = 1.0f;

    private float chanceToFleeOnAttack = 0.75f;

    public WaitState(int difficulty, FlySwatter swatter, FlyBehavior fly)
        : base(difficulty, swatter, fly)
    {
        flySwatterAggroRadius = 12.0f;

        switch (difficulty)
        {
            case 0:
                chanceToFleeOnAttack = 0.25f;
                break;

            case 1:
                chanceToFleeOnAttack = 0.6f;
                break;

            case 2:
                chanceToFleeOnAttack = 0.25f;
                break;

            case 3:
                chanceToFleeOnAttack = 1.0f;
                break;

            default:
                break;
        }
    }

    public override void OnStateEnter(AFlyState oldState)
    {
        fly.IsMoving = false;
    }

    public override void OnStateExit(AFlyState newState)
    {

    }

    public override void OnStateUpdate()
    {

        timeToNewAction -= Time.deltaTime;

        if (timeToNewAction <= 0.0f)
        {
            timeToNewAction = Random.Range(minTimeToNewAction, maxTimeToNewAction);

            float rand = Random.value;

            if (rand < 0.75f)
            {
                fly.TriggerIdle();
            }

            else
            {
                fly.SetState(FlyBehavior.EFlyState.Move);
            }
        }
    }

    public override void OnSwatterAttackStarted(bool isActiveState)
    {
        if (isActiveState)
        {
            if ((swatter.gameObject.transform.position - fly.gameObject.transform.position).sqrMagnitude < flySwatterAggroRadius * flySwatterAggroRadius)
            {

                if (Random.value <= chanceToFleeOnAttack)
                    fly.SetState(FlyBehavior.EFlyState.Move);
            }
        }
    }

    public override void OnSwatterAttackEnded(bool isActiveState)
    {

    }

    public override void OnStateFixedUpdate()
    {
      
    }
}
