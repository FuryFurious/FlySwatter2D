using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour {

    public static WorldManager Instance { get; private set; }

    public GameObject FlyPrefab;

    public GameObject uiCanvas;
    public Text roundText;
    public RoundInfo[] rounds;

    [HideInInspector]
    public FlyBehavior theFly;
    private int curRound = -1;
    private float remainingRoundTime;

    private bool roundIsRunning = false;

    void Awake()
    {
        Instance = this;

        EndRound();
    }

    void Start()
    {
       
    }

    void Update()
    {
        if (roundIsRunning)
        {
            this.remainingRoundTime -= Time.deltaTime;

            if (this.remainingRoundTime < 0.0f)
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


    public void StartRound()
    {
        if (this.theFly)
        {
            Destroy(this.theFly.gameObject);
        }

        GameObject newFly = (GameObject)Instantiate(this.FlyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newFly.GetComponent<FlyBehavior>().Init(curRound);

        Time.timeScale = 1.0f;
        this.remainingRoundTime = rounds[this.curRound].roundTime;

        this.roundIsRunning = true;


        this.uiCanvas.SetActive(false);
    }

    private void EndRound()
    {
        if (this.theFly)
        {
            this.theFly.Pause(true);
        }

        Time.timeScale = 0.0f;
        roundIsRunning = false;
        this.uiCanvas.SetActive(true);

        this.curRound++;

        roundText.text = rounds[this.curRound].text;
    }
}
