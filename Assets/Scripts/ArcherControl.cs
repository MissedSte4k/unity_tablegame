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
    [SyncVar(hook = "ActivateWeapon")] private int currentWeapon;
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

    public int javelinSlashDamage;
    public int handaxeSlashDamage;
    public int shortswordSlashDamage;
    public int daggerSlashDamage;

    public int javelinOverheadDamage;
    public int handaxeOverheadDamage;
    public int shortswordOverheadDamage;
    public int daggerOverheadDamage;

    public int javelinStabDamage;
    public int handaxeStabDamage;
    public int shortswordStabDamage;
    public int daggerStabDamage;

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
        if (!isLocalPlayer)
            return;

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

    public void ActivateWeapon(int value)
    {
        anim = GetComponent<NetworkAnimator>();
        currentWeapon = value;
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
        }

        if (primaryWeapon < 4)
        {
            back[0].SetActive(true);
        } else
        {
            back[1].SetActive(true);
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

    [Command]
    void CmdBowArrowTake()
    {
        RpcBowArrowTake();
    }

    [ClientRpc]
    void RpcBowArrowTake()
    {
        projectileModels[5].SetActive(true);
        projectileModels[6].SetActive(false);
        projectileModels[currentWeapon].SetActive(false);
    }

    [Command]
    void CmdBowArrowPlace()
    {
        RpcBowArrowPlace();
    }

    [ClientRpc]
    void RpcBowArrowPlace()
    {
        projectileModels[5].SetActive(false);
        projectileModels[currentWeapon].SetActive(true);
        projectileModels[6].SetActive(true);
    }

    [Command]
    void CmdJavelinTake()
    {
        RpcJavelinTake();
    }

    [ClientRpc]
    void RpcJavelinTake()
    {
        projectileModels[7].SetActive(false);
    }

    [Command]
    void CmdJavelinReset()
    {
        RpcJavelinReset();
    }

    [ClientRpc]
    void RpcJavelinReset()
    {
        projectileModels[7].SetActive(true);
    }

    [Command]
    void CmdTrapReset()
    {
        RpcJavelinReset();
    }

    [ClientRpc]
    void RpcTrapReset()
    {
        projectileModels[currentWeapon].SetActive(true);
    }

    [Command]
    void CmdArcherShoot(GameObject projPrefab)
    {
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
                proj.GetComponent<Trap>().isExplosive = false;
                Debug.Log("Potato");
                foreach (Transform child in proj.transform)
                {
                    if (child.gameObject.CompareTag("Gum"))
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                break;
            case 9:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * trapSpeed;
                proj.GetComponent<Trap>().isExplosive = true;
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
        Destroy(proj, 30);
        RpcArcherShoot();
    }

    void RpcArcherShoot()
    {
        projectileModels[currentWeapon].SetActive(false);
    }

    [Command]
    void CmdArcherSlashStart()
    {
        var wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        weapons[currentWeapon].GetComponent<Collider>().enabled = true;
        switch (currentWeapon)
        {
            case 4:
                wepon.damage = javelinSlashDamage;
                break;
            case 5:
                wepon.damage = handaxeSlashDamage;
                break;
            case 6:
                wepon.damage = shortswordSlashDamage;
                break;
            case 7:
                wepon.damage = daggerSlashDamage;
                break;
        }
        wepon.isSlash = true;
    }

    [Command]
    void CmdArcherSlashEnd()
    {
        weapons[currentWeapon].GetComponent<Collider>().enabled = false;
    }

    [Command]
    void CmdArcherStabStart()
    {
        var wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        weapons[currentWeapon].GetComponent<Collider>().enabled = true;
        switch (currentWeapon)
        {
            case 4:
                wepon.damage = javelinStabDamage;
                break;
            case 5:
                wepon.damage = handaxeStabDamage;
                break;
            case 6:
                wepon.damage = shortswordStabDamage;
                break;
            case 7:
                wepon.damage = daggerStabDamage;
                break;
        }
        wepon.isSlash = false;
    }

    [Command]
    void CmdArcherStabEnd()
    {
        weapons[currentWeapon].GetComponent<Collider>().enabled = false;
    }

    [Command]
    void CmdArcherOverheadStart()
    {
        var wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        weapons[currentWeapon].GetComponent<Collider>().enabled = true;
        switch (currentWeapon)
        {
            case 4:
                wepon.damage = javelinOverheadDamage;
                break;
            case 5:
                wepon.damage = handaxeOverheadDamage;
                break;
            case 6:
                wepon.damage = shortswordOverheadDamage;
                break;
            case 7:
                wepon.damage = daggerOverheadDamage;
                break;
        }
        wepon.isSlash = false;
    }

    [Command]
    void CmdArcherOverheadEnd()
    {
        weapons[currentWeapon].GetComponent<Collider>().enabled = false;
    }
}