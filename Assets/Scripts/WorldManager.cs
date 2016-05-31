using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour {

    public static WorldManager Instance { get; private set; }

    public GameObject FlyPrefab;

    public string endText;
    public GameObject pauseCanvas;
    public GameObject inGameCanvas;
    public Text roundText;
    public Text continueText;
    public Text roundTimerText;

    public Text roundHitsText;
    public Text roundMissesText;

    /// <summary>Minimum transform of the level (minimum boundary in which the fly should fly)</summary>
    public Transform min;
    public Transform max;

    [SerializeField]
    private RoundInfo[] rounds;

    private int curRound = -1;
    private float currentRoundTime;
    private float remainingRoundTime;
    private bool roundIsRunning = false;
    private bool showEndScreen = false;

    public bool RoundIsRunning { get { return roundIsRunning; } }

    [HideInInspector]
    public FlyBehavior TheFly;
    public FlySwatter TheFlySwatter;

    public AudioSource disappointedCrowdSound;
    public AudioSource happyKidsSound;

    private int hitCount = 0;
    private int missCount = 0;
    private int hitDeadCount = 0;

    private int[] roundHits;
    private int[] roundMisses;

    private bool wroteResults;


    public int GetCurRound()
    {
        return curRound;
    }

    [SerializeField]
    private float timeSinceLastClick;
    [SerializeField]
    private int timeWhenSoundIncrease = 15;
    private int lastSoundIncreaseTime = -1;

    [SerializeField]
    private float volumeIncreaseOnMiss = 0.05f;
    [SerializeField]
    private float volumeIncreaseAfterTime = 0.1f;

    void Awake()
    {
        Instance = this;

        SemainePreAdapter.Init();

        roundHits = new int[rounds.Length - 1];
        roundMisses = new int[roundHits.Length];

#if !UNITY_EDITOR
        if (File.Exists("rundenZeiten.txt"))
        {
            string[] tmpRoundTime = File.ReadAllLines("rundenZeiten.txt");

            if (tmpRoundTime.Length == rounds.Length - 1)
            {
                for (int i = 0; i < tmpRoundTime.Length; i++)
			    {
			        int tryVal = -1;

                    if(int.TryParse(tmpRoundTime[i], out tryVal)){
                        rounds[i].roundTime = (float)tryVal;
                    }
			    }
            }
        }
#endif

    }

    void SetRoundHitsText()
    {
        roundHitsText.text = "Treffer: " + roundHits[curRound];
    }

    void SetRoundMissesText()
    {
        roundMissesText.text = "Nicht-Treffer: " + roundMisses[curRound];
    }

    void Start()
    {
        SemainePreAdapter.SendSemaineEvent(SemainePreAdapter.SemaineEvent.GameStarted);
        EndRound();
    }

    void OnDestroy()
    {
        SemainePreAdapter.SendSemaineEvent(SemainePreAdapter.SemaineEvent.GameEnded);
    }


    private void SetRoundTimerText()
    {
        int seconds = Mathf.RoundToInt(this.currentRoundTime);
        int minutes = seconds / 60;
        seconds = seconds % 60;

        roundTimerText.text = string.Concat((minutes < 10 ? ("0" + minutes) : ""+minutes), ":", (seconds < 10 ? ("0" + seconds) : ""+seconds));
    }

    

    void Update()
    {
        if (!showEndScreen)
        {

            if (roundIsRunning)
            {
                this.remainingRoundTime -= Time.deltaTime;
                currentRoundTime += Time.deltaTime;
                timeSinceLastClick += Time.deltaTime;

                if ((int)timeSinceLastClick % timeWhenSoundIncrease == 0 && (int)timeSinceLastClick != lastSoundIncreaseTime)
                {
                    lastSoundIncreaseTime = (int)timeSinceLastClick;

                    if (TheFly)
                    {
                        TheFly.IncreaseSoundVolume(volumeIncreaseAfterTime);
                    }
                }

                SetRoundTimerText();
                if (remainingRoundTime <= 0.0f)
                {
                    EndRound();
                }
            }

            else
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    StartRound();
            }
        }

        else
        {
            if(!wroteResults)
            {
                WriteResults();
                wroteResults = true;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Application.Quit();
            }
        }
       
    }

    private void WriteResults()
    {
        //string[] contents = { "Treffer: " + hitCount, "Nicht-Treffer: " + missCount };
        List<string> contents = new List<string>();
        contents.Add(string.Concat("Treffer gesamt: ", hitCount));
        contents.Add(string.Concat("Nicht-Treffer gesamt: ", missCount));

        for (int i = 0; i < roundHits.Length; i++)
        {
            contents.Add(string.Concat("Treffer in Runde ", (i + 1), ": ", roundHits[i]));
            contents.Add(string.Concat("Nicht-Treffer in Runde ", (i + 1), ": ", roundMisses[i]));
        }

        int count = 0;
        string name = "result0.txt";
        do
        {
            name = string.Format("result{0}.txt", count);
            count++;
        } while (File.Exists(name));


        File.WriteAllLines(name, contents.ToArray());
    }

    public void StartRound()
    {
        SemainePreAdapter.SendSemaineEvent(SemainePreAdapter.SemaineEvent.RoundStarted);

        SetRoundHitsText();
        SetRoundMissesText();

        ResetLastClickTimer(true);

        //roundTimerText.gameObject.SetActive(true);
        inGameCanvas.gameObject.SetActive(true);
        TheFlySwatter.Unhide();
        
        CreateAFly();

        currentRoundTime = 0.0f;
        Time.timeScale = 1.0f;
        
        this.roundIsRunning = true;
        this.pauseCanvas.SetActive(false);
    }

    public void CreateAFly()
    {
        Vector3 startPos;
        int tryCount = 0;
        do
        {
            float randX = UnityEngine.Random.Range(min.transform.position.x, max.transform.position.x);
            float randY = UnityEngine.Random.Range(min.transform.position.y, max.transform.position.y);

            startPos = new Vector3(randX, randY, 0.0f);
            tryCount++;

        }
        while (Vector3.Distance(startPos, TheFlySwatter.gameObject.transform.position) < 16.0f && tryCount < 10);

        GameObject newFly = (GameObject)Instantiate(this.FlyPrefab, startPos, Quaternion.identity);
        newFly.GetComponent<FlyBehavior>().Init(curRound);
    }

    private void EndRound()
    {
        SemainePreAdapter.SendSemaineEvent(SemainePreAdapter.SemaineEvent.RoundEnded);


        inGameCanvas.gameObject.SetActive(false);

        TheFlySwatter.Hide();

        if (this.TheFly)
        {
            Destroy(this.TheFly.gameObject);
        }

        this.curRound++;
        Time.timeScale = 0.0f;
        roundIsRunning = false;
        this.pauseCanvas.SetActive(true);

        if (curRound == rounds.Length - 1)
        {
            showEndScreen = true;
            continueText.text = endText;
        }

        string tmpText = rounds[this.curRound].text.Replace("NEW", "\n");
        roundText.text = tmpText;
        continueText.text = rounds[this.curRound].continueText.Replace("NEW", "\n");
        this.remainingRoundTime = rounds[this.curRound].roundTime;

        SetRoundTimerText();

    }

    public void OnMissed()
    {
        SemainePreAdapter.SendSemaineEvent(SemainePreAdapter.SemaineEvent.OnFlyMiss);

        ResetLastClickTimer(false);

        if (TheFly)
            TheFly.IncreaseSoundVolume(volumeIncreaseOnMiss);

        disappointedCrowdSound.Play();

        roundMisses[curRound]++;
        SetRoundMissesText();
        missCount++;

    }

    public void OnHit(bool firstHit)
    {
        ResetLastClickTimer(true);

        if (firstHit)
        {          
            happyKidsSound.Play();
            hitCount++;

            SemainePreAdapter.SendSemaineEvent(SemainePreAdapter.SemaineEvent.OnFlyKill);

            roundHits[curRound]++;

            SetRoundHitsText();
        }

        else
        {
            hitDeadCount++;
        }
    }

    private void ResetLastClickTimer(bool resetVolume)
    {
        timeSinceLastClick = 0.0f;
        lastSoundIncreaseTime = 0;

        if (TheFly && resetVolume)
            TheFly.ResetSoundVolume();
    }

    public void OnSwatterAttackStarted()
    {
        if (TheFly)
            TheFly.OnSwatterAttackStarted();

    }
}
