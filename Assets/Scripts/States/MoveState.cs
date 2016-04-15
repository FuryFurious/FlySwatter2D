using UnityEngine;
using System.Collections;


public class MoveState : AFlyState
{
    private float directionChangeAngle = 45.0f;

    private float minSpeed;
    private float maxSpeed;

    private float speed;

    private float timeToChangeDirection;

    public MoveState(int difficulty) 
        : base(difficulty)
    {

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
        timeToChangeDirection -= Time.deltaTime;

        fly.gameObject.transform.position += fly.gameObject.transform.up * speed * Time.deltaTime;

        if (timeToChangeDirection <= 0.0f)
        {
            ChangeDirection(fly);
        }

        if (Random.value < 0.01f)
        {
            fly.SetState(FlyBehavior.EFlyState.Wait);
        }

    }

    private void ChangeDirection(FlyBehavior fly)
    {
        timeToChangeDirection = Random.Range(0.25f, 0.5f);

        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, directionChangeAngle * (2.0f * Random.value - 1.0f));

        Vector3 toOrigin = fly.gameObject.transform.position;
        toOrigin.z = 0.0f;

        float distToOrigin = toOrigin.magnitude;
        toOrigin.Normalize();
        
        float maxDistance = 32.0f;

        float t = distToOrigin / maxDistance;


        fly.gameObject.transform.up = Vector3.Lerp(rotation * fly.gameObject.transform.up, -toOrigin, t);
    }
}
