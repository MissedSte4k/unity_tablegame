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
    [SyncVar]
    private int currentWeapon;
    private NetworkAnimator anim;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
            return;

        currentWeapon = primaryWeapon;
        anim = GetComponent<NetworkAnimator>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        ActivateWeapon();
        CmdActivateWeapon();

        anim.animator.ResetTrigger("Attack2");
        anim.animator.ResetTrigger("Attack3");
        anim.animator.ResetTrigger("Parry");
        anim.animator.ResetTrigger("Reload");
        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
        {
            if (Input.GetButtonDown("Swap"))
            {
                SwapWeapons();
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

    public void ActivateWeapon()
    {
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
            currentWeapon = secondaryWeapon;
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
            currentWeapon = primaryWeapon;
        }

    }

    public void SwapWeapons()
    {
        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
        {
            if (currentWeapon == primaryWeapon)
            {
                currentWeapon = secondaryWeapon;
            }
            else if (currentWeapon == secondaryWeapon)
            {
                currentWeapon = primaryWeapon;
            }
        }
    }

    [Command]
    void CmdActivateWeapon()
    {
        ActivateWeapon();
        RpcActivateWeapon();
    }

    [ClientRpc]
    void RpcActivateWeapon()
    {
        ActivateWeapon();
    }

    [Command]
    void CmdSwapWeapons()
    {
        SwapWeapons();
        RpcSwapWeapons();
    }

    [ClientRpc]
    void RpcSwapWeapons()
    {
        SwapWeapons();
    }
}