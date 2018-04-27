using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlagBlue : NetworkBehaviour
{

    bool VeliavaYra = true;
    bool FlagMoved = false;
    float baseX = -7.703197F;
    float baseY = 35.257F;
    float baseZ = 75.40736F;
    Vector3 BasePos;
    Vector3 StoreAfterCapture;
    CharacterControl PlayerWithFlag = null;

    // Use this for initialization
    void Start()
    {
        StoreAfterCapture.x = -80F;
        StoreAfterCapture.y = 25F;
        StoreAfterCapture.z = 33F;
        BasePos.x = baseX;
        BasePos.y = baseY;
        BasePos.z = baseZ;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerWithFlag != null)
        {
            if (PlayerWithFlag.GetComponent<Health>().CurrentHealth() <= 0)
            {
                FlagDroped(PlayerWithFlag.transform.position);
                PlayerWithFlag = null;
            }
            else if(PlayerWithFlag.FlagStatus() == 2)
            {
                FlagCaptured();
            }
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
                if (member.Team() == 2)
                {
                    member.FlagGot();
                    VeliavaYra = false;
                    FlagMoved = true;
                    PlayerWithFlag = member;
                    MoveFlag(StoreAfterCapture);
                }
                if (member.Team() == 1)
                {
                    if (FlagMoved)
                    {
                        FlagReset();
                        FlagMoved = false;

                    }
                    else
                    {
                        if (member.FlagStatus() == 1)
                        {
                            member.FlagCaptured();
                        }
                    }

                }
            }
        }
    }
    void FlagDroped(Vector3 a)
    {
        MoveFlag(a);
        VeliavaYra = true;
    }
    void MoveFlag(Vector3 a)
    {
        transform.position = a;
    }
    void FlagCaptured()
    {
        FlagReset();
        VeliavaYra = true;
        FlagMoved = false;
        PlayerWithFlag = null;
    }
    void FlagReset()
    {
        MoveFlag(BasePos);
    }
}