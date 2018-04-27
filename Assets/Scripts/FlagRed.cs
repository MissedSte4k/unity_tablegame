using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlagRed : NetworkBehaviour {

    bool VeliavaYra = true;
    bool FlagMoved = false;
    float baseX = 16.36F;
    float baseY = 36.06F;
    float baseZ =  20F;
    Vector3 tempPos;
    CharacterControl PlayerWithFlag = null;

    // Use this for initialization
    void Start () {
        Vector3 tempPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if(PlayerWithFlag != null)
        if(PlayerWithFlag.GetComponent<Health>().GetHealth() <= 0)
        {
            FlagDroped(PlayerWithFlag.transform.position.x, PlayerWithFlag.transform.position.y, PlayerWithFlag.transform.position.z);
                PlayerWithFlag = null;
        }

            }


    void OnCollisionEnter(Collision collision)
    {
        if (VeliavaYra)
        {
            var hit = collision.gameObject;
            var member = hit.GetComponent<CharacterControl>();
            if (member != null)
            { 
                if (member.Team() == 1)
                {
                    member.FlagGot();
                    VeliavaYra = false;
                    FlagMoved = true;
                    PlayerWithFlag = member;
                    this.gameObject.SetActive(false);
                }
                if (member.Team() == 2)
                {
                    if (FlagMoved)
                    {
                        MoveFlag(baseX, baseY, baseZ);
                        FlagMoved = false;

                    }
                    else
                    {
                        if(member.FlagStatus())
                        {
                            FlagCaptured();
                        }
                    }
                    
                }
            }
        }
    }
    void FlagDroped(float x, float y, float z)
    {
        MoveFlag(x, y, z);
        VeliavaYra = true;
    }
    void MoveFlag(float x, float y, float z)
    {
        tempPos.x = x;
        tempPos.y = y;
        tempPos.z = z;
        transform.position = tempPos;

    }
    void FlagCaptured()
    {

    }
}

