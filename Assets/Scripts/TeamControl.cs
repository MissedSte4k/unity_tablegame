using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TeamControl : NetworkBehaviour
{
    [SerializeField] int deathmatchTarget = 15;
    private int blueScoreCount = 0;
    private int redScoreCount = 0;

    // Use this for initialization
    void Start()
    {
        blueScoreCount = 0;
        redScoreCount = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int Team()
    {
        int blueTeamCount = 0;
        int redTeamCount = 0;
        CharacterControl[] CC = FindObjectsOfType<CharacterControl>();
        foreach (CharacterControl cc in CC)
        {
            if (cc.Team() == 1) blueTeamCount++;
            if (cc.Team() == 2) redTeamCount++;
        }
        if (blueTeamCount == redTeamCount) return 0;
        if (blueTeamCount < redTeamCount) return 1;
        else return 2;

    }

    public void IncreaseByOne(int team)
    {
        if (blueScoreCount < deathmatchTarget && redScoreCount < deathmatchTarget)
        {
            if (team == 2) blueScoreCount++;
            if (team == 1) redScoreCount++;
            foreach (Health h in FindObjectsOfType<Health>())
            {
                h.RpcCurrentScore(CurrentScore());
            }
        }
    }

    public int[] CurrentScore()
    {
        int[] C = { deathmatchTarget, blueScoreCount, redScoreCount, 0 };
        if (blueScoreCount == deathmatchTarget) C[3] = 1;
        else if (redScoreCount == deathmatchTarget) C[3] = 2;
        return C;
    }
}
