using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoutControl : NetworkBehaviour
{

    [Header("To be chosen when starting the game")]
    [Range(0, 4)]
    public int primaryWeapon;
    [Range(5, 8)]
    public int secondaryWeapon;

    [Header("Arrays for weapon objects, their names, ammo, other models")]
    public GameObject[] weapons;
    public string[] weaponNames;
    public int[] maxAmmo;
    public GameObject[] projectileModels;

    [Header("BulletSpawn in camera, CharacterControl script, Health script")]
    public Transform bulletSpawn;
    public CharacterControl characterControl;
    public Health health;

    [Header("Melee weapon slash damage")]
    [Range(0, 200)]
    public int shortswordSlashDamage;
    [Range(0, 200)]
    public int daggerSlashDamage;
    [Range(0, 200)]
    public int lightMaceSlashDamage;
    [Range(0, 200)]
    public int shortswordSlashDamage2;
    [Range(0, 200)]
    public int quarterstaffSlashDamage;

    [Header("Melee weapon overhead swing damage")]
    [Range(0, 200)]
    public int shortswordOverheadDamage;
    [Range(0, 200)]
    public int daggerOverheadDamage;
    [Range(0, 200)]
    public int lightMaceOverheadDamage;
    [Range(0, 200)]
    public int shortswordOverheadDamage2;
    [Range(0, 200)]
    public int quarterstaffOverheadDamage;

    [Header("Melee weapon stab damage")]
    [Range(0, 200)]
    public int shortswordStabDamage;
    [Range(0, 200)]
    public int daggerStabDamage;
    [Range(0, 200)]
    public int lightMaceStabDamage;
    [Range(0, 200)]
    public int shortswordStabDamage2;
    [Range(0, 200)]
    public int quarterstaffStabDamage;

    [Header("Ranged attack projectile speeds")]
    [Range(0, 200)]
    public int flashBombSpeed;
    [Range(0, 200)]
    public int smokeBombSpeed;
    [Range(0, 200)]
    public int throwingDaggerSpeed;
    [Range(0, 200)]
    public int handCorssbowBoltSpeed;

    [Header("Ranged attack projectile damage")]
    [Range(0, 200)]
    public int throwingDaggerDamage;
    [Range(0, 200)]
    public int handCorssbowBoltDamage;

    [Header("Multipliers for movement speed")]
    [Range(0, 1)]
    public float flashBombThrowSpeed;
    [Range(0, 1)]
    public float smokeBombThrowSpeed;
    [Range(0, 1)]
    public float throwingDaggerThrowSpeed;
    [Range(0, 1)]
    public float handCorssbowBoltThrowSpeed;
    [Range(0, 1)]
    public float weaponSwapSpeedMultiplier;

    //[Header("Food settings")]
    //[Range(0, 60)]
    //public float energyPillDuration;
    //[Range(0, 10)]
    //public float energyPillSpeedMultiplier;
    //[Range(0, 200)]
    //public int peanutHealTotal;

    [Header("UI elements")]
    public Text ammoText;
    //public Slider energySlider;

    public GameObject playerModel;

    [SyncVar(hook = "ActivateWeapon")] private int currentWeapon;
    private NetworkAnimator anim;

    [SyncVar(hook = "SetAmmoText")]
    private int primaryWeaponAmmo;
    [SyncVar(hook = "SetAmmoText")]
    private int secondaryWeaponAmmo;

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
            anim.animator.ResetTrigger("Reload");
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
        {
            if (Input.GetButtonDown("Swap"))
            {
                CmdSwapWeapons();
            }
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

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
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

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
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

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
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
                }
                break;
            case 4:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
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
                        CmdBlockStart();
                    }
                }
                break;
            case 5:
                break;
            case 6:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }
                break;
            case 7:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }
                break;
            case 8:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }
                break;
            case 9:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
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
                weapons[10].GetComponent<MeleeWeapon>().collidersActive = true;
                weapons[10].GetComponent<MeleeWeapon>().damage = 0;
                weapons[10].tag = "Block";
                break;
            case 1:
                weapons[11].GetComponent<MeleeWeapon>().collidersActive = true;
                weapons[11].GetComponent<MeleeWeapon>().damage = 0;
                weapons[11].tag = "Block";
                break;
            case 2:
                weapons[12].GetComponent<MeleeWeapon>().collidersActive = true;
                weapons[12].GetComponent<MeleeWeapon>().damage = 0;
                weapons[12].tag = "Block";
                break;
            case 3:
                weapons[13].GetComponent<Collider>().enabled = true;
                break;
        }
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = true;
        weapons[currentWeapon].GetComponent<MeleeWeapon>().damage = 0;
        weapons[currentWeapon].tag = "Block";
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
                weapons[10].GetComponent<MeleeWeapon>().collidersActive = false;
                weapons[10].tag = "Weapon";
                break;
            case 1:
                weapons[11].GetComponent<MeleeWeapon>().collidersActive = false;
                weapons[11].tag = "Weapon";
                break;
            case 2:
                weapons[12].GetComponent<MeleeWeapon>().collidersActive = false;
                weapons[12].tag = "Weapon";
                break;
            case 3:
                weapons[13].GetComponent<Collider>().enabled = false;
                break;
        }
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        weapons[currentWeapon].tag = "Weapon";
    }

    void ScoutShoot(GameObject projPrefab)
    {
        if (currentWeapon < 5)
            primaryWeaponAmmo--;
        else secondaryWeaponAmmo--;
        CmdScoutShoot(projPrefab);
    }

    [Command]
    void CmdScoutShoot(GameObject projPrefab)
    {
        if (isServer)
            RpcScoutShoot();

        switch (currentWeapon)
        {
            case 6:
                bulletSpawn.localPosition = new Vector3(0.372f, 0.005f, 0.797f);
                break;
            case 7:
                bulletSpawn.localPosition = new Vector3(0.391f, 0.005f, 0.779f);
                break;
            case 8:
                bulletSpawn.localPosition = new Vector3(0.541f, -0.272f, 1.397f);
                break;
            case 9:
                bulletSpawn.localPosition = new Vector3(0.535f, -0.329f, 1.144f);
                break;
        }

        GameObject proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);

        switch (currentWeapon)
        {
            case 6:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * flashBombSpeed;
                break;
            case 7:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * smokeBombSpeed;
                break;
            case 8:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * throwingDaggerSpeed;
                proj.GetComponent<Projectile>().damage = throwingDaggerDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                break;
            case 9:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * handCorssbowBoltSpeed;
                proj.GetComponent<Projectile>().damage = throwingDaggerSpeed;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                break;
        }
        NetworkServer.Spawn(proj);
        Destroy(proj, 30);
    }

    [ClientRpc]
    void RpcScoutShoot()
    {
        projectileModels[currentWeapon].SetActive(false);
    }

    void BerserkerSlashStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.collidersActive = true;
        MeleeWeapon wepon2;
        switch (currentWeapon)
        {
            case 0:
                wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                wepon.damage = shortswordSlashDamage;
                wepon2.damage = shortswordSlashDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 1:
                wepon2 = weapons[11].GetComponent<MeleeWeapon>();
                wepon.damage = daggerSlashDamage;
                wepon2.damage = daggerSlashDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 2:
                wepon2 = weapons[12].GetComponent<MeleeWeapon>();
                wepon.damage = lightMaceSlashDamage;
                wepon2.damage = lightMaceSlashDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 3:
                wepon.damage = shortswordSlashDamage2;
                break;
            case 4:
                wepon.damage = quarterstaffSlashDamage;
                break;
        }
        wepon.isSlash = true;
    }


    void BerserkerSlashEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        switch (currentWeapon)
        {
            case 0:
                weapons[10].GetComponent<MeleeWeapon>().collidersActive = false; ;
                break;
            case 1:
                weapons[11].GetComponent<MeleeWeapon>().collidersActive = false;
                break;
            case 2:
                weapons[12].GetComponent<MeleeWeapon>().collidersActive = false;
                break;
        }
    }


    void BerserkerStabStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.collidersActive = true;
        MeleeWeapon wepon2;
        switch (currentWeapon)
        {
            case 0:
                wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                wepon.damage = shortswordStabDamage;
                wepon2.damage = shortswordStabDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 1:
                wepon2 = weapons[11].GetComponent<MeleeWeapon>();
                wepon.damage = daggerStabDamage;
                wepon2.damage = daggerStabDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 2:
                wepon2 = weapons[12].GetComponent<MeleeWeapon>();
                wepon.damage = lightMaceStabDamage;
                wepon2.damage = lightMaceStabDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 3:
                wepon.damage = shortswordStabDamage2;
                break;
            case 4:
                wepon.damage = quarterstaffStabDamage;
                break;
        }
        wepon.isSlash = false;
    }


    void BerserkerStabEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        switch (currentWeapon)
        {
            case 0:
                weapons[10].GetComponent<MeleeWeapon>().collidersActive = false; ;
                break;
            case 1:
                weapons[11].GetComponent<MeleeWeapon>().collidersActive = false;
                break;
            case 2:
                weapons[12].GetComponent<MeleeWeapon>().collidersActive = false;
                break;
        }
    }


    void BerserkerOverheadStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.collidersActive = true;
        MeleeWeapon wepon2;
        switch (currentWeapon)
        {
            case 0:
                wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                wepon.damage = shortswordOverheadDamage;
                wepon2.damage = shortswordOverheadDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 1:
                wepon2 = weapons[11].GetComponent<MeleeWeapon>();
                wepon.damage = daggerOverheadDamage;
                wepon2.damage = daggerOverheadDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 2:
                wepon2 = weapons[12].GetComponent<MeleeWeapon>();
                wepon.damage = lightMaceOverheadDamage;
                wepon2.damage = lightMaceOverheadDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 3:
                wepon.damage = shortswordOverheadDamage2;
                break;
            case 4:
                wepon.damage = quarterstaffOverheadDamage;
                break;
        }
        wepon.isSlash = false;
    }


    void BerserkerOverheadEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        switch (currentWeapon)
        {
            case 0:
                weapons[10].GetComponent<MeleeWeapon>().collidersActive = false; ;
                break;
            case 1:
                weapons[11].GetComponent<MeleeWeapon>().collidersActive = false;
                break;
            case 2:
                weapons[12].GetComponent<MeleeWeapon>().collidersActive = false;
                break;
        }
    }

    void SetAmmoText(int value)
    {
        if (isLocalPlayer)
        {
            if (currentWeapon == primaryWeapon)
            {
                ammoText.text = String.Format("{0}/{1}", value, maxAmmo[primaryWeapon]);
            }
            else
            {
                ammoText.text = String.Format("{0}/{1}", value, maxAmmo[secondaryWeapon]);
            }
        }
    }
}