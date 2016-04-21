using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class WorldManager : MonoBehaviour {

    public static WorldManager Instance { get; private set; }

    public GameObject FlyPrefab;

    public string endText;
    public GameObject uiCanvas;
    public Text roundText;
    public Text continueText;
    public Text roundTimerText;

    public Transform min;
    public Transform max;

    [SerializeField]
    private RoundInfo[] rounds;

    private int curRound = -1;
    private float currentRoundTime;
    private float remainingRoundTime;
    private bool roundIsRunning = false;
    private bool showEndScreen = false;

    [HideInInspector]
    public FlyBehavior TheFly;
    public FlySwatter TheFlySwatter;

    public AudioSource disappointedCrowdSound;
    public AudioSource happyKidsSound;

    private int hitCount = 0;
    private int missCount = 0;
    private int hitDeadCount = 0;

    void Awake()
    {
        Instance = this;
     

        /*

        <b>Herzlich Willkommen bei �Klatsch die Fliege�!</b>NEWDie Aufgabe des Spiels ist es, die Fliege mit einer Fliegenklatsche so schnell und oft wie m�glich zu t�ten.NEWDie Steuerung der Fliegenklatsche erfolgt mit Hilfe der Maus, mit der Sie sich frei �ber das gesamte Spielfeld bewegen k�nnen. Mit der linken oder rechten Maustaste k�nnen Sie die Fliegenklatsche benutzen und sehen, wie sie zuschl�gt.NEWW�hrend des Spiels h�ren Sie das Summen der Fliege. Nach jedem Versuch, die Fliege zu treffen, bekommen Sie ein akustisches Feedback. Im Falle des Erfolgs h�ren Sie ein Jubeln beziehungsweise bei einem Fehlversuch ein Raunen.NEWDas Spiel ist in 4 Bl�cke unterteilt. Zwischen den einzelnen Bl�cken k�nnen Sie eine Pause machen. Um den n�chsten Block zu beginnen, dr�cken Sie die �Enter�-Taste. Der erste Block dient als Trainingsblock.NEW<b>Wichtige Anmerkung: Bitte achten Sie darauf, nur die Hand, mit der Sie das Spiel steuern, zu bewegen.</b>
        <b>Herzlich Willkommen bei �Klatsch die Fliege�!</b>\nDie Aufgabe des Spiels ist es, die Fliege mit einer Fliegenklatsche so schnell und oft wie m�glich zu t�ten.\nDie Steuerung der Fliegenklatsche erfolgt mit Hilfe der Maus, mit der Sie sich frei �ber das gesamte Spielfeld bewegen k�nnen. Mit der linken oder rechten Maustaste k�nnen Sie die Fliegenklatsche benutzen und sehen, wie sie zuschl�gt.\nW�hrend des Spiels h�ren Sie das Summen der Fliege. Nach jedem Versuch, die Fliege zu treffen, bekommen Sie ein akustisches Feedback. Im Falle des Erfolgs h�ren Sie ein Jubeln beziehungsweise bei einem Fehlversuch ein Raunen.\nDas Spiel ist in 4 Bl�cke unterteilt. Zwischen den einzelnen Bl�cken k�nnen Sie eine Pause machen. Um den n�chsten Block zu beginnen, dr�cken Sie die �Enter�-Taste. Der erste Block dient als Trainingsblock.\n<b>Wichtige Anmerkung: Bitte achten Sie darauf, nur die Hand, mit der Sie das Spiel steuern, zu bewegen.</b>
        
        
        <b>Block 1: Trainingsphase</b>NEWZielen Sie mit Hilfe der Fliegenklatsche auf die Fliege.NEWUm die Fliegenklatsche zu benutzen, dr�cken Sie die linke oder rechte Maustaste.
        <b>Block 1 ist geschafft!</b>NEWDr�cken Sie �Enter�, um den <b>2. Block </b> zu starten.
        <b>Block 2 ist geschafft!</b>NEWDr�cken Sie �Enter�, um den <b>3. Block </b> zu starten.
        <b>Block 3 ist geschafft!</b>NEWDr�cken Sie �Enter�, um den <b>4. Block </b> zu starten.
        <b>Block 4 ist geschafft!</b>NEWVielen Dank, dass Sie mitgespielt haben.NEWSie erhalten jetzt einen Fragebogen.NEWDr�cken Sie �Enter�, um das Programm zu schlie�en.
    };
        */

    }

    void Start()
    {
        //  Cursor.visible = false;
        EndRound();
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
            if (Input.GetKeyDown(KeyCode.Return))
            {
                WriteResults();
                Application.Quit();
            }
        }
       
    }

    private void WriteResults()
    {
        string[] contents = { "Treffer: " + hitCount, "Nicht-Treffer: " + missCount };

        int count = 0;
        string name = "result0.txt";
        do
        {
            name = string.Concat("result.txt", count);
            count++;
        } while (File.Exists(name));


        File.WriteAllLines(name, contents);
    }

    public void StartRound()
    {
        roundTimerText.gameObject.SetActive(true);
        TheFlySwatter.Unhide();

        CreateAFly();

        currentRoundTime = 0.0f;
        Time.timeScale = 1.0f;
        

        this.roundIsRunning = true;


        this.uiCanvas.SetActive(false);
    }

    public void CreateAFly()
    {
        Vector3 startPos;
        int tryCount = 0;
        do
        {
            float randX = Random.Range(min.transform.position.x, max.transform.position.x);
            float randY = Random.Range(min.transform.position.y, max.transform.position.y);

            startPos = new Vector3(randX, randY, 0.0f);
            tryCount++;

        }
        while (Vector3.Distance(startPos, TheFlySwatter.gameObject.transform.position) < 16.0f && tryCount < 10);

  

        GameObject newFly = (GameObject)Instantiate(this.FlyPrefab, startPos, Quaternion.identity);
        newFly.GetComponent<FlyBehavior>().Init(curRound);
    }

    private void EndRound()
    {
        roundTimerText.gameObject.SetActive(false);
        TheFlySwatter.Hide();

        if (this.TheFly)
        {
            Destroy(this.TheFly.gameObject);
        }

        this.curRound++;
        Time.timeScale = 0.0f;
        roundIsRunning = false;
        this.uiCanvas.SetActive(true);

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
        disappointedCrowdSound.Play();


        missCount++;
    }

    public void OnHit(bool firstHit)
    {
        if (firstHit)
        {          
            happyKidsSound.Play();
            hitCount++;
        }

        else
        {
            hitDeadCount++;
        }
    }


    
}
