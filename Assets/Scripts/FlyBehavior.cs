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

    private const float FLY_BLOOD_DECAY_TIME = 1.0f;
    private const float BLOOD_DECAY_TIME_MIN = 2.0f;

    public GameObject[] bloodSplatters;
    public DecayOverTime[] ownBodyParts;
    public Animator myAnimator;
    public AudioSource flySound;

    public bool isMoving;
    public bool IsMoving { get { return isMoving; } set { SetIsMoving(value); } }

    public int Difficulty { get { return difficulty; } }

    private AFlyState[] flyStates;

    [SerializeField]
    private EFlyState curFlyStateIndex = EFlyState.Wait;

    public bool isPaused = false;
    public bool isDead = false;
    public bool soundIsPlaying = true;

    private float lifeTime = 0.0f;
    private float timeToRemove;

    [SerializeField]
    private float startVolume = 0.25f;
    [SerializeField]
    private float maxVolume = 0.75f;

    private int difficulty;

    void Start () 
    {

    }

    public void IncreaseSoundVolume(float delta)
    {
        flySound.volume = Mathf.Min(flySound.volume + delta, maxVolume);
    }

    public void ResetSoundVolume()
    {
        flySound.volume = startVolume;
    }

    public void Init(int curRound)
    {
        difficulty = curRound;

        flyStates = new AFlyState[(int)EFlyState.Count];
        
        flyStates[(int)EFlyState.Wait] = new WaitState(curRound, WorldManager.Instance.TheFlySwatter, this);
        flyStates[(int)EFlyState.Move] = new MoveState(curRound, WorldManager.Instance.TheFlySwatter, this);

        flyStates[(int)curFlyStateIndex].OnStateEnter(null);

        WorldManager.Instance.TheFly = this;

        flySound.volume = startVolume;
    }


    void Update()
    {
        if (!isPaused)
        {
            if (!isDead)
            {
                flyStates[(int)curFlyStateIndex].OnStateUpdate();
                lifeTime += Time.deltaTime;
            }

            else
            {
                timeToRemove -= Time.deltaTime;

                if (timeToRemove < 0.0f)
                    Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if(!isDead)
            flyStates[(int)curFlyStateIndex].OnStateFixedUpdate();
    }


    public void SetState(EFlyState newStateIndex)
    {
        AFlyState oldState = flyStates[(int)curFlyStateIndex];
        AFlyState newState = flyStates[(int)newStateIndex];

        oldState.OnStateExit(newState);
        newState.OnStateEnter(oldState);

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


    public void OnSwatterAttackStarted()
    {
        for (int i = 0; i < flyStates.Length; i++)
        {
            flyStates[i].OnSwatterAttackStarted(i == (int)curFlyStateIndex);
        }
    }

    public void OnSwatterAttackEnded()
    {
        for (int i = 0; i < flyStates.Length; i++)
        {
            flyStates[i].OnSwatterAttackEnded(i == (int)curFlyStateIndex);
        }

    }

    void SpawnBlood()
    {
        int num = Random.Range(1, 5);

        for (int i = 0; i < num; i++)
        {
            GameObject newObj = (GameObject)Instantiate(bloodSplatters[Random.Range(0, bloodSplatters.Length)], gameObject.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0, 360.0f)));

            Vector3 offset = new Vector3(Random.Range(BLOOD_MIN_OFFSET_X, BLOOD_MAX_OFFSET_X), Random.Range(BLOOD_MIN_OFFSET_Y, BLOOD_MAX_OFFSET_Y), 0.0f);
            Vector3 scale = new Vector3(Random.Range(BLOOD_MIN_SCALE_X, BLOOD_MAX_SCALE_X), Random.Range(BLOOD_MIN_SCALE_Y, BLOOD_MAX_SCALE_Y), 1.0f);

            newObj.transform.position += offset;
            newObj.transform.localScale = Vector3.Scale(scale, newObj.transform.localScale);

            newObj.GetComponent<DecayOverTime>().totalLifeTime = (BLOOD_DECAY_TIME_MIN);
        }


    }

    public void Remove()
    {
        GameObject.Destroy(gameObject);     
    }

    public bool Die()
    {
        SpawnBlood();

        if (!isDead)
        {
            isDead = true;
            myAnimator.SetBool("IsDead", true);

            flySound.Pause();
            soundIsPlaying = false;

            for (int i = 0; i < ownBodyParts.Length; i++)
            {
                ownBodyParts[i].totalLifeTime = FLY_BLOOD_DECAY_TIME;
                ownBodyParts[i].enabled = true;
            }

            WorldManager.Instance.CreateAFly();

            timeToRemove = FLY_BLOOD_DECAY_TIME;

            return true;
        }

        return false;
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
