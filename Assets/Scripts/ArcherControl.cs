using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArcherControl : NetworkBehaviour {

    public int primaryWeapon;
    public int secondaryWeapon;
    public GameObject[] weapons;
    public GameObject[] projectileModels;
    public GameObject[] back;
    public string[] weaponNames;
    [SyncVar]
    private int currentWeapon;
    private NetworkAnimator anim;
    public Transform bulletSpawn;

    public int longbowArrowSpeed;
    public int shortbowArrowSpeed;
    public int lightCrossbowArrowSpeed;
    public int heavyCrossbowArrowSpeed;
    public int javelinSpeed;
    public int trapSpeed;

    public int longbowDamage;
    public int shortbowDamage;
    public int lightCrossbowDamage;
    public int heavyCrossbowDamage;
    public int javelinDamage;

    bool javelinActive;
    bool trapActive;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        if (primaryWeapon == 4)
        {
            javelinActive = true;
        }
        else
        {
            javelinActive = false;
        }

        if (secondaryWeapon == 8 || secondaryWeapon == 9)
        {
            trapActive = true;
        }
        else
        {
            trapActive = false;
        }

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
        anim.animator.ResetTrigger("Throw");

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_idle"))
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

            if (secondaryWeapon == 8 || secondaryWeapon == 9)
            {
                if (trapActive)
                {
                    weapons[primaryWeapon].SetActive(false);
                    weapons[secondaryWeapon].SetActive(true);
                }
                else
                {
                    weapons[primaryWeapon].SetActive(false);
                    weapons[secondaryWeapon].SetActive(false);
                }
            }
            else
            {

                weapons[primaryWeapon].SetActive(true);
                weapons[secondaryWeapon].SetActive(false);
            }

            anim.animator.SetBool(weaponNames[primaryWeapon], false);
            anim.animator.SetBool(weaponNames[secondaryWeapon], true);
            currentWeapon = secondaryWeapon;
        }
        else
        {
            if (primaryWeapon == 4)
            {
                weapons[10].SetActive(true);
                if (javelinActive)
                {
                    weapons[primaryWeapon].SetActive(true);
                    weapons[secondaryWeapon].SetActive(false);
                }
                else
                {
                    weapons[primaryWeapon].SetActive(false);
                    weapons[secondaryWeapon].SetActive(false);
                }

            }
            else
            {

                weapons[primaryWeapon].SetActive(true);
                weapons[secondaryWeapon].SetActive(false);
            }
            anim.animator.SetBool(weaponNames[secondaryWeapon], false);
            anim.animator.SetBool(weaponNames[primaryWeapon], true);
            currentWeapon = primaryWeapon;
        }

        if (primaryWeapon < 4)
        {
            back[0].SetActive(true);
        } else
        {
            back[1].SetActive(true);
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

    void BowArrowTake()
    {
        projectileModels[5].SetActive(true);
        projectileModels[6].SetActive(false);
        projectileModels[currentWeapon].SetActive(false);
        CmdBowArrowTake();
    }

    [Command]
    void CmdBowArrowTake()
    {
        projectileModels[5].SetActive(true);
        projectileModels[6].SetActive(false);
        projectileModels[currentWeapon].SetActive(false);
        RpcBowArrowTake();
    }

    [ClientRpc]
    void RpcBowArrowTake()
    {
        projectileModels[5].SetActive(true);
        projectileModels[6].SetActive(false);
        projectileModels[currentWeapon].SetActive(false);
    }

    void BowArrowPlace()
    {
        projectileModels[5].SetActive(false);
        projectileModels[currentWeapon].SetActive(true);
        projectileModels[6].SetActive(true);
        CmdBowArrowPlace();
    }

    [Command]
    void CmdBowArrowPlace()
    {
        projectileModels[5].SetActive(false);
        projectileModels[currentWeapon].SetActive(true);
        projectileModels[6].SetActive(true);
        RpcBowArrowPlace();
    }

    [ClientRpc]
    void RpcBowArrowPlace()
    {
        projectileModels[5].SetActive(false);
        projectileModels[currentWeapon].SetActive(true);
        projectileModels[6].SetActive(true);
    }

    void JavelinTake()
    {
        projectileModels[7].SetActive(false);
        javelinActive = true;
        CmdJavelinTake();
    }

    void CmdJavelinTake()
    {
        projectileModels[7].SetActive(false);
        javelinActive = true;
        RpcJavelinTake();
    }

    void RpcJavelinTake()
    {
        projectileModels[7].SetActive(false);
        javelinActive = true;
    }

    void JavelinReset()
    {
        projectileModels[7].SetActive(true);
        CmdJavelinReset();
    }

    void CmdJavelinReset()
    {
        projectileModels[7].SetActive(true);
        RpcJavelinReset();
    }

    void RpcJavelinReset()
    {
        projectileModels[7].SetActive(true);
    }

    void TrapReset()
    {
        projectileModels[currentWeapon].SetActive(true);
        CmdTrapReset();
    }

    void CmdTrapReset()
    {
        projectileModels[currentWeapon].SetActive(true);
        RpcJavelinReset();
    }

    void RpcTrapReset()
    {
        projectileModels[currentWeapon].SetActive(true);
    }

    void ArcherShoot(GameObject projPrefab)
    {
        projectileModels[currentWeapon].SetActive(false);
        javelinActive = false;
        trapActive = false;
        CmdArcherShoot(projPrefab);
    }

    [Command]
    void CmdArcherShoot(GameObject projPrefab)
    {
        projectileModels[currentWeapon].SetActive(false);
        javelinActive = false;
        trapActive = false;

        RpcArcherShoot();

        switch (currentWeapon)
        {
            case 0:
                bulletSpawn.localPosition = new Vector3(0.35f, 0, 1);
                break;
            case 1:
                bulletSpawn.localPosition = new Vector3(0.37f, 0, 1);
                break;
            case 2:
                bulletSpawn.localPosition = new Vector3(0.545f, -0.235f, 1.6f);
                break;
            case 3:
                bulletSpawn.localPosition = new Vector3(0.545f, -0.3f, 1.35f);
                break;
            case 4:
                bulletSpawn.localPosition = new Vector3(0.422f, 0.305f, 0.16f);
                break;
            case 8:
                bulletSpawn.localPosition = new Vector3(0.13f, -0.7f, 0.93f);
                break;
            case 9:
                bulletSpawn.localPosition = new Vector3(0.13f, -0.7f, 0.93f);
                break;
        }

        GameObject proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);

        switch (currentWeapon)
        {
            case 0:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * longbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = longbowDamage;
                break;
            case 1:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * shortbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = shortbowDamage;
                break;
            case 2:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * heavyCrossbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = heavyCrossbowDamage;
                break;
            case 3:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * lightCrossbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = lightCrossbowDamage;
                break;
            case 4:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * javelinSpeed;
                proj.GetComponent<Projectile>().damage = javelinDamage;
                break;
            case 8:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * trapSpeed;
                proj.GetComponent<ImmobilisingTrap>().enabled = true;
                foreach (Transform child in proj.transform)
                {
                    if (child.gameObject.CompareTag("Gum")) {
                        child.gameObject.SetActive(true);
                    }
                }
                break;
            case 9:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * trapSpeed;
                proj.GetComponent<ExplosiveTrap>().enabled = true;
                foreach (Transform child in proj.transform)
                {
                    if (child.gameObject.CompareTag("Battery"))
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                break;
        }

        NetworkServer.Spawn(proj);

        // Destroy the bullet after 2 seconds
        Destroy(proj, 30);
    }

    void RpcArcherShoot()
    {
        projectileModels[currentWeapon].SetActive(false);
        javelinActive = false;
        trapActive = false;
    }
}