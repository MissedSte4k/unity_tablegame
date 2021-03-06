using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class KnightControl : NetworkBehaviour
{

    [SyncVar]
    [HideInInspector]
    public int primaryWeapon;
    [SyncVar]
    [HideInInspector]
    public int secondaryWeapon;

    [Header("Arrays for weapon objects, their names, ammo, other models")]
    public GameObject[] weapons;
    public string[] weaponNames;
    [Range(0, 30)]
    public int[] maxAmmo;
    [Range(0, 2)]
    public float[] attackSpeeds;
    [Range(0, 100)]
    public int[] slashStaminaUse;
    [Range(0, 100)]
    public int[] stabStaminaUse;
    [Range(0, 100)]
    public int[] overheadStaminaUse;

    [Header("BulletSpawn in camera and CharacterControl script")]
    public Transform bulletSpawn;

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

    [Header("Misc stamina uses")]
    [Range(0, 100)]
    public int bombThrowStaminaUse;
    [Range(0, 100)]
    public int diskThrowStaminaUse;
    [Range(0, 100)]
    public int blockStaminaUse;

    [Header("Banner heal settings")]
    [Range(0, 2)]
    public float bannerHealInterval;
    [Range(0, 50)]
    public float bannerHealRadius;
    [Range(0, 100)]
    public int bannerHealAmount;
    private float intervalTick = 0;

    [Header("UI elements and GameObjects")]
    public Text ammoText;
    public GameObject bombFuse;

    [Header("Audio sources and sounds")]
    public AudioSource audioSource;
    public AudioClip slashClip;
    public AudioClip stabClip;
    public AudioClip overheadClip;
    public AudioClip throwClip;
    public AudioClip blockClip;
    public AudioClip drawPrimary;
    public AudioClip drawSecondary;
    public AudioClip drawAmmo;

    [SyncVar] private int currentWeapon;
    private NetworkAnimator anim;

    [HideInInspector]
    [SyncVar]
    public int primaryWeaponAmmo;
    [HideInInspector]
    [SyncVar]
    public int secondaryWeaponAmmo;

    private CharacterControl characterControl;
    private Health health;

    void OnEnable()
    {
        if (!isLocalPlayer)
            return;

        if (anim == null)
            anim = GetComponent<NetworkAnimator>();
        if (characterControl == null)
            characterControl = GetComponent<CharacterControl>();
        if (health == null)
            health = GetComponent<Health>();

        PlayOptions po = FindObjectOfType<PlayOptions>();
        primaryWeapon = po.primaryWeaponIndex;
        secondaryWeapon = po.secondaryWeaponIndex + 5;
        CmdSetWeapons(primaryWeapon, secondaryWeapon);

        primaryWeaponAmmo = maxAmmo[primaryWeapon];
        secondaryWeaponAmmo = maxAmmo[secondaryWeapon];
        CmdSetAmmo(primaryWeaponAmmo, secondaryWeaponAmmo);

        currentWeapon = primaryWeapon;
        CmdActivateWeapon(primaryWeapon);

        foreach (AnimatorControllerParameter parameter in anim.animator.parameters)
        {
            anim.animator.SetBool(parameter.name, false);
        }
        ShowMeWhatYouGot();

        CmdDraw(0);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        characterControl.moveSpeed = characterControl.defaultMoveSpeed;

        if (anim.animator.GetBool("Block hurt") == true)
        {
            anim.animator.ResetTrigger("Block hurt");
            health.CmdChangeStamina(-blockStaminaUse);
            CmdBlockSound();
        }

        if (NetworkServer.active)
        {
            anim.animator.ResetTrigger("Attack2");
            anim.animator.ResetTrigger("Attack3");
            anim.animator.ResetTrigger("Parry");
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Draw"))
        {
            characterControl.moveSpeed *= weaponSwapSpeedMultiplier;
        }

        if (characterControl.onSprint)
            return;

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
        {
            //if (Input.GetButtonDown("Swap") && (secondaryWeaponAmmo > 0 || maxAmmo[secondaryWeapon] == 0))
            if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(SwapWeapon)"]) && (secondaryWeaponAmmo > 0 || maxAmmo[secondaryWeapon] == 0))
            {
                if (currentWeapon == primaryWeapon) CmdDraw(1);
                else CmdDraw(0);
                if (currentWeapon < 5)
                {
                    anim.animator.SetBool("OutOfSecondary", false);
                }
                SwapWeapons();
            }
        }

        switch (currentWeapon)
        {
            case 0:
                // ataka vyksta kol laikomas nuspaustas mygtukas
                //if (Input.GetButton("Fire1") && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
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
                    //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    // ataka vyksta 1 kartą nuspaudus mygtuką
                    //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }

                // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                //if (Input.GetButton("Block"))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
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
                //if (Input.GetButton("Fire1") && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
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
                    //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    // ataka vyksta 1 kartą nuspaudus mygtuką
                    //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }

                // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                //if (Input.GetButton("Block"))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
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
                //if (Input.GetButton("Fire1") && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
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
                    //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    // ataka vyksta 1 kartą nuspaudus mygtuką
                    //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }

                // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                //if (Input.GetButton("Block"))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
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
                //if (Input.GetButton("Fire1") && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
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
                    //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    // ataka vyksta 1 kartą nuspaudus mygtuką
                    //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                    //if (Input.GetButton("Block"))
                    if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 4:
                // ataka vyksta kol laikomas nuspaustas mygtukas
                //if (Input.GetButton("Fire1") && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
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
                    //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    // ataka vyksta 1 kartą nuspaudus mygtuką
                    //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                    //if (Input.GetButton("Block"))
                    if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 5:
                if (intervalTick <= 0)
                {
                    Collider[] colliders = Physics.OverlapSphere(transform.position, bannerHealRadius);
                    foreach (Collider col in colliders)
                    {
                        if (col.tag == "Player" && col.GetComponent<CharacterControl>().Team() == characterControl.Team())
                        {
                            col.GetComponent<Health>().Heal(bannerHealAmount);
                        }
                    }
                    intervalTick = bannerHealInterval;
                } else
                {
                    intervalTick -= Time.deltaTime;
                }

                // ataka vyksta kol laikomas nuspaustas mygtukas
                //if (Input.GetButton("Fire1") && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
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
                    //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    // ataka vyksta 1 kartą nuspaudus mygtuką
                    //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
                    //if (Input.GetButton("Block"))
                    if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 6:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed *= cherryBombSpeedMultiplier;
                }
                if (secondaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        //if (Input.GetButtonDown("Fire1") && !health.IsStaminaZero(bombThrowStaminaUse))
                        if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(bombThrowStaminaUse))
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
                        if (currentWeapon == primaryWeapon) CmdDraw(1);
                        else CmdDraw(0);
                        SwapWeapons();
                    }
                }
                break;
            case 7:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed *= throwingDiskSpeedMultiplier;
                }
                if (secondaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        //if (Input.GetButtonDown("Fire1") && !health.IsStaminaZero(diskThrowStaminaUse))
                        if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(diskThrowStaminaUse))
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
                        if (currentWeapon == primaryWeapon) CmdDraw(1);
                        else CmdDraw(0);
                        SwapWeapons();
                    }
                }
                break;
            case 8:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed *= matchboxSpeedMultiplier;
                }
                if (secondaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        //if (Input.GetButtonDown("Fire1"))
                        if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]))
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
                        if (currentWeapon == primaryWeapon) CmdDraw(1);
                        else CmdDraw(0);
                        SwapWeapons();
                    }
                }
                break;
            case 9:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    //if (Input.GetButtonDown("Fire1") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }
                }

                //if (Input.GetButton("Block"))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
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
            if (isLocalPlayer)
            {
                anim.animator.SetBool(weaponNames[primaryWeapon], false);
                anim.animator.SetBool(weaponNames[secondaryWeapon], true);
                anim.animator.SetFloat("Attack speed", attackSpeeds[secondaryWeapon]);
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
            if (isLocalPlayer)
            {
                anim.animator.SetBool(weaponNames[secondaryWeapon], false);
                anim.animator.SetBool(weaponNames[primaryWeapon], true);
                anim.animator.SetFloat("Attack speed", attackSpeeds[primaryWeapon]);
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
        RpcActivateWeapon(value);
    }

    [ClientRpc]
    void RpcActivateWeapon(int value)
    {
        ActivateWeapon(value);
    }

    void SwapWeapons()
    {
        if (currentWeapon == primaryWeapon)
        {
            currentWeapon = secondaryWeapon;
        }
        else /*if (currentWeapon == secondaryWeapon)*/
        {
            currentWeapon = primaryWeapon;
        }
        CmdActivateWeapon(currentWeapon);
    }

    [Command]
    void CmdBlockStart()
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
            weapons[currentWeapon].GetComponent<MeleeWeapon>().isTrigger = false;
            weapons[currentWeapon].tag = "Block";
        }
    }

    [Command]
    void CmdBlockEnd()
    {
        switch (currentWeapon)
        {
            case 0:
                weapons[12].GetComponent<Collider>().enabled = false;
                break;
            case 1:
                weapons[12].GetComponent<Collider>().enabled = false;
                break;
            case 2:
                weapons[12].GetComponent<Collider>().enabled = false;
                break;
            case 9:
                weapons[9].GetComponent<Collider>().enabled = false;
                break;
        }
        if (currentWeapon > 2 && currentWeapon != 9)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[currentWeapon].tag = "Weapon";
            weapons[currentWeapon].GetComponent<MeleeWeapon>().isTrigger = true;
        }
    }

    void KnightShoot(GameObject projPrefab)
    {
        if (isLocalPlayer)
        {
            SetAmmoText(--secondaryWeaponAmmo);
            if (currentWeapon == 6)
            {
                health.CmdChangeStamina(-bombThrowStaminaUse);
            }
            else if (currentWeapon == 7)
            {
                health.CmdChangeStamina(-diskThrowStaminaUse);
            }
        }
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
                bulletSpawn.localPosition = new Vector3(1.139f, -0.122f, 0.766f);
                break;
            case 7:
                bulletSpawn.localPosition = new Vector3(0.845f, -0.334f, 0.962f);
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
                proj.GetComponent<Bomb>().team = GetComponent<CharacterControl>().Team();
                proj.GetComponent<CapsuleCollider>().radius = 0.35f;
                proj.GetComponent<CapsuleCollider>().height = 0.6f;
                proj.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.38f, 0);
                break;
            case 7:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * throwingDiskSpeed;
                proj.GetComponent<Projectile>().damage = throwingDiskDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
                break;
            case 8:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * ammoBoxSpeed;
                proj.GetComponent<AmmoBox>().spawnedBy = netId;
                proj.GetComponent<AmmoBox>().team = GetComponent<CharacterControl>().Team();
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
        if (isLocalPlayer)
        {
            health.CmdChangeStamina(-slashStaminaUse[currentWeapon]);
            CmdSlashStart();
        }
    }

    [Command]
    void CmdSlashStart()
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
        RpcSlashStart();
    }

    [ClientRpc]
    void RpcSlashStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(slashClip);
    }

    void KnightSlashEnd()
    {
        if (isLocalPlayer)
            CmdSlashEnd();
    }

    [Command]
    void CmdSlashEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
    }

    void KnightStabStart()
    {
        if (isLocalPlayer)
        {
            health.CmdChangeStamina(-stabStaminaUse[currentWeapon]);
            CmdStabStart();
        }
    }

    [Command]
    void CmdStabStart()
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
        RpcStabStart();
    }

    [ClientRpc]
    void RpcStabStart()
    {
        MeleeWeapon wepon;
        if (currentWeapon != 9)
        {
            wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        }
        else
        {
            wepon = weapons[13].GetComponent<MeleeWeapon>();
        }
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(stabClip);
    }

    void KnightStabEnd()
    {
        if (isLocalPlayer)
            CmdStabEnd();
    }

    [Command]
    void CmdStabEnd()
    {
        if (currentWeapon != 9)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        }
        else
        {
            weapons[13].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[9].GetComponent<Collider>().enabled = false;
        }
    }

    void KnightOverheadStart()
    {
        if (isLocalPlayer)
        {
            health.CmdChangeStamina(-overheadStaminaUse[currentWeapon]);
            CmdOverheadStart();
        }
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(overheadClip);
    }

    [Command]
    void CmdOverheadStart()
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
        RpcOverheadStart();
    }

    [ClientRpc]
    void RpcOverheadStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(overheadClip);
    }

    void KnightOverheadEnd()
    {
        if (isLocalPlayer)
            CmdOverheadEnd();
    }

    [Command]
    void CmdOverheadEnd()
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
            CmdSetAmmo(primaryWeaponAmmo, secondaryWeaponAmmo);
        }
    }

    [Command]
    void CmdSetAmmo(int ammo1, int ammo2)
    {
        RpcSetAmmo(ammo1, ammo2);
    }

    [ClientRpc]
    void RpcSetAmmo(int ammo1, int ammo2)
    {
        primaryWeaponAmmo = ammo1;
        secondaryWeaponAmmo = ammo2;
    }

    [Command]
    void CmdKnightRedraw()
    {
        if (isServer)
        {
            RpcDraw(2);
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

    [Command]
    void CmdSetWeapons(int primary, int secondary)
    {
        RpcSetWeapons(primary, secondary);
    }

    [ClientRpc]
    void RpcSetWeapons(int primary, int secondary)
    {
        primaryWeapon = primary;
        secondaryWeapon = secondary;
    }

    public bool isAmmoFull()
    {
        if (primaryWeaponAmmo < maxAmmo[primaryWeapon] || secondaryWeaponAmmo < maxAmmo[secondaryWeaponAmmo])
        {
            return false;
        }

        return true;
    }

    public void ammoRefresh()
    {
        if (isLocalPlayer)
        {
            primaryWeaponAmmo = maxAmmo[primaryWeapon];
            secondaryWeaponAmmo = maxAmmo[secondaryWeapon];
            CmdSetAmmo(maxAmmo[primaryWeapon], maxAmmo[secondaryWeapon]);
            if (currentWeapon == primaryWeapon)
            {
                SetAmmoText(maxAmmo[primaryWeapon]);
            }
            else
            {
                SetAmmoText(maxAmmo[secondaryWeapon]);
            }
        }
    }

    void ShootSound()
    {
        if (isLocalPlayer)
            CmdShootSound();
    }

    [Command]
    void CmdShootSound()
    {
        RpcShootSound();
    }

    [ClientRpc]
    void RpcShootSound()
    {
        weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
        weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(throwClip);
    }

    [Command]
    void CmdDraw(int num)
    {
        RpcDraw(num);
    }

    [ClientRpc]
    void RpcDraw(int num)
    {
        switch (num)
        {
            case 0:
                audioSource.PlayOneShot(drawPrimary);
                break;
            case 1:
                audioSource.PlayOneShot(drawSecondary);
                break;
            case 2:
                audioSource.PlayOneShot(drawAmmo);
                break;
        }
    }

    [Command]
    void CmdBlockSound()
    {
        RpcBlockSound();
    }

    [ClientRpc]
    void RpcBlockSound()
    {
        switch (currentWeapon)
        {
            case 0:
                weapons[12].GetComponent<AudioSource>().PlayOneShot(blockClip);
                break;
            case 1:
                weapons[12].GetComponent<AudioSource>().PlayOneShot(blockClip);
                break;
            case 2:
                weapons[12].GetComponent<AudioSource>().PlayOneShot(blockClip);
                break;
            case 9:
                weapons[9].GetComponent<AudioSource>().PlayOneShot(blockClip);
                break;
        }
        if (currentWeapon > 2 && currentWeapon != 9)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().audioSourceDamage.PlayOneShot(blockClip);
        }
    }
}