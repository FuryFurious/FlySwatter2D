using UnityEngine;
using System.Collections;
using System;

public class FlySwatter : MonoBehaviour {

    public Animator myAnimator;
    public BoxCollider2D myCollider;
    public GameObject bottomObject;

    private bool isAttackin;
    public bool IsAttacking { get { return isAttackin; } private set { SetIsAttacking(value); } }

    private int roundWhenStartedAttack = -1;

    private void SetIsAttacking(bool value)
    {
        isAttackin = value;
        myAnimator.SetBool("IsAttacking", value);
    }

    bool hitFly = false;
    private bool firstHit = false;

    public AudioSource whipSound;

    bool canCollide = false;

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

            if (!IsAttacking)
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
        if (!IsAttacking)
        {
            IsAttacking = true;

            WorldManager.Instance.OnSwatterAttackStarted();

            whipSound.Play();

            hitFly = false;
            firstHit = false;

            roundWhenStartedAttack = WorldManager.Instance.GetCurRound();
        }
    }

    public void AttackEnter()
    {
        //myCollider.enabled = true;

        canCollide = true;
    }

    public void AttackExit()
    {
        if (roundWhenStartedAttack == WorldManager.Instance.GetCurRound())
        {
            if (hitFly)
                WorldManager.Instance.OnHit(firstHit);

            else
                WorldManager.Instance.OnMissed();
        }

        if (WorldManager.Instance.TheFly)
        {
            WorldManager.Instance.TheFly.OnSwatterAttackEnded();
        }

        IsAttacking = false;

        canCollide = false;
        //myCollider.enabled = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        FlyBehavior fly = other.GetComponent<FlyBehavior>();

        if (fly)
        {
            if (fly.Difficulty == 3)
                return;

            else if(canCollide)
            {
                hitFly = true;
                firstHit = fly.Die();
            }
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
