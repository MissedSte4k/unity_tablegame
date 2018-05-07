using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BerserkerControl : NetworkBehaviour
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
    public int greatswordSlashDamage;
    [Range(0, 200)]
    public int greataxeSlashDamage;
    [Range(0, 200)]
    public int greathammerSlashDamage;
    [Range(0, 200)]
    public int greatclubSlashDamage;
    [Range(0, 200)]
    public int axeSlashDamage;
    [Range(0, 200)]
    public int fistSlashDamage;

    [Header("Melee weapon overhead swing damage")]
    [Range(0, 200)]
    public int greatswordOverheadDamage;
    [Range(0, 200)]
    public int greataxeOverheadDamage;
    [Range(0, 200)]
    public int greathammerOverheadDamage;
    [Range(0, 200)]
    public int greatclubOverheadDamage;
    [Range(0, 200)]
    public int axeOverheadDamage;
    [Range(0, 200)]
    public int fistOverheadDamage;

    [Header("Melee weapon stab damage")]
    [Range(0, 200)]
    public int greatswordStabDamage;
    [Range(0, 200)]
    public int greataxeStabDamage;
    [Range(0, 200)]
    public int greathammerStabDamage;
    [Range(0, 200)]
    public int greatclubStabDamage;
    [Range(0, 200)]
    public int axeStabDamage;
    [Range(0, 200)]
    public int fistStabDamage;

    [Header("Ranged attack projectile speeds")]
    [Range(0, 200)]
    public int boulderSpeed;
    [Range(0, 200)]
    public int axeThrowSpeed;

    [Header("Ranged attack projectile damage")]
    [Range(0, 200)]
    public int boulderDamage;
    [Range(0, 200)]
    public int axeThrowDamage;

    [Header("Multipliers for movement speed")]
    [Range(0, 1)]
    public float eatSpeedMultiplier;
    [Range(0, 1)]
    public float boulderThrowSpeedMultiplier;
    [Range(0, 1)]
    public float weaponSwapSpeedMultiplier;

    [Header("Food settings")]
    [Range(0, 60)]
    public float energyPillDuration;
    [Range(0, 10)]
    public float energyPillSpeedMultiplier;
    [Range(0, 200)]
    public int peanutHealTotal;

    [Header("UI elements")]
    public Text ammoText;
    public Slider energySlider;

    public AudioSource audioSource;

    [SyncVar(hook = "ActivateWeapon")] private int currentWeapon;
    [SyncVar]
    private bool isThrow1 = false;
    private NetworkAnimator anim;

    [HideInInspector]
    [SyncVar(hook = "SetAmmoText")]
    public int primaryWeaponAmmo;
    [HideInInspector]
    [SyncVar(hook = "SetAmmoText")]
    public int secondaryWeaponAmmo;

    private bool isCaffeinated = false;
    private float remainingDuration;

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
            anim.animator.ResetTrigger("Throw1");
            anim.animator.ResetTrigger("Throw2");
            anim.animator.ResetTrigger("Parry");
        }

        if (isCaffeinated && !anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Eat"))
        {
            if (remainingDuration < 0)
            {
                audioSource.enabled = false;
                isCaffeinated = false;
            }
            else if (remainingDuration >= 0)
            {
                energySlider.value = remainingDuration / energyPillDuration;
                remainingDuration -= Time.deltaTime;
            }
            characterControl.moveSpeed *= energyPillSpeedMultiplier;
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
        {
            if (Input.GetButtonDown("Swap") && currentWeapon != 4 && secondaryWeaponAmmo > 0)
            {
                if (currentWeapon < 5)
                {
                    anim.animator.SetBool("OutOfSecondary", false);
                }
                CmdSwapWeapons();
            }
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Draw"))
        {
            characterControl.moveSpeed *= weaponSwapSpeedMultiplier;
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Eat"))
        {
            characterControl.moveSpeed *= eatSpeedMultiplier;
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

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 4:
                if (primaryWeaponAmmo > 0)
                {
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

                        if (Input.GetButtonDown("Fire4"))
                        {
                            if (primaryWeaponAmmo <= 2)
                            {
                                anim.animator.SetBool("Empty", true);
                            }
                            if (isThrow1)
                            {
                                anim.SetTrigger("Throw1");
                                isThrow1 = false;
                            }
                            else
                            {
                                anim.SetTrigger("Throw2");
                                isThrow1 = true;
                            }
                        }

                        if (Input.GetButtonDown("Block"))
                        {
                            anim.SetTrigger("Parry");
                            CmdBlockStart();
                        }
                    }
                }
                break;
            case 5:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed *= boulderThrowSpeedMultiplier;
                }
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (secondaryWeaponAmmo > 0)
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
                    else
                    {
                        CmdSwapWeapons();
                    }
                }
                break;
            case 6:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (secondaryWeaponAmmo > 0)
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
                    else
                    {
                        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                        {
                            CmdSwapWeapons();
                        }
                    }
                }
                break;
            case 7:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (secondaryWeaponAmmo > 0)
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
                    else
                    {
                        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                        {
                            CmdSwapWeapons();
                        }
                    }
                }
                break;
            case 8:
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
        }
    }


    public void ActivateWeapon(int value)
    {
        anim = GetComponent<NetworkAnimator>();
        currentWeapon = value;
        if (isLocalPlayer && secondaryWeapon == 6)
        {
            energySlider.gameObject.SetActive(true);
        }
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
            if (primaryWeapon == 4)
            {
                weapons[9].SetActive(false);
            }
            if (secondaryWeapon == 8)
            {
                weapons[10].SetActive(true);
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
            if (primaryWeapon == 4)
            {
                weapons[9].SetActive(true);
            }
            if (secondaryWeapon == 8)
            {
                weapons[10].SetActive(false);
            }
            weapons[primaryWeapon].SetActive(true);
            weapons[secondaryWeapon].SetActive(false);
            anim.animator.SetBool(weaponNames[secondaryWeapon], false);
            anim.animator.SetBool(weaponNames[primaryWeapon], true);
            if (primaryWeaponAmmo == 0)
            {
                projectileModels[0].SetActive(false);
                projectileModels[1].SetActive(false);
            } if (primaryWeaponAmmo == 1)
            {
                if (isThrow1)
                    projectileModels[0].SetActive(false);
                else projectileModels[1].SetActive(false);
            }
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
        ActivateWeapon(currentWeapon);
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
        if (currentWeapon == 4)
        {
            weapons[9].GetComponent<MeleeWeapon>().collidersActive = true;
            weapons[9].GetComponent<MeleeWeapon>().damage = 0;
            weapons[9].tag = "Block";
        }
        else if (currentWeapon == 8)
        {
            weapons[10].GetComponent<MeleeWeapon>().collidersActive = true;
            weapons[10].GetComponent<MeleeWeapon>().damage = 0;
            weapons[10].tag = "Block";
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
        if (currentWeapon == 4)
        {
            weapons[9].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[9].tag = "Weapon";
        }
        else if (currentWeapon == 8)
        {
            weapons[10].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[10].tag = "Weapon";
        }
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        weapons[currentWeapon].tag = "Weapon";
    }

    void BerserkerShoot(GameObject projPrefab)
    {
        if (currentWeapon < 5)
            primaryWeaponAmmo--;
        else secondaryWeaponAmmo--;
        CmdBerserkerShoot(projPrefab, isThrow1);
    }

    [Command]
    void CmdBerserkerShoot(GameObject projPrefab, bool hand)
    {
        if (isServer)
            RpcBerserkerShoot();

        switch (currentWeapon)
        {
            case 4:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw 1"))
                {
                    bulletSpawn.localPosition = new Vector3(0.737f, -0.122f, 1.315f);
               } else
                {
                    bulletSpawn.localPosition = new Vector3(-0.454f, -0.116f, 1.593f);
                }
                break;
            case 5:
                bulletSpawn.localPosition = new Vector3(0.799f, 0.137f, 0.852f);
                break;
        }

        GameObject proj = null;

        switch (currentWeapon)
        {
            case 4:

                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation * Quaternion.Euler(55, 0, 0));
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.forward * axeThrowSpeed;
                proj.GetComponent<Projectile>().damage = axeThrowDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                break;
            case 5:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.forward * boulderSpeed;
                proj.GetComponent<Projectile>().damage = boulderDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                break;
        }
        NetworkServer.Spawn(proj);
        Destroy(proj, 30);
    }

    [ClientRpc]
    void RpcBerserkerShoot()
    {
        if (currentWeapon == 5)
        {
            projectileModels[2].SetActive(false);
        } else
        {
            if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw 1"))
                projectileModels[0].SetActive(false);
            else
                projectileModels[1].SetActive(false);
        }
    }

    [Command]
    void CmdBerserkerRedraw()
    {
        RpcBerserkerRedraw();
    }

    [ClientRpc]
    void RpcBerserkerRedraw()
    {
        if (currentWeapon == 4)
        {
            if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw 1"))
                projectileModels[0].SetActive(true);
            else
                projectileModels[1].SetActive(true);
        }
        else
        {
            weapons[currentWeapon].SetActive(true);
        }
    }

    void BerserkerSlashStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.collidersActive = true;
        MeleeWeapon wepon2;
        switch (currentWeapon)
        {
            case 0:
                wepon.damage = greatswordSlashDamage;
                break;
            case 1:
                wepon.damage = greataxeSlashDamage;
                break;
            case 2:
                wepon.damage = greathammerSlashDamage;
                break;
            case 3:
                wepon.damage = greatclubSlashDamage;
                break;
            case 4:
                wepon2 = weapons[9].GetComponent<MeleeWeapon>();
                wepon.damage = axeSlashDamage;
                wepon2.damage = axeSlashDamage;
                wepon2.isSlash = true;
                wepon2.collidersActive = true;
                break;
            case 8:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Fists_punch_1")){
                    wepon.damage = fistSlashDamage;
                    wepon.isSlash = false;
                } else
                {
                    wepon.collidersActive = false;
                    wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                    wepon2.damage = fistSlashDamage;
                    wepon2.isSlash = false;
                    wepon2.collidersActive = true;
                }
                break;
        }
        if (currentWeapon != 8)
        {
            wepon.isSlash = true;
        }
    }


    void BerserkerSlashEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        if (currentWeapon == 4)
        {
            weapons[9].GetComponent<MeleeWeapon>().collidersActive = false;
        }
        else if (currentWeapon == 8)
        {
            weapons[10].GetComponent<MeleeWeapon>().collidersActive = false;
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
                wepon.damage = greatswordStabDamage;
                break;
            case 1:
                wepon.damage = greataxeStabDamage;
                break;
            case 2:
                wepon.damage = greathammerStabDamage;
                break;
            case 3:
                wepon.damage = greatclubStabDamage;
                break;
            case 4:
                wepon2 = weapons[9].GetComponent<MeleeWeapon>();
                weapons[9].GetComponent<Collider>().enabled = true;
                wepon.damage = axeStabDamage;
                wepon2.damage = axeStabDamage;
                wepon2.isSlash = false;
                wepon2.collidersActive = true;
                break;
            case 8:
                wepon.damage = fistStabDamage;
                break;
        }
        wepon.isSlash = false;
    }


    void BerserkerStabEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        if (currentWeapon == 4)
        {
            weapons[9].GetComponent<MeleeWeapon>().collidersActive = false;
        }
        else if (currentWeapon == 8)
        {
            weapons[10].GetComponent<MeleeWeapon>().collidersActive = false;
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
                wepon.damage = greatswordOverheadDamage;
                break;
            case 1:
                wepon.damage = greataxeOverheadDamage;
                break;
            case 2:
                wepon.damage = greathammerOverheadDamage;
                break;
            case 3:
                wepon.damage = greatclubOverheadDamage;
                break;
            case 4:
                wepon2 = weapons[9].GetComponent<MeleeWeapon>();
                weapons[9].GetComponent<Collider>().enabled = true;
                wepon.damage = axeOverheadDamage;
                wepon2.damage = axeOverheadDamage;
                wepon2.isSlash = false;
                wepon2.collidersActive = true;
                break;
            case 8:
                wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                weapons[10].GetComponent<Collider>().enabled = true;
                wepon.damage = fistOverheadDamage;
                wepon2.damage = fistOverheadDamage;
                wepon2.isSlash = false;
                wepon2.collidersActive = true;
                break;
        }
        wepon.isSlash = false;
    }


    void BerserkerOverheadEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        if (currentWeapon == 4)
        {
            weapons[9].GetComponent<MeleeWeapon>().collidersActive = false;
        }
        else if (currentWeapon == 8)
        {
            weapons[10].GetComponent<MeleeWeapon>().collidersActive = false;
        }
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

    //called twice per meal
    void FoodEat(int instance)
    {
        if (isLocalPlayer)
        {
            switch (currentWeapon)
            {
                case 6:
                    if (instance == 1)
                    {
                        isCaffeinated = true;
                        remainingDuration = energyPillDuration;
                        secondaryWeaponAmmo--;
                        CmdDeactivateFood();
                        audioSource.enabled = true;
                    }
                    energySlider.value += 0.5f;
                    break;
                case 7:
                    if (instance == 1)
                    {
                        secondaryWeaponAmmo--;
                        CmdDeactivateFood();
                    }
                    health.Heal((int)peanutHealTotal / 2);
                    break;
            }
        }
    }
    //Farmed Locally, Very Locally

    [Command]
    void CmdDeactivateFood()
    {
        RpcDeactivateFood();
    }

    [ClientRpc]
    void RpcDeactivateFood()
    {
        weapons[currentWeapon].SetActive(false);
    }
}