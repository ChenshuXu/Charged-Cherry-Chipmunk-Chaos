using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryBomb : MonoBehaviour {

    [Header("Debug")]
    public List<GameObject> teamRedChipmunks;
    public List<GameObject> teamBlueChipmunks;
    public List<GameObject> teamYellowChipmunks;
    public List<GameObject> teamGreenChipmunks;

    [Header("Explosion settings")]
    public float explisionRadius = 5.0f;
    public float explosionPower = 10f;

    private int score;
    private PlayerBehavior player;
    private List<GameObject> hitsList;
    private int npcPoints;
    private int playerPoints;

    // Use this for initialization
    void Start () {
        player = GetComponentInParent<PlayerBehavior>();
        hitsList = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
	 * start a explosion to explode others
	 */
    private void UpdateHitList()
    {
        // raycast may hit an obstacle for many times in a single explosion
        // so we need a list to prevent duplicated hit

        // start a raycast from the position of player
        for (int angle = 0; angle < 360; angle += 1)
        {
            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);
            Vector3 dir = new Vector3(x, 0, y);
            RaycastHit hit;
            if (Physics.Raycast(transform.parent.position, dir, out hit, explisionRadius))
            {
                //RaycastHit hit = hits[i];
                GameObject obj = hit.collider.gameObject;

                // kill other players
                if ((obj.GetComponent<PlayerBehavior>() || obj.GetComponent<NPCBehavior>()) && !hitsList.Contains(obj) && obj.transform.tag != "Bomb")
                {
                    hitsList.Add(obj);
                }
            }
        }
    }

    public void UpdateScore()
    {
        UpdateHitList();
        score = 0;
        Debug.Log(hitsList.Count);
        foreach (GameObject obj in hitsList)
        {
            if (obj.transform.tag + " player" != transform.parent.tag && 
                !obj.transform.tag.Contains("player"))
            {
                score += npcPoints;
            }
            else if (obj.transform.tag != transform.parent.tag)
            {
                score += playerPoints;
            }
        }
        Debug.Log(score);
        GameObject.Find("GameManager").GetComponent<GameManager>().UpdateScore((int)player.team, score);
    }

    public void Explode()
    {
        foreach (GameObject obj in hitsList)
        {
            // kill players
            if (obj.GetComponent<PlayerBehavior>().enabled)
            {
                obj.GetComponent<PlayerBehavior>().BeExploded();
            }
            // kill NPCs
            else if (obj.GetComponent<NPCBehavior>().enabled)
            {
                obj.GetComponent<NPCBehavior>().BeExploded();
            }
        }

        player.BeExploded();
    }

    public void SetTeamRemainChipmunks(List<GameObject> teamRed, List<GameObject> teamBlue, List<GameObject> teamYellow, List<GameObject> teamGreen)
    {
        teamRedChipmunks = teamRed;
        teamBlueChipmunks = teamBlue;
        teamYellowChipmunks = teamYellow;
        teamGreenChipmunks = teamGreen;
    }

    /*
     * Set up the kill points for player and npc
     */
    public void setPoints(int player, int npc)
    {
        playerPoints = player;
        npcPoints = npc;
    }
}
