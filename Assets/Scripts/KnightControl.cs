using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class KnightControl : NetworkBehaviour
{

    [Header("To be chosen when starting the game")]
    [Range(0, 4)]
    public int primaryWeapon;
    [Range(5, 9)]
    public int secondaryWeapon;

    [Header("Arrays for weapon objects, their names, ammo, other models")]
    public GameObject[] weapons;
    public string[] weaponNames;
    public int[] maxAmmo;

    [Header("BulletSpawn in camera and CharacterControl script")]
    public Transform bulletSpawn;
    public CharacterControl characterControl;

    [Header("Melee weapon slash damage")]
    [Range(0, 200)]
    public int hammerSlashDamage;
    [Range(0, 200)]
    public int swordSlashDamage;
    [Range(0, 200)]
    public int axeSlashDamage;
    [Range(0, 200)]
    public int halberdSlashDamage;
    [Range(0, 200)]
    public int spearSlashDamage;
    [Range(0, 200)]
    public int bannerSlashDamage;


    [Header("Melee weapon overhead swing damage")]
    [Range(0, 200)]
    public int hammerOverheadDamage;
    [Range(0, 200)]
    public int swordOverheadDamage;
    [Range(0, 200)]
    public int axeOverheadDamage;
    [Range(0, 200)]
    public int halberdOverheadDamage;
    [Range(0, 200)]
    public int spearOverheadDamage;
    [Range(0, 200)]
    public int bannerOverheadDamage;

    [Header("Melee weapon stab damage")]
    [Range(0, 200)]
    public int hammerStabDamage;
    [Range(0, 200)]
    public int swordStabDamage;
    [Range(0, 200)]
    public int axeStabDamage;
    [Range(0, 200)]
    public int halberdStabDamage;
    [Range(0, 200)]
    public int spearStabDamage;
    [Range(0, 200)]
    public int bannerStabDamage;
    [Range(0, 200)]
    public int shieldStabDamage;

    [Header("Ranged attack projectile speeds")]
    [Range(0, 200)]
    public int bombSpeed;
    [Range(0, 200)]
    public int throwingDiskSpeed;
    [Range(0, 200)]
    public int ammoBoxSpeed;

    [Header("Ranged attack projectile damage")]
    [Range(0, 200)]
    public int cherryBombDamage;
    [Range(0, 200)]
    public int throwingDiskDamage;

    [Header("Multipliers for movement speed")]
    [Range(0, 1)]
    public float cherryBombSpeedMultiplier;
    [Range(0, 1)]
    public float throwingDiskSpeedMultiplier;
    [Range(0, 1)]
    public float matchboxSpeedMultiplier;
    [Range(0, 1)]
    public float weaponSwapSpeedMultiplier;

    [Header("UI elements")]
    public Text ammoText;

    public GameObject bombFuse;

    [SyncVar(hook = "ActivateWeapon")] private int currentWeapon;
    private NetworkAnimator anim;

    [HideInInspector]
    [SyncVar(hook = "SetAmmoText")]
    public int primaryWeaponAmmo;
    [HideInInspector]
    [SyncVar(hook = "SetAmmoText")]
    public int secondaryWeaponAmmo;

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
        primaryWeaponAmmo = maxAmmo[primaryWeapon];
        secondaryWeaponAmmo = maxAmmo[secondaryWeapon];
        currentWeapon = primaryWeapon;
        ActivateWeapon(currentWeapon);
        if (isLocalPlayer)
            CmdActivateWeapon(currentWeapon);
    }

    void OnEnable()
    {
        primaryWeaponAmmo = maxAmmo[primaryWeapon];
        secondaryWeaponAmmo = maxAmmo[secondaryWeapon];
        currentWeapon = primaryWeapon;
        ActivateWeapon(currentWeapon);
        CmdActivateWeapon(currentWeapon);
        foreach (AnimatorControllerParameter parameter in anim.animator.parameters)
        {
            anim.animator.SetBool(parameter.name, false);
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        characterControl.moveSpeed = characterControl.defaultMoveSpeed;

        if (NetworkServer.active)
        {
            anim.animator.ResetTrigger("Attack2");
            anim.animator.ResetTrigger("Attack3");
            anim.animator.ResetTrigger("Parry");
            anim.animator.ResetTrigger("Stop");
            anim.animator.ResetTrigger("Hurt");
            anim.animator.ResetTrigger("Block hurt");
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
        {
            if (Input.GetButtonDown("Swap") && (secondaryWeaponAmmo > 0 || maxAmmo[secondaryWeapon] == 0))
            {
                if (currentWeapon < 5)
                {
                    anim.animator.SetBool("OutOfSecondary", false);
                }
                CmdSwapWeapons();
            }
        }

        switch (currentWeapon)
        {
            case 0:
                // ataka vyksta kol laikomas nuspaustas mygtukas
                if (Input.GetButton("Fire1"))
                {
                    anim.animator.SetBool("Attack1", true);
                }
                else
                {
                    anim.animator.SetBool("Attack1", false);
                }

                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
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
                }

                // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                if (Input.GetButton("Block"))
                {
                    anim.animator.SetBool("Block", true);
                    CmdBlockStart();
                }
                else
                {
                    anim.animator.SetBool("Block", false);
                    CmdBlockEnd();
                }
                break;
            case 1:
                // ataka vyksta kol laikomas nuspaustas mygtukas
                if (Input.GetButton("Fire1"))
                {
                    anim.animator.SetBool("Attack1", true);
                }
                else
                {
                    anim.animator.SetBool("Attack1", false);
                }

                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
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
                }

                // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                if (Input.GetButton("Block"))
                {
                    anim.animator.SetBool("Block", true);
                    CmdBlockStart();
                }
                else
                {
                    anim.animator.SetBool("Block", false);
                    CmdBlockEnd();
                }
                break;
            case 2:
                // ataka vyksta kol laikomas nuspaustas mygtukas
                if (Input.GetButton("Fire1"))
                {
                    anim.animator.SetBool("Attack1", true);
                }
                else
                {
                    anim.animator.SetBool("Attack1", false);
                }

                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
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
                }

                // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                if (Input.GetButton("Block"))
                {
                    anim.animator.SetBool("Block", true);
                    CmdBlockStart();
                }
                else
                {
                    anim.animator.SetBool("Block", false);
                    CmdBlockEnd();
                }
                break;
            case 3:
                // ataka vyksta kol laikomas nuspaustas mygtukas
                if (Input.GetButton("Fire1"))
                {
                    anim.animator.SetBool("Attack1", true);
                }
                else
                {
                    anim.animator.SetBool("Attack1", false);
                }

                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
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

                    // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                    if (Input.GetButton("Block"))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 4:
                // ataka vyksta kol laikomas nuspaustas mygtukas
                if (Input.GetButton("Fire1"))
                {
                    anim.animator.SetBool("Attack1", true);
                }
                else
                {
                    anim.animator.SetBool("Attack1", false);
                }

                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
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

                    // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                    if (Input.GetButton("Block"))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 5:
                // ataka vyksta kol laikomas nuspaustas mygtukas
                if (Input.GetButton("Fire1"))
                {
                    anim.animator.SetBool("Attack1", true);
                }
                else
                {
                    anim.animator.SetBool("Attack1", false);
                }

                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
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

                    // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                    if (Input.GetButton("Block"))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 6:
                if (secondaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            if (secondaryWeaponAmmo == 1)
                            {
                                anim.animator.SetBool("OutOfSecondary", true);
                            }
                            anim.SetTrigger("Attack3");
                        }
                    }
                }
                else
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        CmdSwapWeapons();
                    }
                }
                break;
            case 7:
                if (secondaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            if (secondaryWeaponAmmo == 1)
                            {
                                anim.animator.SetBool("OutOfSecondary", true);
                            }
                            anim.SetTrigger("Attack3");
                        }
                    }
                }
                else
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        CmdSwapWeapons();
                    }
                }
                break;
            case 8:
                if (secondaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            if (secondaryWeaponAmmo == 1)
                            {
                                anim.animator.SetBool("OutOfSecondary", true);
                            }
                            anim.SetTrigger("Attack3");
                        }
                    }
                }
                else
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        CmdSwapWeapons();
                    }
                }
                break;
            case 9:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }

                if (Input.GetButton("Block"))
                {
                    anim.animator.SetBool("Block", true);
                    CmdBlockStart();
                }
                else
                {
                    anim.animator.SetBool("Block", false);
                    CmdBlockEnd();
                }
                break;
        }
    }

    public void ActivateWeapon(int value)
    {
        anim = GetComponent<NetworkAnimator>();
        currentWeapon = value;
        if (currentWeapon == secondaryWeapon)
        {
            if (isLocalPlayer)
            {
                if (maxAmmo[secondaryWeapon] > 0)
                {
                    ammoText.text = String.Format("{0}/{1}", secondaryWeaponAmmo, maxAmmo[secondaryWeapon]);
                }
                else
                {
                    ammoText.text = "";
                }
            }
            if (primaryWeapon <= 2)
            {
                weapons[10].SetActive(false);
            }
            if (secondaryWeapon == 6)
            {
                weapons[11].SetActive(true);
            }
            weapons[primaryWeapon].SetActive(false);
            weapons[secondaryWeapon].SetActive(true);
            anim.animator.SetBool(weaponNames[primaryWeapon], false);
            anim.animator.SetBool(weaponNames[secondaryWeapon], true);
        }
        else
        {
            if (isLocalPlayer)
            {
                if (maxAmmo[primaryWeapon] > 0)
                {
                    ammoText.text = String.Format("{0}/{1}", primaryWeaponAmmo, maxAmmo[primaryWeapon]);
                }
                else
                {
                    ammoText.text = "";
                }
            }
            if (primaryWeapon <= 2)
            {
                weapons[10].SetActive(true);
            }
            if (secondaryWeapon == 6)
            {
                weapons[11].SetActive(false);
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
    void CmdBlockStart()
    {
        RpcBlockStart();
    }

    [ClientRpc]
    void RpcBlockStart()
    {
        switch (currentWeapon)
        {
            case 0:
                weapons[12].GetComponent<Collider>().enabled = true;
                break;
            case 1:
                weapons[12].GetComponent<Collider>().enabled = true;
                break;
            case 2:
                weapons[12].GetComponent<Collider>().enabled = true;
                break;
            case 9:
                weapons[9].GetComponent<Collider>().enabled = true;
                break;
        }
        if (currentWeapon > 2 && currentWeapon != 9)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = true;
            weapons[currentWeapon].GetComponent<MeleeWeapon>().damage = 0;
            weapons[currentWeapon].GetComponent<Collider>().isTrigger = false;
            weapons[currentWeapon].tag = "Block";
        }
    }

    [Command]
    void CmdBlockEnd()
    {
        RpcBlockEnd();
    }

    [ClientRpc]
    void RpcBlockEnd()
    {
        switch (currentWeapon)
        {
            case 0:
                weapons[10].GetComponent<Collider>().enabled = false;
                break;
            case 1:
                weapons[10].GetComponent<Collider>().enabled = false;
                break;
            case 2:
                weapons[10].GetComponent<Collider>().enabled = false;
                break;
            case 9:
                weapons[9].GetComponent<Collider>().enabled = false;
                break;
        }
        if (currentWeapon > 2 && currentWeapon != 9)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[currentWeapon].tag = "Weapon";
            weapons[currentWeapon].GetComponent<Collider>().isTrigger = true;
        }
    }

    void KnightShoot(GameObject projPrefab)
    {
        secondaryWeaponAmmo--;
        CmdKnightShoot(projPrefab);
    }

    [Command]
    void CmdKnightShoot(GameObject projPrefab)
    {
        if (isServer)
            RpcKnightShoot();

        switch (currentWeapon)
        {
            case 6:
                bulletSpawn.localPosition = new Vector3(0.577f, -0.958f, 0.748f);
                break;
            case 7:
                bulletSpawn.localPosition = new Vector3(0.577f, -0.958f, 0.748f);
                break;
            case 8:
                bulletSpawn.localPosition = new Vector3(0.256f, -0.238f, 1.065f);
                break;
        }

        GameObject proj = null;

        switch (currentWeapon)
        {
            case 6:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * bombSpeed;
                proj.GetComponent<Bomb>().spawnedBy = netId;
                proj.GetComponent<Bomb>().bombType = 2;
                break;
            case 7:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * throwingDiskSpeed;
                proj.GetComponent<Projectile>().damage = throwingDiskDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                break;
            case 8:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * ammoBoxSpeed;
                proj.GetComponent<AmmoBox>().spawnedBy = netId;
                break;
        }
        NetworkServer.Spawn(proj);
        Destroy(proj, 30);
    }

    [ClientRpc]
    void RpcKnightShoot()
    {
        weapons[currentWeapon].SetActive(false);
        if (currentWeapon == 6)
        {
            bombFuse.SetActive(false);
        }
    }

    void KnightSlashStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.collidersActive = true;
        switch (currentWeapon)
        {
            case 0:
                wepon.damage = hammerSlashDamage;
                break;
            case 1:
                wepon.damage = swordSlashDamage;
                break;
            case 2:
                wepon.damage = axeSlashDamage;
                break;
            case 3:
                wepon.damage = halberdSlashDamage;
                break;
            case 4:
                wepon.damage = spearSlashDamage;
                break;
            case 5:
                wepon.damage = bannerSlashDamage;
                break;
        }
        wepon.isSlash = true;
    }


    void KnightSlashEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
    }


    void KnightStabStart()
    {
        MeleeWeapon wepon = null;
        if (currentWeapon != 9)
        {
            wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
            wepon.collidersActive = true;
            wepon.isSlash = false;
        }
        switch (currentWeapon)
        {
            case 0:
                wepon.damage = hammerStabDamage;
                break;
            case 1:
                wepon.damage = swordStabDamage;
                break;
            case 2:
                wepon.damage = axeStabDamage;
                break;
            case 3:
                wepon.damage = halberdStabDamage;
                break;
            case 4:
                wepon.damage = spearStabDamage;
                break;
            case 5:
                wepon.damage = bannerStabDamage;
                break;
            case 9:
                wepon = weapons[13].GetComponent<MeleeWeapon>();
                wepon.collidersActive = true;
                wepon.isSlash = false;
                wepon.damage = shieldStabDamage;
                weapons[9].GetComponent<Collider>().enabled = true;
                break;
        }
    }


    void KnightStabEnd()
    {
        if (currentWeapon != 9)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        } else
        {
            weapons[13].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[9].GetComponent<Collider>().enabled = false;
        }

    }


    void KnightOverheadStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.collidersActive = true;
        switch (currentWeapon)
        {
            case 0:
                wepon.damage = hammerOverheadDamage;
                break;
            case 1:
                wepon.damage = swordOverheadDamage;
                break;
            case 2:
                wepon.damage = axeOverheadDamage;
                break;
            case 3:
                wepon.damage = halberdOverheadDamage;
                break;
            case 4:
                wepon.damage = spearOverheadDamage;
                break;
            case 5:
                wepon.damage = bannerOverheadDamage;
                break;
        }
        wepon.isSlash = false;
    }


    void KnightOverheadEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
    }

    void SetAmmoText(int value)
    {
        if (isLocalPlayer)
        {
            if (currentWeapon == primaryWeapon && maxAmmo[primaryWeapon] > 0)
            {
                ammoText.text = String.Format("{0}/{1}", value, maxAmmo[primaryWeapon]);
            }
            else if (currentWeapon == secondaryWeapon && maxAmmo[secondaryWeapon] > 0)
            {
                ammoText.text = String.Format("{0}/{1}", value, maxAmmo[secondaryWeapon]);
            }
        }
    }

    [Command]
    void CmdKnightRedraw()
    {
        if (isServer)
        {
            RpcKnightRedraw();
        }
    }

    [ClientRpc]
    void RpcKnightRedraw()
    {
        weapons[currentWeapon].SetActive(true);
    }

    [Command]
    void CmdFuseLight()
    {
        if (isServer)
        {
            RpcFuseLight();
        }
    }

    [ClientRpc]
    void RpcFuseLight()
    {
        bombFuse.SetActive(true);
    }
}