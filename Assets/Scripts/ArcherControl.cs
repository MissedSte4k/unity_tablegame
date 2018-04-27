using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArcherControl : NetworkBehaviour {

    public int primaryWeapon;
    public int secondaryWeapon;
    public GameObject[] weapons;
    public GameObject[] back;
    public string[] weaponNames;
    [SyncVar]
    private int currentWeapon;
    private NetworkAnimator anim;

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
        anim.animator.ResetTrigger("Light");

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

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Stahp");
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

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Stahp");
                    }
                    break;
                case 2:
                    if (Input.GetButtonDown("Fire1"))
                    {
                        anim.animator.SetBool("Empty", true);
                        anim.SetTrigger("Attack3");
                    }

                    if (Input.GetButtonDown("Reload"))
                    {
                        anim.animator.SetBool("Empty", false);
                        anim.SetTrigger("Reload");
                    }
                    break;
                case 3:
                    if (Input.GetButtonDown("Fire1"))
                    {
                        anim.animator.SetBool("Empty", true);
                        anim.SetTrigger("Attack3");
                    }

                    if (Input.GetButtonDown("Reload"))
                    {
                        anim.animator.SetBool("Empty", false);
                        anim.SetTrigger("Reload");
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

                    if (Input.GetButtonDown("Fire2"))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    if (Input.GetButtonDown("Fire3"))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    if (Input.GetButtonDown("Fire4"))
                    {
                        anim.SetTrigger("Throw");
                    }

                    if (Input.GetButton("Block"))
                    {
                        anim.animator.SetBool("Block", true);
                    }
                    else
                    {
                        anim.animator.SetBool("Block", false);
                    }

                    if (Input.GetButtonDown("Reload"))
                    {
                        anim.SetTrigger("Light");
                    }
                    break;
                case 5:
                    if (Input.GetButton("Fire1"))
                    {
                        anim.animator.SetBool("Attack1", true);
                    }
                    else
                    {
                        anim.animator.SetBool("Attack1", false);
                    }

                    if (Input.GetButtonDown("Fire2"))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    if (Input.GetButtonDown("Fire3"))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Parry");
                    }
                    break;
                case 6:
                    if (Input.GetButton("Fire1"))
                    {
                        anim.animator.SetBool("Attack1", true);
                    }
                    else
                    {
                        anim.animator.SetBool("Attack1", false);
                    }

                    if (Input.GetButtonDown("Fire2"))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    if (Input.GetButtonDown("Fire3"))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Parry");
                    }
                    break;
                case 7:
                    if (Input.GetButton("Fire1"))
                    {
                        anim.animator.SetBool("Attack1", true);
                    }
                    else
                    {
                        anim.animator.SetBool("Attack1", false);
                    }

                    if (Input.GetButtonDown("Fire2"))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    if (Input.GetButtonDown("Fire3"))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Parry");
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
                    }
                    break;
            }
        }
    }

    public void ActivateWeapon()
    {
        if (currentWeapon == secondaryWeapon)
        {
            if (primaryWeapon == 4)
            {
                weapons[10].SetActive(false);
            }

            weapons[primaryWeapon].SetActive(false);
            weapons[secondaryWeapon].SetActive(true);
            anim.animator.SetBool(weaponNames[primaryWeapon], false);
            anim.animator.SetBool(weaponNames[secondaryWeapon], true);
            currentWeapon = secondaryWeapon;
        }
        else
        {
            if (primaryWeapon == 4)
            {
                weapons[10].SetActive(true);
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
        if (currentWeapon == primaryWeapon)
        {
            currentWeapon = secondaryWeapon;
        }
        else if (currentWeapon == secondaryWeapon)
        {
            currentWeapon = primaryWeapon;
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