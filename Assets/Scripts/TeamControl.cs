using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TeamControl : NetworkBehaviour
{
    public bool friendlyFireOff = false;

    // Use this for initialization
    void Start()
    {

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
        foreach(CharacterControl cc in CC)
        {
            if (cc.Team() == 1) blueTeamCount++;
            if (cc.Team() == 2) redTeamCount++;
        }
        if (blueTeamCount == redTeamCount) return 0;
        if (blueTeamCount > redTeamCount) return 1;
        else return 2;
        
    }
}
