using UnityEngine;
using System.Collections;

public class MoveState : AFlyState
{
    private float directionChangeAngle = 45.0f;

    private float minSpeed;
    private float maxSpeed;

    private float speed;

    private float timeToChangeDirection;

    private float runAwayRadius;
    private float runAwaySpeed;

    private bool swatterIsAttacking;

    public MoveState(int difficulty) 
        : base(difficulty)
    {
        runAwayRadius = 10.0f;
        runAwaySpeed = 50.0f;
    }

    public override void OnStateEnter(FlyBehavior fly, AFlyState oldState)
    {
        fly.IsMoving = true;
        this.minSpeed = 10.0f;
        this.maxSpeed = 15.0f;

        this.speed = Random.Range(minSpeed, maxSpeed);

        ChangeDirection(fly);
    }

    public override void OnStateExit(FlyBehavior fly, AFlyState newState)
    {

    }

    public override void OnStateUpdate(FlyBehavior fly)
    {

        if(!swatterIsAttacking)
        {
            timeToChangeDirection -= Time.deltaTime;

            if (timeToChangeDirection <= 0.0f)
            {
                ChangeDirection(fly);
            }

            if (Random.value < 0.01f)
            {
                fly.SetState(FlyBehavior.EFlyState.Wait);
            }
        }

        else
        {
            Debug.Log("run awway");
            Vector3 runAwayDirection = fly.gameObject.transform.position - WorldManager.Instance.TheFlySwatter.gameObject.transform.position;
            runAwayDirection.z = 0.0f;

            float distance = runAwayDirection.magnitude;

            runAwayDirection.Normalize();

            if (distance < runAwayRadius)
            {
                fly.gameObject.transform.up = runAwayDirection;
                speed = runAwaySpeed;
            }

            else
            {
                fly.SetState(FlyBehavior.EFlyState.Wait);
            }

        
        }


        fly.gameObject.transform.position += fly.gameObject.transform.up * speed * Time.deltaTime;
    }

    private void ChangeDirection(FlyBehavior fly)
    {
        timeToChangeDirection = Random.Range(0.25f, 0.5f);

        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, directionChangeAngle * (2.0f * UnityEngine.Random.value - 1.0f));

        Vector3 toOrigin = fly.gameObject.transform.position;
        toOrigin.z = 0.0f;

        float distToOrigin = toOrigin.magnitude;
        toOrigin.Normalize();
        
        float maxDistance = 32.0f;

        float t = distToOrigin / maxDistance;

      //  this.speed = Random.RandomRange(minSpeed, maxSpeed);


        fly.gameObject.transform.up = Vector3.Lerp(rotation * fly.gameObject.transform.up, -toOrigin, t);
    }

    public override void OnSwatterAttackEnter(FlyBehavior fly, FlySwatter swatter)
    {
        swatterIsAttacking = true;
        Debug.Log("start");
    }

    public override void OnSwatterAttackUpdate(FlyBehavior fly, FlySwatter swatter)
    {
        Debug.Log("update");
    }

    public override void OnSwatterAttackExit(FlyBehavior fly, FlySwatter swatter)
    {
        swatterIsAttacking = false;

        Debug.Log("exit");
    }
}
