using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour {

    public float X_from, X_to;
    public float Z_from, Z_to;
    public GameObject chipmunkPrefab;

    [Header("Only for debug use")]
    [Header("Set dynamically")]
    public GameObject gameManager;
    private Vector3 spawnPosition;
    private int playerIndex;

    public enum Team
    {
        red = 1, blue = 2, yellow = 3, green = 4
    }

    public Team team;

    // Use this for initialization
    void Start() {
        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update() {
        //if (bomb explodes)

    }

    public void InitialTeam(int teamSize)
    {
        SignTeam();
        SpawnChipmunk(teamSize);
        ChoosePlayer();
    }

    private void SignTeam()
    {
        string teamTag = transform.tag;
        if (teamTag == "team red")
        {
            team = Team.red;
        }
        else if (teamTag == "team blue")
        {
            team = Team.blue;
        }
        else if (teamTag == "team yellow")
        {
            team = Team.yellow;
        }
        else if (teamTag == "team green")
        {
            team = Team.green;
        }
    }

    /**
     * Spawn specific number of chipmunk for this team
    **/ 
    private void SpawnChipmunk(int teamSize)
    {
        for (int i = 0; i < teamSize; i++) {
            // set the random position of every new enemies
            spawnPosition.x = Random.Range(X_from, X_to);
            spawnPosition.z = Random.Range(Z_from, Z_to);
            spawnPosition.y = transform.position.y + 1.5f;

            GameObject chipmunk = Instantiate<GameObject>(chipmunkPrefab);

            chipmunk.transform.position = spawnPosition;
            chipmunk.transform.parent = transform;
            chipmunk.tag = transform.tag;

            chipmunk.GetComponent<NPCBehavior>().enabled = true;
            chipmunk.GetComponent<PlayerBehavior>().enabled = false;

        }
    }

    /**
     * chose a random player from current this children 
     **/
    private void ChoosePlayer() {
        if(transform.childCount != 0)
        {
            playerIndex = Random.Range(0, transform.childCount - 1);
            Transform player = transform.GetChild(playerIndex);
            player.tag = transform.tag + " player";

            player.GetComponent<NPCBehavior>().enabled = false;
            player.GetComponent<PlayerBehavior>().enabled = true;
        }
    }


    /**
     * update input transform list with current chipmunks transform in this team children
     **/
    public void GetCurrentChipmunks(List<Transform> team)
    {
        foreach (Transform child in transform) {
            team.Add(child);
        }
    }

    /**
     * return current chipmunks count of this team
    **/
    public int GetCurrentChipmunksCount()
    {
        return transform.childCount;
    }

    /**
     * return the player object index in this team children
     **/
    public int GetPlayerIndex() {
        return playerIndex;
    }

    /**
     * return the player transform
     **/
    public Transform GetPlayerTransform()
    {
        return transform.GetChild(playerIndex);
    }

    /**
     * return the team name with int (0: red, 1: blue, 2: yellow, 3: green)
     **/
    public int GetTeamName()
    {
        return (int)team;
    }

    public void DestroyAllChildren()
    {
        foreach (Transform chipmunk in transform)
        {
            Destroy(chipmunk.gameObject);
        }
    }

    public void Explode(){
        transform.GetChild(playerIndex).gameObject.GetComponentInChildren<CherryBomb>().Explode();
    }
}
