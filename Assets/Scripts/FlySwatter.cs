using UnityEngine;
using System.Collections;
using System;

public class FlySwatter : MonoBehaviour {

    public Animator myAnimator;
    public BoxCollider2D myCollider;
    private bool isAttacking;


	// Use this for initialization
	void Start () {
        WorldManager.Instance.TheFlySwatter = this;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            Attack();

        if(!isAttacking)
            UpdatePosition();

        else
        {
           if (WorldManager.Instance.TheFly)
                WorldManager.Instance.TheFly.OnSwatterAttackEnter();
        }
	}

    private void UpdatePosition()
    {
        Vector3 mousePosViewport = Input.mousePosition;
        mousePosViewport.z = -Camera.main.gameObject.transform.position.z;

      //  Vector3 direction = worldPos - gameObject.transform.position;
     //   direction.Normalize();

        gameObject.transform.position = Camera.main.ScreenToWorldPoint(mousePosViewport);
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            myAnimator.SetBool("IsAttacking", isAttacking);
        }

        else
        {
            if (WorldManager.Instance.TheFly)
                WorldManager.Instance.TheFly.OnSwatterAttackUpdate();
        }

    }

    public void AttackEnter()
    {
        myCollider.enabled = true;

    }

    public void AttackExit()
    {
     
        myCollider.enabled = false;

        if (WorldManager.Instance.TheFly)
        {
            Debug.Log("fly exists");
           WorldManager.Instance.TheFly.OnSwatterAttackExit();
        }
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
