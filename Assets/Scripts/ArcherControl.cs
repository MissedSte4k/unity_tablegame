using System.Collections;
using System.Collections.Generic;
using System;
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
    public CharacterControl characterControl;

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

    public int[] maxAmmo;

    [SyncVar(hook = "SetAmmoText")]
    private int primaryWeaponAmmo;
    [SyncVar(hook = "SetAmmoText")]
    private int secondaryWeaponAmmo;
    [SyncVar]
    bool isShooting = false;

    public float shortbowShootSpeedReduction;
    public float longbowShootSpeedReduction;
    public float lightCrossbowShootSpeedReduction;
    public float heavyCrossbowShootSpeedReduction;
    public float javelinThrowSpeedReduction;
    public float trapThrowSpeedReduction;
    public float trapReloadSpeedReduction;
    public float weaponSwapSpeedReduction;

    private float defaultMoveSpeed = 5;

    public float matchLightDuration;
    private float remainingDuration;
    [SyncVar]
    private bool isLitFam = false;
    public float litDamageMultiplier;
    public GameObject fire;
    public GameObject sparks;
    public Text ammoText;
    public Slider matchSlider;
    public GameObject fireSprite;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        defaultMoveSpeed = characterControl.moveSpeed;

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

        characterControl.moveSpeed = defaultMoveSpeed;


        if (NetworkServer.active)
        {
            anim.animator.ResetTrigger("Attack2");
            anim.animator.ResetTrigger("Attack3");
            anim.animator.ResetTrigger("Parry");
            anim.animator.ResetTrigger("Reload");
            anim.animator.ResetTrigger("Light");
            anim.animator.ResetTrigger("Throw");
            anim.animator.ResetTrigger("Stahp");
        }

        if (isLitFam)
        {
            if (remainingDuration < 0 || isShooting)
            {
                Debug.Log(remainingDuration);
                isLitFam = false;
                FireEnd();
            }
            else if (remainingDuration >= 0)
            {
                remainingDuration -= Time.deltaTime;
                matchSlider.value = remainingDuration / matchLightDuration;
            }
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
        {
            if (!isLitFam && primaryWeaponAmmo > 1)
            {
                if (Input.GetButtonDown("Swap") && anim.animator.GetBool("OutOfTraps") == false && currentWeapon != 4)
                {
                    CmdSwapWeapons();
                    if ((currentWeapon == 2 || currentWeapon == 3) && primaryWeaponAmmo > 0)
                    {
                        anim.animator.SetBool("Empty", false);
                    }
                }
                matchSlider.value = 1;
            }
            CmdNotShooting();
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Draw"))
        {
            characterControl.moveSpeed = characterControl.moveSpeed - weaponSwapSpeedReduction;
        }

        switch (currentWeapon)
        {
            case 0:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_draw") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_idle") ||
                    anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_shoot") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_attack_ammo") ||
                    anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_attack_empty"))
                {
                    characterControl.moveSpeed = characterControl.moveSpeed - longbowShootSpeedReduction;
                }
                if (primaryWeaponAmmo > 0)
                {
                    if (Input.GetButton("Fire1"))
                    {
                        anim.animator.SetBool("Attack1", true);
                        if (primaryWeaponAmmo == 1)
                        {
                            anim.animator.SetBool("Empty", true);
                        }
                    }
                    else
                    {
                        anim.animator.SetBool("Attack1", false);
                    }

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Stahp");
                    }
                }
                else anim.animator.SetBool("Empty", true);
                break;
            case 1:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_draw") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_idle") ||
                    anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_shoot") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_attack_ammo") ||
                    anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_attack_empty"))
                {
                    characterControl.moveSpeed = characterControl.moveSpeed - shortbowShootSpeedReduction;
                }
                if (primaryWeaponAmmo > 0)
                {
                    if (Input.GetButton("Fire1"))
                    {
                        anim.animator.SetBool("Attack1", true);
                        if (primaryWeaponAmmo == 1)
                        {
                            anim.animator.SetBool("Empty", true);
                        }
                    }
                    else
                    {
                        anim.animator.SetBool("Attack1", false);
                    }

                    if (Input.GetButtonDown("Block"))
                    {
                        anim.SetTrigger("Stahp");
                    }
                }
                else anim.animator.SetBool("Empty", true);
                break;
            case 2:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Shoot"))
                {
                    characterControl.moveSpeed = characterControl.moveSpeed - heavyCrossbowShootSpeedReduction;
                }
                if (primaryWeaponAmmo > 0)
                {
                    Debug.Log(primaryWeaponAmmo);
                    if (Input.GetButtonDown("Fire1") && anim.animator.GetBool("Empty") == false)
                    {
                        anim.animator.SetBool("Empty", true);
                        anim.SetTrigger("Attack3");
                    }

                    if (Input.GetButtonDown("Reload") && anim.animator.GetBool("Empty") == true)
                    {
                        anim.animator.SetBool("Empty", false);
                        anim.SetTrigger("Reload");
                    }
                }
                break;
            case 3:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Shoot"))
                {
                    characterControl.moveSpeed = characterControl.moveSpeed - lightCrossbowShootSpeedReduction;
                }
                if (primaryWeaponAmmo > 0)
                {
                    if (Input.GetButtonDown("Fire1") && anim.animator.GetBool("Empty") == false)
                    {
                        anim.animator.SetBool("Empty", true);
                        anim.SetTrigger("Attack3");
                    }

                    if (Input.GetButtonDown("Reload") && anim.animator.GetBool("Empty") == true)
                    {
                        anim.animator.SetBool("Empty", false);
                        anim.SetTrigger("Reload");
                    }
                }
                break;
            case 4:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed = characterControl.moveSpeed - javelinThrowSpeedReduction;
                }

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
                        CmdBlockStart();
                    }
                    else
                    {
                        anim.animator.SetBool("Block", false);
                        CmdBlockEnd();
                    }

                    if (Input.GetButtonDown("Reload"))
                    {
                        if (!isLitFam)
                        {
                            anim.SetTrigger("Light");
                            javelinDamage = (int)(javelinDamage * litDamageMultiplier);
                            javelinOverheadDamage = (int)(javelinOverheadDamage * litDamageMultiplier);
                            javelinSlashDamage = (int)(javelinSlashDamage * litDamageMultiplier);
                            javelinStabDamage = (int)(javelinStabDamage * litDamageMultiplier);
                        }
                    }
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
                    CmdBlockStart();
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
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed = characterControl.moveSpeed - trapThrowSpeedReduction;
                }

                if (secondaryWeaponAmmo > 0)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }
                else
                {
                    anim.animator.SetBool("OutOfTraps", true);
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        CmdSwapWeapons();
                        if (currentWeapon == 2 || currentWeapon == 3)
                        {
                            anim.animator.SetBool("Empty", false);
                        }
                    }
                }
                break;
            case 9:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed = characterControl.moveSpeed - trapThrowSpeedReduction;
                }

                if (secondaryWeaponAmmo > 0)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }
                else
                {
                    anim.animator.SetBool("OutOfTraps", true);
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        CmdSwapWeapons();
                        if (currentWeapon == 2 || currentWeapon == 3)
                        {
                            anim.animator.SetBool("Empty", false);
                        }
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
                if (isLocalPlayer)
                    matchSlider.gameObject.SetActive(false);
                weapons[10].SetActive(false);
            }

            weapons[primaryWeapon].SetActive(false);
            weapons[secondaryWeapon].SetActive(true);
            anim.animator.SetBool(weaponNames[primaryWeapon], false);
            anim.animator.SetBool(weaponNames[secondaryWeapon], true);
            if (secondaryWeaponAmmo == 0)
            {
                projectileModels[secondaryWeapon].SetActive(false);
            }
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
                if (isLocalPlayer)
                    matchSlider.gameObject.SetActive(true);
                weapons[10].SetActive(true);
            }

            weapons[primaryWeapon].SetActive(true);
            weapons[secondaryWeapon].SetActive(false);
            anim.animator.SetBool(weaponNames[secondaryWeapon], false);
            anim.animator.SetBool(weaponNames[primaryWeapon], true);
            if (primaryWeaponAmmo == 0)
            {
                projectileModels[primaryWeapon].SetActive(false);
            }
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
        if (!isServer)
            return;
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
        if (!isServer)
            return;
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
        if (currentWeapon < 5 && primaryWeaponAmmo > 0)
            projectileModels[currentWeapon].SetActive(true);
    }

    void BowArrowTake()
    {
        if (isLocalPlayer && primaryWeaponAmmo > 0)
        {
            CmdBowArrowTake();
        }
    }

    [Command]
    void CmdBowArrowTake()
    {
        if (!isServer)
            return;
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
        if (isLocalPlayer && primaryWeaponAmmo > 0)
        {
            CmdBowArrowPlace();
        }
    }

    [Command]
    void CmdBowArrowPlace()
    {
        if (!isServer)
            return;
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
        anim.animator.SetBool("Empty", false);
        if (isLocalPlayer && primaryWeaponAmmo > 0)
        {
            CmdJavelinTake();
        } else
        {
            matchSlider.value = 0;
        }
    }

    [Command]
    void CmdJavelinTake()
    {
        if (!isServer)
            return;
        RpcJavelinTake();
    }

    [ClientRpc]
    void RpcJavelinTake()
    {
        projectileModels[4].SetActive(true);
        projectileModels[7].SetActive(false);
    }

    void JavelinReset()
    {
        if (isLocalPlayer && primaryWeaponAmmo > 0)
        {
            CmdJavelinReset();
        }
    }

    [Command]
    void CmdJavelinReset()
    {
        if (!isServer)
            return;
        RpcJavelinReset();
    }

    [ClientRpc]
    void RpcJavelinReset()
    {
        if (primaryWeaponAmmo > 0)
        {
            projectileModels[7].SetActive(true);
        }
    }

    [Command]
    void CmdTrapReset()
    {
        if (!isServer)
            return;
        RpcTrapReset();
    }

    [ClientRpc]
    void RpcTrapReset()
    {
        projectileModels[currentWeapon].SetActive(true);
    }

    void ArcherShoot(GameObject projPrefab)
    {
        if (!isShooting)
        {
            isShooting = true;
            if (currentWeapon < 5) 
                primaryWeaponAmmo--;
            else secondaryWeaponAmmo--;
            CmdArcherShoot(projPrefab);
        }
    }

    [Command]
    void CmdArcherShoot(GameObject projPrefab)
    {
        if (isServer)
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
                bulletSpawn.localPosition = new Vector3(0.322f, 0.305f, 0.16f);
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
                proj.GetComponent<Projectile>().spawnedBy = netId;
                break;
            case 1:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * shortbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = shortbowDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                break;
            case 2:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * heavyCrossbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = heavyCrossbowDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                break;
            case 3:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * lightCrossbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = lightCrossbowDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                break;
            case 4:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * javelinSpeed;
                proj.GetComponent<Projectile>().damage = javelinDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                if (isLitFam)
                {
                    proj.GetComponent<Projectile>().isOnFire = true;
                }
                break;
            case 8:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * trapSpeed;
                proj.GetComponent<Trap>().isExplosive = false;
                proj.GetComponent<Trap>().spawnedBy = netId;
                break;
            case 9:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * trapSpeed;
                proj.GetComponent<Trap>().isExplosive = true;
                proj.GetComponent<Trap>().spawnedBy = netId;
                break;
        }
        Physics.IgnoreCollision(this.GetComponent<Collider>(), proj.GetComponent<Collider>());
        NetworkServer.Spawn(proj);
        Destroy(proj, 30);
    }

    [ClientRpc]
    void RpcArcherShoot()
    {
        projectileModels[currentWeapon].SetActive(false);
    }

    [Command]
    void CmdLightStart()
    {
        if (!isServer)
            return;
        RpcLightStart();
    }

    [ClientRpc]
    void RpcLightStart()
    {
        sparks.SetActive(true);
    }

    void LightEnd()
    {
        isLitFam = true;
        fireSprite.SetActive(true);
        remainingDuration = matchLightDuration;
        if (isLocalPlayer)
        {
            CmdLightEnd();
        }
    }

    [Command]
    void CmdLightEnd()
    {
        if (!isServer)
            return;
        RpcLightEnd();
    }

    [ClientRpc]
    void RpcLightEnd()
    {
        fire.SetActive(true);
        sparks.SetActive(false);
    }

    void FireEnd()
    {
        javelinDamage = (int)(javelinDamage / litDamageMultiplier);
        javelinOverheadDamage = (int)(javelinOverheadDamage / litDamageMultiplier);
        javelinSlashDamage = (int)(javelinSlashDamage / litDamageMultiplier);
        javelinStabDamage = (int)(javelinStabDamage / litDamageMultiplier);
        anim.animator.SetBool("Empty", true);
        fireSprite.SetActive(false);
        if (!isShooting)
            CmdFireEnd();
        else
            CmdFireAbruptEnd();
    }

    [Command]
    void CmdFireEnd()
    {
        RpcFireEnd();
    }

    [ClientRpc]
    void RpcFireEnd()
    {
        primaryWeaponAmmo--;
        weapons[4].SetActive(false);
        fire.SetActive(false);
    }

    [Command]
    void CmdFireAbruptEnd()
    {
        RpcFireAbruptEnd();
    }

    [ClientRpc]
    void RpcFireAbruptEnd()
    {
        fire.SetActive(false);
    }

    void ArcherSlashStart()
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

    
    void ArcherSlashEnd()
    {
        weapons[currentWeapon].GetComponent<Collider>().enabled = false;
    }

    
    void ArcherStabStart()
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

    
    void ArcherStabEnd()
    {
        weapons[currentWeapon].GetComponent<Collider>().enabled = false;
    }

    
    void ArcherOverheadStart()
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

    
    void ArcherOverheadEnd()
    {
        weapons[currentWeapon].GetComponent<Collider>().enabled = false;
    }

    [Command]
    void CmdNotShooting()
    {
        isShooting = false;
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
            weapons[10].GetComponent<Collider>().enabled = true;
        }
        else weapons[currentWeapon].GetComponent<Collider>().enabled = true;
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
            weapons[10].GetComponent<Collider>().enabled = false;
        }
        else weapons[currentWeapon].GetComponent<Collider>().enabled = false;
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