using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoutControl : NetworkBehaviour {

    public int primaryWeapon;
    public int secondaryWeapon;
    public GameObject[] weapons;
    public string[] weaponNames;
    [SyncVar(hook = "ActivateWeapon")] private int currentWeapon;
    private NetworkAnimator anim;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
            return;

        currentWeapon = primaryWeapon;
        ActivateWeapon(currentWeapon);
        CmdActivateWeapon(currentWeapon);
        ShowMeWhatYouGot();
    }

    void Awake()
    {
        currentWeapon = primaryWeapon;
        ActivateWeapon(currentWeapon);
        CmdActivateWeapon(currentWeapon);
    }

    void Update()
    {
        if(isLocalPlayer)
        {

            anim.animator.ResetTrigger("Attack2");
            anim.animator.ResetTrigger("Attack3");
            anim.animator.ResetTrigger("Parry");
            anim.animator.ResetTrigger("Reload");
            if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
            {
                if (Input.GetButtonDown("Swap"))
                {
                    CmdSwapWeapons();
                }

                switch (currentWeapon)
                {
                    case 0:
                        if (Input.GetButton("Fire1"))
                        {
                            anim.animator.SetBool("Attack1", true);
                        }
                        else
                        {
                            anim.animator.SetBool("Attack1", false);
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire2"))
                        {
                            anim.SetTrigger("Attack2");
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire3"))
                        {
                            anim.SetTrigger("Attack3");
                        }

                        if (Input.GetButtonDown("Block"))
                        {
                            anim.SetTrigger("Parry");
                        }
                        break;
                    case 1:
                        if (Input.GetButton("Fire1"))
                        {
                            anim.animator.SetBool("Attack1", true);
                        }
                        else
                        {
                            anim.animator.SetBool("Attack1", false);
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire2"))
                        {
                            anim.SetTrigger("Attack2");
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire3"))
                        {
                            anim.SetTrigger("Attack3");
                        }

                        if (Input.GetButtonDown("Block"))
                        {
                            anim.SetTrigger("Parry");
                        }
                        break;
                    case 2:
                        if (Input.GetButton("Fire1"))
                        {
                            anim.animator.SetBool("Attack1", true);
                        }
                        else
                        {
                            anim.animator.SetBool("Attack1", false);
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire2"))
                        {
                            anim.SetTrigger("Attack2");
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire3"))
                        {
                            anim.SetTrigger("Attack3");
                        }

                        if (Input.GetButtonDown("Block"))
                        {
                            anim.SetTrigger("Parry");
                        }
                        break;
                    case 3:
                        if (Input.GetButton("Fire1"))
                        {
                            anim.animator.SetBool("Attack1", true);
                        }
                        else
                        {
                            anim.animator.SetBool("Attack1", false);
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire2"))
                        {
                            anim.SetTrigger("Attack2");
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire3"))
                        {
                            anim.SetTrigger("Attack3");
                        }

                        if (Input.GetButton("Block"))
                        {
                            anim.animator.SetBool("Block", true);
                        }
                        else
                        {
                            anim.animator.SetBool("Block", false);
                        }
                        break;
                    case 4:
                        if (Input.GetButton("Fire1"))
                        {
                            anim.animator.SetBool("Attack1", true);
                        }
                        else
                        {
                            anim.animator.SetBool("Attack1", false);
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire2"))
                        {
                            anim.SetTrigger("Attack2");
                        }

                        // ataka vyksta 1 kartą nuspaudus mygtuką
                        if (Input.GetButtonDown("Fire3"))
                        {
                            anim.SetTrigger("Attack3");
                        }

                        if (Input.GetButtonDown("Block"))
                        {
                            anim.SetTrigger("Parry");
                        }
                        break;
                    case 5:
                        break;
                    case 6:
                        if (Input.GetButtonDown("Fire1"))
                        {
                            anim.SetTrigger("Attack3");
                        }
                        break;
                    case 7:
                        if (Input.GetButtonDown("Fire1"))
                        {
                            anim.SetTrigger("Attack3");
                        }
                        break;
                    case 8:
                        if (Input.GetButtonDown("Fire1"))
                        {
                            anim.SetTrigger("Attack3");
                        }
                        break;
                    case 9:
                        if (Input.GetButtonDown("Fire1"))
                        {
                            anim.SetTrigger("Attack3");
                            anim.animator.SetBool("Empty", true);
                        }
                        if (Input.GetButtonDown("Reload"))
                        {
                            anim.SetTrigger("Reload");
                            anim.animator.SetBool("Empty", false);
                        }
                        break;
                }
            }
        }
    }

    public void ActivateWeapon(int value)
    {
        anim = GetComponent<NetworkAnimator>();
        currentWeapon = value;
        if (currentWeapon == secondaryWeapon)
        {
            if (primaryWeapon == 0)
            {
                weapons[10].SetActive(false);
            }
            else if (primaryWeapon == 1)
            {
                weapons[11].SetActive(false);
            }
            else if (primaryWeapon == 2)
            {
                weapons[12].SetActive(false);
            }
            else if (primaryWeapon == 3)
            {
                weapons[13].SetActive(false);
            }

            if (secondaryWeapon == 6 || secondaryWeapon == 7)
            {
                weapons[14].SetActive(true);
            }
            else if (secondaryWeapon == 8)
            {
                weapons[15].SetActive(true);
            }
            else if (secondaryWeapon == 9)
            {
                weapons[16].SetActive(true);
            }

            weapons[primaryWeapon].SetActive(false);
            weapons[secondaryWeapon].SetActive(true);
            anim.animator.SetBool(weaponNames[primaryWeapon], false);
            anim.animator.SetBool(weaponNames[secondaryWeapon], true);
        }
        else
        {
            if (primaryWeapon == 0)
            {
                weapons[10].SetActive(true);
            }
            else if (primaryWeapon == 1)
            {
                weapons[11].SetActive(true);
            }
            else if (primaryWeapon == 2)
            {
                weapons[12].SetActive(true);
            }
            else if (primaryWeapon == 3)
            {
                weapons[13].SetActive(true);
            }

            if (secondaryWeapon == 6 || secondaryWeapon == 7)
            {
                weapons[14].SetActive(false);
            }
            else if (secondaryWeapon == 8)
            {
                weapons[15].SetActive(false);
            }
            else if (secondaryWeapon == 9)
            {
                weapons[16].SetActive(false);
            }

            weapons[primaryWeapon].SetActive(true);
            weapons[secondaryWeapon].SetActive(false);
            anim.animator.SetBool(weaponNames[secondaryWeapon], false);
            anim.animator.SetBool(weaponNames[primaryWeapon], true);
        }
    }

    public int CurrentWeapon()
    {
        return currentWeapon;
    }

    private void ShowMeWhatYouGot()
    {
        KnightControl[] K = FindObjectsOfType<KnightControl>();
        foreach (KnightControl c in K)
        {
            c.ActivateWeapon(c.CurrentWeapon());
        }

        ArcherControl[] A = FindObjectsOfType<ArcherControl>();
        foreach (ArcherControl c in A)
        {
            c.ActivateWeapon(c.CurrentWeapon());
        }

        BerserkerControl[] B = FindObjectsOfType<BerserkerControl>();
        foreach (BerserkerControl c in B)
        {
            c.ActivateWeapon(c.CurrentWeapon());
        }

        ScoutControl[] S = FindObjectsOfType<ScoutControl>();
        foreach (ScoutControl c in S)
        {
            c.ActivateWeapon(c.CurrentWeapon());
        }

    [Command]
    void CmdActivateWeapon(int value)
    {
        RpcActivateWeapon(value);
    }

    [ClientRpc]
    void RpcActivateWeapon(int value)
    {
        ActivateWeapon(value);
    }

    [Command]
    void CmdSwapWeapons()
    {
        RpcSwapWeapons();
    }

    [ClientRpc]
    void RpcSwapWeapons()
    {
        if (currentWeapon == primaryWeapon)
        {
            currentWeapon = secondaryWeapon;
        }
        else /*if (currentWeapon == secondaryWeapon)*/
        {
            currentWeapon = primaryWeapon;
        }
    }
}