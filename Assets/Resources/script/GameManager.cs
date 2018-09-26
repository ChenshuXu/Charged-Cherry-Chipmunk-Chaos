using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [Header("Main Camera")]
    public Camera multipleTargetCamera;
    public MultipleTargetCamera mtc;

    [Header("Important Info")]
    public int thisScene = 0;

    [Header("Cherry Bomb Object")]
    public GameObject cherryBombPrefab;
    public Vector3 bombOffset;
    public int playerWorthPoints = 0;
    public int npcWorthPoints = 1;

    [Header("Team Member Size")]
    public int teamMemberSize = 5;

    [Header("Team List")]
    public List<GameObject> team;
    public int teamLeft;

    [Header("Team Members")]
    public List<Transform> teamRed;   
    public List<Transform> teamBlue;  
    public List<Transform> teamYellow;
    public List<Transform> teamGreen;

    [Header("Team Remain")]
    static int teamRedRemain;
    static int teamBlueRemain;
    static int teamYellowRemain;
    static int teamGreenRemain;

    [Header("Team Score")]
    public Dictionary<string, int> teamScore;
    private GameObject lead;
    public List<GameObject> leader;

    [Header("Team Player")]
    public GameObject teamRedPlayer;
    public GameObject teamBluePlayer;
    public GameObject teamYellowPlayer;
    public GameObject teamGreenPlayer;

    [Header("Game Round")]
    public int round = 0;

    public enum RoundState { RoundStart, RoundIn, RoundEnd };
    public RoundState roundState;

    [Header("Waiting Time For New Round")]
    public float waitingTime;
    private float _waitingTime;
    private bool exploded;

    private enum Team
    {
        red = 1, blue = 2, yellow = 3, green = 4
    }

    [Header("Timer Object")]
    public float timeLeft = 10;
    private GameObject timer;
    private Text numTime;
    private GameObject boom;
    public List<GameObject> timerChildren;

    private void Start()
    {
        //Initializes the score of each team
        teamScore = new Dictionary<string, int>();
        teamScore["r"] = 0;
        teamScore["b"] = 0;
        teamScore["y"] = 0;
        teamScore["g"] = 0;

        //Finds the number timer
        numTime = GameObject.FindGameObjectWithTag("Timer").GetComponent<Text>();
        //Finds all peices of the Cherry Timer
        timer = GameObject.FindGameObjectWithTag("Cherry Timer");
        for (int x = 0; x <= 10; x++)
        {
            if (timer.transform.GetChild(x).gameObject.name != "Boom!")
            {
                timerChildren.Add(timer.transform.GetChild(x).gameObject);
            }

            else
            {
                boom = timer.transform.GetChild(x).gameObject;
            }

        }
        //Sets the multiple target camera
        mtc = multipleTargetCamera.GetComponent<MultipleTargetCamera>();

        //Find Leader Images for the leader board.
        lead = GameObject.FindGameObjectWithTag("Leader");
        for (int x = 0; x <= 15; x++)
        {
            leader.Add(lead.transform.GetChild(x).gameObject);
        }

        //Set up timer value
        _waitingTime = waitingTime;

        //Starts countdown
        StartCoroutine(StartCountdown());

        //Initial Game
        InitialGame();
    }

    void Update()
    {
        switch (roundState){
            case RoundState.RoundStart:
                
                roundState = RoundState.RoundIn;
                break;
            case RoundState.RoundIn:
                UpdateTeamMembers();
                UpdateTeamPlayer();
                UpdateCameraSetting();
                mtc.setState(1);
                
                break;
            case RoundState.RoundEnd:
                break;
        }
    }

    /*Countdown Cherry Bomb UI by calling CherryTimer
     *Countdown for number timer
     *Resets after each round by calling CHerryTimer Reset
     */
    public IEnumerator StartCountdown()
    {
        timeLeft++;
        numTime.text = "10";
        numTime.enabled = true;
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeLeft--;
            CherryTimer();
            numTime.text = (timeLeft - 1).ToString();
            if (numTime.text == "0")
            {
                numTime.enabled = false;
            }
        }

        Explode();
        //Wait time (can be changed if you want) to show scores etc then resets
        yield return new WaitForSeconds(1.0f);
        teamLeft = 0;
        mtc.setState(2);
        round++;
        UpdateTeamMembers();
        UpdateTeamRemain();
        yield return new WaitForSeconds(1.0f);
        
        foreach (GameObject child in team)
        {
            //Debug.Log(child.transform.childCount);
            if (child.transform.childCount != 0)
            {
                teamLeft++;
                TeamController teamController = child.GetComponent<TeamController>();
                teamController.DestroyAllChildren();
            }

        }
        yield return new WaitForSeconds(1.0f);
        UpdateLeader();
        timeLeft = 10;
        CherryTimerReset();
        if (teamLeft <= 3)
        {
            SceneManager.LoadScene(thisScene);
        }
        else
        {
            InitialGame();
        }


        StartCoroutine(StartCountdown());
    }

   /* Checks throught the team scores to see which is higher
    * then checks if there are any ties. After, it deactivates
    * the current leader on leader board and sets the new leader
    * if any.   
    */
    private void UpdateLeader()
    {
        string temp = "r";
        string tie = "";
      //Finds highest score
        for(int x = 0; x < 1; x++)
        {
            if (teamScore[temp] < teamScore["r"])
            {
                temp = "r";
            }
            if (teamScore[temp] < teamScore["b"])
            {
                temp = "b";
            }
            if (teamScore[temp] < teamScore["g"])
            {
                temp = "g";
            }
            if (teamScore[temp] < teamScore["y"])
            {
                temp = "y";
            }
        }
        //Finds ties
        for (int x = 0; x < 1; x++)
        {
            if (teamScore[temp] == teamScore["r"])
            {
                tie += "r";
            }
            if (teamScore[temp] == teamScore["b"])
            {
                tie += "b";
            }
            if (teamScore[temp] == teamScore["y"])
            {
                tie += "y";
            }
            if (teamScore[temp] == teamScore["g"])
            {
                tie += "g";
            }
        }
        //Disables leader
        foreach(GameObject l in leader)
        {
            l.SetActive(false);
        }
        //Enables new leader
        foreach (GameObject ll in leader)
        {
            if (ll.name == tie)
            {
                ll.SetActive(true);
            }
        }
    }

    //Resets the bomb timer after each round
    private void CherryTimerReset()
    {
        foreach (GameObject cc in timerChildren)
        {
            cc.SetActive(true);
            if (cc.name != "CherryBomb9")
            {
                cc.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        boom.SetActive(false);
    }

    //When called remove a piece of the cherry clock
    void CherryTimer()
    {
        for (int x = 0; x < timeLeft; x++)
        {
            //If Cherry Piece has the number at which the time is 
            //going to be atremove it
            if (timerChildren[x].name == ("CherryBomb" + (timeLeft - 1)))
            {
                if (timerChildren[x].name == "CherryBomb0")
                {
                    timerChildren[x].SetActive(false);
                    boom.SetActive(true);
                    break;
                }
                else
                {
                    timerChildren[x].SetActive(false);
                }
            }
            //Show the spark for the next CherryBomb
            if (timerChildren[x].name == ("CherryBomb" + (timeLeft - 2)))
            {
                timerChildren[x].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }



    /*
     * 
     * 
     * THIS PART IS FOR INITIAL GAME FUNCTION
     * 
     * 
     */

    /** 
     * Initial the game
     * spawn 5 chipmunks on each team
     * get each teammembers into different team list
     * spawn 4 cherry bomb on top of each player character
     * sign the each team player
     **/
    private void InitialGame()
    {
	    exploded = true;

        foreach (GameObject child in team)
        {
            
            TeamController teamController = child.GetComponent<TeamController>();

            TeamInitialByRound(round, teamController);

            if (child.transform.childCount != 0)
            {
                int playerIndex = teamController.GetPlayerIndex();

                Transform player = child.transform.GetChild(playerIndex);

                SpawnCherryBomb(player);
            }
        }
        roundState = RoundState.RoundStart;
    }

    /**
     * Spawn chipmunks for each team in different case
     * Game start (0 round)
     * After first round, spawn chipmunk with different number which is the remain chipmunk of each team
     **/ 
    private void TeamInitialByRound(int round, TeamController teamController)
    {
        if (round == 0)
        {
            teamController.InitialTeam(teamMemberSize);
        }
        else
        {
            switch (teamController.GetTeamName())
            {
                case (int)Team.red:
                    teamController.InitialTeam(teamRedRemain);
                    break;
                case (int)Team.blue:
                    teamController.InitialTeam(teamBlueRemain);
                    break;
                case (int)Team.yellow:
                    teamController.InitialTeam(teamYellowRemain);
                    break;
                case (int)Team.green:
                    teamController.InitialTeam(teamGreenRemain);
                    break;
            }
        }
    }

    /** 
     * Function for spawn cherry bomb on top of each player character
     **/
    private void SpawnCherryBomb(Transform player)
    {
        GameObject newCherryBomb = Instantiate<GameObject>(cherryBombPrefab);

        Vector3 newPosition = player.position + bombOffset;

        newCherryBomb.transform.position = newPosition;
        newCherryBomb.transform.parent = player;

        newCherryBomb.GetComponent<CherryBomb>().setPoints(playerWorthPoints, npcWorthPoints);
    }


    /*
     * 
     * 
     * THIS PART IS FOR UPDATE GAME DATA FUNCTION
     * 
     * 
     */

    /** 
     * Function for update current team members for each team list object
     **/
    private void UpdateTeamMembers()
    {
        foreach (GameObject child in team)
        {
            TeamController teamController = child.GetComponent<TeamController>();

            switch (teamController.GetTeamName())
            {
                case (int)Team.red:
                    teamRed = new List<Transform>(teamController.GetCurrentChipmunksCount());
                    teamController.GetCurrentChipmunks(teamRed);
                    break;
                case (int)Team.blue:
                    teamBlue = new List<Transform>(teamController.GetCurrentChipmunksCount());
                    teamController.GetCurrentChipmunks(teamBlue);
                    break;
                case (int)Team.yellow:
                    teamYellow = new List<Transform>(teamController.GetCurrentChipmunksCount());
                    teamController.GetCurrentChipmunks(teamYellow);
                    break;
                case (int)Team.green:
                    teamGreen = new List<Transform>(teamController.GetCurrentChipmunksCount());
                    teamController.GetCurrentChipmunks(teamGreen);
                    break;
            }
        }
    }

    /** 
     * Function for update new player for each team player object 
     **/
    private void UpdateTeamPlayer()
    {
        teamRedPlayer = GameObject.FindGameObjectWithTag("team red player");
        teamBluePlayer = GameObject.FindGameObjectWithTag("team blue player");
        teamYellowPlayer = GameObject.FindGameObjectWithTag("team yellow player");
        teamGreenPlayer = GameObject.FindGameObjectWithTag("team green player");
    }

    /**
     * Update number of each team chipmunk remain for next round
     **/ 
    private void UpdateTeamRemain()
    {
        teamRedRemain = teamRed.Count;
        teamBlueRemain = teamBlue.Count;
        teamYellowRemain = teamYellow.Count;
        teamGreenRemain = teamGreen.Count;
    }

    /**
     * Update remain player of each team for relocating the camera
     **/
    private void UpdateCameraSetting()
    {
        List<string> teamList = new List<string>();
        if (teamRedPlayer != null)
        {
            teamList.Add("red");
        }
        if (teamBluePlayer != null)
        {
            teamList.Add("blue");
        }
        if (teamYellowPlayer != null)
        {
            teamList.Add("yellow");
        }
        if (teamGreenPlayer != null)
        {
            teamList.Add("green");
        }

        mtc.UpdateTargetCapacity(teamList);
    }

    /**
     * Update the score of each team
     * Called by CherryBomb script
     **/
    public void UpdateScore(int team, int score)
    {
        switch (team)
        {
            case (int)Team.red:
                teamScore["r"] += score;
                break;
            case (int)Team.blue:
                teamScore["b"] += score;
                break;
            case (int)Team.yellow:
                teamScore["y"] += score;
                break;
            case (int)Team.green:
                teamScore["g"] += score;
                break;
        }
    }


    /*
     * 
     *
     * THIS PART IS FOR START A NEW ROUND FUNCTION
     * 
     * 
     */

    /**
     * Start a new round when round finish
     * Will check the each team chipmunk remain
     * and initial a new round with those amount
     **/
    private void StartNewRound()
    {
        mtc.setState(2);
        // check the chipmunks remain of each team at the end of the round
        if (waitingTime == _waitingTime)
        {
            teamLeft = 0;
            round++;
        }
            
        waitingTime -= Time.deltaTime;

        if (waitingTime >= 1f){
            UpdateTeamMembers();
            UpdateTeamRemain();
        }

        if (waitingTime <= 1f)
        {
            

            foreach (GameObject child in team)
            {
                //Debug.Log(child.transform.childCount);
                if (child.transform.childCount != 0)
                {
                    teamLeft++;
                    TeamController teamController = child.GetComponent<TeamController>();
                    teamController.DestroyAllChildren();
                }
                
            }
        }

        if (waitingTime <= 0f)
        {
            waitingTime = _waitingTime;
            if (teamLeft <= 3)
            {
                SceneManager.LoadScene(thisScene);
            }
            else
            {
                InitialGame();
            }
            
        }
    }

    private void Explode(){
        if (exploded){
            //Update each team score before destory 
            teamBluePlayer.GetComponentInChildren<CherryBomb>().UpdateScore();
            teamGreenPlayer.GetComponentInChildren<CherryBomb>().UpdateScore();
            teamRedPlayer.GetComponentInChildren<CherryBomb>().UpdateScore();
            teamYellowPlayer.GetComponentInChildren<CherryBomb>().UpdateScore();

            teamBluePlayer.GetComponentInChildren<CherryBomb>().Explode();
            teamGreenPlayer.GetComponentInChildren<CherryBomb>().Explode();
            teamRedPlayer.GetComponentInChildren<CherryBomb>().Explode();
            teamYellowPlayer.GetComponentInChildren<CherryBomb>().Explode();
        }
        exploded = false;
    }
}
