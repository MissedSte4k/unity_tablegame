using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlagBlue : NetworkBehaviour
{

    bool VeliavaYra = true;
    bool FlagMoved = false;
    Vector3 BasePos;
    Vector3 StoreAfterCapture;
    CharacterControl PlayerWithFlag = null;
    

    // Use this for initialization
    void Start()
    {
        StoreAfterCapture.x = -80F;
        StoreAfterCapture.y = 25F;
        StoreAfterCapture.z = 33F;
        BasePos = this.gameObject.transform.position;
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
        if (FlagMoved)
        {
            if(this.gameObject.transform.position.y < 20F)
            {
                FlagReset();
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
                    FlagSoundController.acInstance.PlayTakenSound();
                }
                if (member.Team() == 1)
                {
                    if (FlagMoved)
                    {
                        FlagReset();
                        FlagMoved = false;
                        FlagSoundController.acInstance.PlayRetakenSound();

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
        VeliavaYra = true;
        FlagMoved = false;
        PlayerWithFlag = null;
        FlagReset();
        FlagSoundController.acInstance.PlayCapturedSound();
    }
    void FlagReset()
    {
        MoveFlag(BasePos);
        
    }
}