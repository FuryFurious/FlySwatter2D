using UnityEngine;
using System.Collections;
using System;

public class FlySwatter : MonoBehaviour {

    public Animator myAnimator;
    public BoxCollider2D myCollider;
    public GameObject bottomObject;
    private bool isAttacking;

    private bool hitFly = false;
    private bool firstHit = false;

    public AudioSource whipSound;

	// Use this for initialization
	void Start () {
        //WorldManager.Instance.TheFlySwatter = this;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (WorldManager.Instance.RoundIsRunning)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                Attack();

            if (!isAttacking)
                UpdatePosition();
        }
	}

    private void UpdatePosition()
    {
        Vector3 mousePosViewport = Input.mousePosition;
        mousePosViewport.z = -Camera.main.gameObject.transform.position.z;

        gameObject.transform.position = Camera.main.ScreenToWorldPoint(mousePosViewport);
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            myAnimator.SetBool("IsAttacking", isAttacking);

            WorldManager.Instance.OnSwatterAttackStarted();

            whipSound.Play();
        }
    }

    public void AttackEnter()
    {
        myCollider.enabled = true;

    }

    public void AttackExit()
    {
        if (WorldManager.Instance.TheFly)
        {
            WorldManager.Instance.TheFly.OnSwatterAttackEnded();
        }

        if (hitFly)
            WorldManager.Instance.OnHit(firstHit);

        else
            WorldManager.Instance.OnMissed();

        hitFly = false;
        firstHit = false;
    }

    public void CancelAttackAnimation()
    {
        OnAttackAnimationEnd();
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        myCollider.enabled = false;
        myAnimator.SetBool("IsAttacking", isAttacking);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        FlyBehavior fly  = other.GetComponent<FlyBehavior>();

        if (fly)
        {
            hitFly = true;
            firstHit = fly.Die();
        }
    }

    public void Hide()
    {
        bottomObject.SetActive(false);
    }

    public void Unhide()
    {
        bottomObject.SetActive(true);
    }
    
}
