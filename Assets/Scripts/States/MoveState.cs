using UnityEngine;
using System.Collections;

public class MoveState : AFlyState
{
    /// <summary>The degree the fly can change its direction.</summary>
    private float directionChangeAngle = 45.0f;

    private float minSpeed = 30.0f;
    private float maxSpeed = 45.0f;

    private float timeToNewActionMin = 0.25f;
    private float timeToNewActionMax = 0.5f;
    private float timeToNewAction;

    /// <summary>The distance in which the fly flees from the swatter when it attacks.</summary>
    private float runAwayRadius = 15.0f;
    /// <summary>The speed the fly is using when the swatter is attacking and its in the radius.</summary>
    private float runAwaySpeed = 55.0f;
    /// <summary>The chance in percent that the fly will wait when its making its next decision.</summary>
    private float chanceToWait = 0.33f;
    private float runAwayChance = 1.0f;

    /// <summary>True if the swatter ist attacking, until it hit the surface.</summary>
    private bool swatterIsAttacking = false;

    /// <summary>The current speed the fly is flying around.</summary>
    private float speed;

    private float maxDistanceToNormalize = 48.0f;

  //  Vector3 toSwatterDirection;
   // float distanceToSwatter;

    public MoveState(int difficulty, FlySwatter swatter, FlyBehavior fly) 
        : base(difficulty, swatter, fly)
    {
        switch (difficulty)
        {
            case 0:
                minSpeed = 10.0f;
                maxSpeed = 20.0f;
                runAwayChance = 0.33f;
                runAwaySpeed = 40.0f;              
                break;

            case 1:
                minSpeed = 15.0f;
                maxSpeed = 25.0f;
                runAwayChance = 0.75f;
                runAwaySpeed = 50.0f;
                break;

            case 2:
                minSpeed = 10.0f;
                maxSpeed = 20.0f;
                runAwayChance = 0.33f;
                runAwaySpeed = 40.0f;   
                break;

            case 3:
                minSpeed = 25.0f;
                maxSpeed = 30.0f;
                runAwayChance = 1.0f;
                runAwaySpeed = 60.0f;
                runAwayRadius = 20.0f;
                break;

            default:
                break;
        }
    }

    public override void OnStateEnter( AFlyState oldState)
    {
        fly.IsMoving = true;
        //ChangeNormalDirection();
    }

    public override void OnStateExit(AFlyState newState)
    {
       // this.speed = Random.Range(minSpeed, maxSpeed);
    }

    public override void OnStateUpdate()
    {
        Vector3 toSwatterDirection = fly.gameObject.transform.position - WorldManager.Instance.TheFlySwatter.gameObject.transform.position;
        toSwatterDirection.z = 0.0f;

        float distanceToSwatter = toSwatterDirection.magnitude;

        //update if the swatter is not attackig, or if the swatter is attacking but far away:
        if(!swatterIsAttacking ||  (swatterIsAttacking && distanceToSwatter >= runAwayRadius))
        {
            timeToNewAction -= Time.deltaTime;

            if (timeToNewAction <= 0.0f)
            {
                ChangeNormalDirection();

                if (Random.value < chanceToWait)
                {
                    fly.SetState(FlyBehavior.EFlyState.Wait);
                }
            }
        }



    }

    private void ChangeNormalDirection()
    {
        timeToNewAction = Random.Range(timeToNewActionMin, timeToNewActionMax);

        ApplyNormalMovementSpeed();

        fly.gameObject.transform.up = GetNewDirectionVector();
    }

    private void ApplyNormalMovementSpeed()
    {
        this.speed = Random.Range(minSpeed, maxSpeed);
    }

    private Vector3 GetNewDirectionVector()
    {
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, directionChangeAngle * (2.0f * UnityEngine.Random.value - 1.0f));
        Vector3 randomDirection = rotation * fly.transform.up;

        Vector3 toOrigin = fly.gameObject.transform.position;
        toOrigin.z = 0.0f;
        float distToOrigin = toOrigin.magnitude;

        float t = distToOrigin / maxDistanceToNormalize;

        return Vector3.Lerp(randomDirection, -toOrigin, t);
    }

    public override void OnSwatterAttackStarted(bool isActiveState)
    {

        Vector3 toSwatterDirection = fly.gameObject.transform.position - WorldManager.Instance.TheFlySwatter.gameObject.transform.position;
        toSwatterDirection.z = 0.0f;

        float distanceToSwatter = toSwatterDirection.magnitude;
        swatterIsAttacking = true;

        if (distanceToSwatter < runAwayRadius && Random.value <= runAwayChance)
        {
            fly.gameObject.transform.up = toSwatterDirection;
            speed = runAwaySpeed;
        }

    }

    public override void OnSwatterAttackEnded(bool isActiveState)
    {
        swatterIsAttacking = false;

        ChangeNormalDirection();
    }

    public override void OnStateFixedUpdate()
    {
        fly.gameObject.transform.position += fly.gameObject.transform.up * speed * Time.deltaTime;
    }
}
