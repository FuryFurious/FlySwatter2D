using UnityEngine;
using System.Collections;

public class FlyBehavior : MonoBehaviour {

    public enum EFlyState { Wait, Move, Count}
    private const string idleName = "IdleCleaning";

    private const float BLOOD_MIN_OFFSET_X = -2.0f;
    private const float BLOOD_MAX_OFFSET_X = 2.0f;
    private const float BLOOD_MIN_OFFSET_Y = -2.0f;
    private const float BLOOD_MAX_OFFSET_Y = 2.0f;

    private const float BLOOD_MIN_SCALE_X = 0.75f;
    private const float BLOOD_MAX_SCALE_X = 0.75f;
    private const float BLOOD_MIN_SCALE_Y = 1.3f;
    private const float BLOOD_MAX_SCALE_Y = 1.3f;

    private const float FLY_BLOOD_DECAY_TIME = 5.0f;

    public GameObject[] bloodSplatters;
    public DecayOverTime[] ownBodyParts;
    public Animator myAnimator;
    public AudioSource flySound;

    private bool isMoving;
    public bool IsMoving { get { return isMoving; } set { SetIsMoving(value); } }

    private AFlyState[] flyStates;

    [SerializeField]
    private EFlyState curFlyStateIndex = EFlyState.Wait;

    private bool isPaused = false;
    private bool isDead = false;
    private bool soundIsPlaying = true;

    private float lifeTime = 0.0f;
    private float timeToRemove;

    void Start () 
    {
    
	}

    public void Init(int curRound)
    {
        flyStates = new AFlyState[(int)EFlyState.Count];
        
        flyStates[(int)EFlyState.Wait] = new WaitState(curRound);
        flyStates[(int)EFlyState.Move] = new MoveState(curRound);

        flyStates[(int)curFlyStateIndex].OnStateEnter(this, null);

        WorldManager.Instance.theFly = this;
    }


    void Update()
    {

        if (!isPaused)
        {

            if (!isDead)
            {
                flyStates[(int)curFlyStateIndex].OnStateUpdate(this);
                lifeTime += Time.deltaTime;
            }

            else
            {
                timeToRemove -= Time.deltaTime;

                if (timeToRemove < 0.0f)
                    Destroy(gameObject);
            }


        }
        //TODO: remove:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Die();
        }
    }


    public void SetState(EFlyState newStateIndex)
    {
        AFlyState oldState = flyStates[(int)curFlyStateIndex];
        AFlyState newState = flyStates[(int)newStateIndex];

        oldState.OnStateExit(this, newState);
        newState.OnStateEnter(this, oldState);

        this.curFlyStateIndex = newStateIndex;
    }

    public void TriggerIdle()
    {
        myAnimator.SetTrigger(idleName);
    }


    private void SetIsMoving(bool value)
    {
        this.isMoving = value;
        myAnimator.SetBool("IsMoving", this.isMoving);

        if (this.isMoving)
        {
            flySound.Play();
            soundIsPlaying = true;
        }

        else
        {
            flySound.Pause();
            soundIsPlaying = false;
        }
    }

    void OnTriggerEnter2D()
    {
       
       
    }

    void SpawnBlood()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject newObj = (GameObject)Instantiate(bloodSplatters[Random.Range(0, bloodSplatters.Length)], gameObject.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0, 360.0f)));

            Vector3 offset = new Vector3(Random.Range(BLOOD_MIN_OFFSET_X, BLOOD_MAX_OFFSET_X), Random.Range(BLOOD_MIN_OFFSET_Y, BLOOD_MAX_OFFSET_Y), 0.0f);
            Vector3 scale = new Vector3(Random.Range(BLOOD_MIN_SCALE_X, BLOOD_MAX_SCALE_X), Random.Range(BLOOD_MIN_SCALE_Y, BLOOD_MAX_SCALE_Y), 1.0f);

            newObj.transform.position += offset;
            newObj.transform.localScale = Vector3.Scale(scale, newObj.transform.localScale);

            newObj.GetComponent<DecayOverTime>().totalLifeTime = FLY_BLOOD_DECAY_TIME;
        }


    }

    public void Die()
    {
        isDead = true;
        myAnimator.SetBool("IsDead", true);
        SpawnBlood();

        flySound.Pause();
        soundIsPlaying = false;

        for (int i = 0; i < ownBodyParts.Length; i++)
        {
            ownBodyParts[i].totalLifeTime = FLY_BLOOD_DECAY_TIME;
            ownBodyParts[i].enabled = true;
        }

        timeToRemove = FLY_BLOOD_DECAY_TIME;
    }

    public void Pause(bool val)
    {
        this.isPaused = val;
        //pause the game:
        if (val)
        {
            if(soundIsPlaying)
                flySound.Pause();
        }

        else
        {
            if (soundIsPlaying)
                flySound.Play();
        }


    }


}
