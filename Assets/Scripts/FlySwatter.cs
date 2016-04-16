using UnityEngine;
using System.Collections;
using System;

public class FlySwatter : MonoBehaviour {

    public float moveSpeed = 2.0f;
    public Animator myAnimator;
    public BoxCollider2D myCollider;
    private bool isAttacking;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            Attack();

        if(!isAttacking)
            UpdatePosition();
	}

    private void UpdatePosition()
    {
        Vector3 mousePosViewport = Input.mousePosition;
        mousePosViewport.z = -Camera.main.gameObject.transform.position.z;
        Vector3 worldPos =  Camera.main.ScreenToWorldPoint(mousePosViewport);

        Vector3 direction = worldPos - gameObject.transform.position;
        direction.Normalize();

        gameObject.transform.position = worldPos;// direction * Time.deltaTime * moveSpeed;
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            myAnimator.SetBool("IsAttacking", isAttacking);
        }

    }

    public void AttackEnter()
    {
        myCollider.enabled = true;
    }

    public void AttackExit()
    {
        myCollider.enabled = false;
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        myAnimator.SetBool("IsAttacking", isAttacking);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        FlyBehavior fly  = other.GetComponent<FlyBehavior>();

        if (fly)
        {
            fly.Die();
        }
    }
    
}
