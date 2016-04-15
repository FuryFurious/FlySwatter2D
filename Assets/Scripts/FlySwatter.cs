using UnityEngine;
using System.Collections;

public class FlySwatter : MonoBehaviour {

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
        Debug.Log("enabled");
        myCollider.enabled = true;
    }

    public void AttackExit()
    {
        Debug.Log("disabled");
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
            Debug.Log("hit");
            fly.Die();
        }
    }
    
}
