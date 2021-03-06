using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoutControl : NetworkBehaviour
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
    public GameObject[] projectileModels;

    [Header("BulletSpawn in camera, CharacterControl script, Health script")]
    public Transform bulletSpawn;

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
    public int handCrossbowBoltSpeed;

    [Header("Ranged attack projectile damage")]
    [Range(0, 200)]
    public int throwingDaggerDamage;
    [Range(0, 200)]
    public int handCrossbowBoltDamage;

    [Header("Multipliers for movement speed")]
    [Range(0, 1)]
    public float flashBombThrowSpeedMultiplier;
    [Range(0, 1)]
    public float smokeBombThrowSpeedMultiplier;
    [Range(0, 1)]
    public float throwingDaggerThrowSpeedMultiplier;
    [Range(0, 1)]
    public float handCrossbowBoltShootSpeedMultiplier;
    [Range(0, 1)]
    public float weaponSwapSpeedMultiplier;

    [Header("Misc stamina uses")]
    [Range(0, 100)]
    public int daggerThrowStaminaUse;
    [Range(0, 100)]
    public int flashBombThrowStaminaUse;
    [Range(0, 100)]
    public int smokeBombThrowStaminaUse;
    [Range(0, 100)]
    public int blockStaminaUse;

    [Header("UI elements and GameObjects")]
    public Text ammoText;
    public GameObject playerModel;
    public GameObject flashBombFuse;
    public GameObject smokeBombFuse;

    [Header("Audio sources and sounds")]
    public AudioSource audioSource;
    public AudioClip slashClip;
    public AudioClip stabClip;
    public AudioClip overheadClip;
    public AudioClip throwClip;
    public AudioClip bowShootClip;
    public AudioClip bowPullClip;
    public AudioClip arrowPlaceClip;
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

    // Use this for initialization
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
            anim.animator.ResetTrigger("Reload");
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

                if (currentWeapon == 5)
                {
                    CmdVisible();
                }
                if (currentWeapon < 5)
                {
                    anim.animator.SetBool("OutOfSecondary", false);
                }
                SwapWeapons();
                if (currentWeapon == 9 && secondaryWeaponAmmo > 0)
                {
                    anim.animator.SetBool("Empty", false);
                }
            }
        }

        switch (currentWeapon)
        {
            case 0:
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

                    //if (Input.GetButtonDown("Block"))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 1:
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

                    //if (Input.GetButtonDown("Block"))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 2:
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

                    //if (Input.GetButtonDown("Block"))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 3:
                //if (Input.GetButton("Fire1") && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
                if(Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
                {
                    anim.animator.SetBool("Attack1", true);
                }
                else
                {
                    anim.animator.SetBool("Attack1", false);
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
                break;
            case 4:
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

                    //if (Input.GetButtonDown("Block"))
                    if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 5:
                if (characterControl.isCrouched && playerModel.activeInHierarchy)
                {
                    CmdInvisible();
                } else if (!playerModel.activeInHierarchy)
                {
                    CmdVisible();
                }
                break;
            case 6:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed *= flashBombThrowSpeedMultiplier;
                }
                if (secondaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        //if (Input.GetButtonDown("Fire1") && !health.IsStaminaZero(-flashBombThrowStaminaUse))
                        if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-flashBombThrowStaminaUse))
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
                        SwapWeapons();
                    }
                }
                break;
            case 7:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed *= smokeBombThrowSpeedMultiplier;
                }
                if (secondaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        //if (Input.GetButtonDown("Fire1") && !health.IsStaminaZero(-smokeBombThrowStaminaUse))
                        if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-smokeBombThrowStaminaUse))
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
                        SwapWeapons();
                    }
                }
                break;
            case 8:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed *= throwingDaggerThrowSpeedMultiplier;
                }
                if (secondaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        //if (Input.GetButtonDown("Fire1") && !health.IsStaminaZero(-daggerThrowStaminaUse))
                        if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-daggerThrowStaminaUse))
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
                        SwapWeapons();
                    }
                }
                break;
            case 9:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Shoot"))
                {
                    characterControl.moveSpeed *= handCrossbowBoltShootSpeedMultiplier;
                }
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (secondaryWeaponAmmo > 0)
                    {
                        //if (Input.GetButtonDown("Fire1"))
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && anim.animator.GetBool("Empty") == false)
                        {
                            anim.SetTrigger("Attack3");
                            anim.animator.SetBool("Empty", true);
                        }
                        //if (Input.GetButtonDown("Reload"))
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Reload)"]) && anim.animator.GetBool("Empty") == true)
                        {
                            anim.SetTrigger("Reload");
                            anim.animator.SetBool("Empty", false);
                        }
                    }
                }
                else
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                    {
                        anim.animator.SetBool("OutOfSecondary", true);
                        SwapWeapons();
                    }
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

            weapons[primaryWeapon].SetActive(false);
            weapons[secondaryWeapon].SetActive(true);
            if (isLocalPlayer)
            {
                anim.animator.SetBool(weaponNames[primaryWeapon], false);
                anim.animator.SetBool(weaponNames[secondaryWeapon], true);
                anim.animator.SetFloat("Attack speed", attackSpeeds[secondaryWeapon]);
            }

            if (secondaryWeaponAmmo == 0 && secondaryWeapon == 9)
            {
                projectileModels[3].SetActive(false);
            } else if (secondaryWeapon == 9)
            {
                projectileModels[3].SetActive(true);
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
        if (!isServer)
            return;

        switch (currentWeapon)
        {
            case 0:
                weapons[10].GetComponent<MeleeWeapon>().collidersActive = true;
                weapons[10].GetComponent<MeleeWeapon>().damage = 0;
                weapons[10].tag = "Block";
                weapons[10].GetComponent<MeleeWeapon>().isTrigger = false;
                break;
            case 1:
                weapons[11].GetComponent<MeleeWeapon>().collidersActive = true;
                weapons[11].GetComponent<MeleeWeapon>().damage = 0;
                weapons[11].tag = "Block";
                weapons[11].GetComponent<MeleeWeapon>().isTrigger = false;
                break;
            case 2:
                weapons[12].GetComponent<MeleeWeapon>().collidersActive = true;
                weapons[12].GetComponent<MeleeWeapon>().damage = 0;
                weapons[12].tag = "Block";
                weapons[12].GetComponent<MeleeWeapon>().isTrigger = false;
                break;
            case 3:
                weapons[17].GetComponent<Collider>().enabled = true;
                break;
        }
        if (currentWeapon != 3)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = true;
            weapons[currentWeapon].GetComponent<MeleeWeapon>().damage = 0;
            weapons[currentWeapon].tag = "Block";
            weapons[currentWeapon].GetComponent<MeleeWeapon>().isTrigger = false;
        }
    }

    [Command]
    void CmdBlockEnd()
    {
        if (!isServer)
            return;

        switch (currentWeapon)
        {
            case 0:
                weapons[10].GetComponent<MeleeWeapon>().collidersActive = false;
                weapons[10].tag = "Weapon";
                weapons[10].GetComponent<MeleeWeapon>().isTrigger = true;
                break;
            case 1:
                weapons[11].GetComponent<MeleeWeapon>().collidersActive = false;
                weapons[11].tag = "Weapon";
                weapons[11].GetComponent<MeleeWeapon>().isTrigger = true;
                break;
            case 2:
                weapons[12].GetComponent<MeleeWeapon>().collidersActive = false;
                weapons[12].tag = "Weapon";
                weapons[12].GetComponent<MeleeWeapon>().isTrigger = true;
                break;
            case 3:
                weapons[17].GetComponent<Collider>().enabled = false;
                break;
        }
        if (currentWeapon != 3)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[currentWeapon].tag = "Weapon";
            weapons[currentWeapon].GetComponent<MeleeWeapon>().isTrigger = true;
        }
    }

    void ScoutShoot(GameObject projPrefab)
    {
        if (isLocalPlayer)
        {
            if (currentWeapon < 5)
                SetAmmoText(--primaryWeaponAmmo);
            else SetAmmoText(--secondaryWeaponAmmo);
            switch (currentWeapon)
            {
                case 6:
                    health.CmdChangeStamina(-flashBombThrowStaminaUse);
                    break;
                case 7:
                    health.CmdChangeStamina(-smokeBombThrowStaminaUse);
                    break;
                case 8:
                    health.CmdChangeStamina(-daggerThrowStaminaUse);
                    break;
            }
        }
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
                bulletSpawn.localPosition = new Vector3(0.577f, -0.958f, 0.748f);
                break;
            case 7:
                bulletSpawn.localPosition = new Vector3(0.577f, -0.958f, 0.748f);
                break;
            case 8:
                bulletSpawn.localPosition = new Vector3(0.256f, -0.238f, 1.065f);
                break;
            case 9:
                bulletSpawn.localPosition = new Vector3(0.397f, -0.223f, 0.829f);
                break;
        }

        GameObject proj = null;

        switch (currentWeapon)
        {
            case 6:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation * Quaternion.Euler(0, 0, -90));
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * flashBombSpeed;
                proj.GetComponent<Bomb>().spawnedBy = netId;
                proj.GetComponent<Bomb>().bombType = 0;
                proj.GetComponent<Bomb>().team = GetComponent<CharacterControl>().Team();
                proj.GetComponent<CapsuleCollider>().height = 0.4f;
                proj.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.41f, 0);
                break;
            case 7:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation * Quaternion.Euler(0, 0, -90));
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * smokeBombSpeed;
                proj.GetComponent<Bomb>().spawnedBy = netId;
                proj.GetComponent<Bomb>().bombType = 1;
                proj.GetComponent<Bomb>().team = GetComponent<CharacterControl>().Team();
                break;
            case 8:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * throwingDaggerSpeed;
                proj.GetComponent<Projectile>().damage = throwingDaggerDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
                break;
            case 9:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * handCrossbowBoltSpeed;
                proj.GetComponent<Projectile>().damage = handCrossbowBoltDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
                break;
        }
        NetworkServer.Spawn(proj);
        Destroy(proj, 30);
    }

    [ClientRpc]
    void RpcScoutShoot()
    {
        projectileModels[currentWeapon - 6].SetActive(false);
        if (currentWeapon == 6)
        {
            flashBombFuse.SetActive(false);
        }
        else if (currentWeapon == 7)
        {
            smokeBombFuse.SetActive(false);
        }
    }

    void ScoutSlashStart()
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
        RpcSlashStart();
    }

    [ClientRpc]
    void RpcSlashStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(slashClip);
        MeleeWeapon wepon2;
        switch (currentWeapon)
        {
            case 0:
                wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(slashClip);
                break;
            case 1:
                wepon2 = weapons[11].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(slashClip);
                break;
            case 2:
                wepon2 = weapons[12].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(slashClip);
                break;
        }
    }

    void ScoutSlashEnd()
    {
        if (isLocalPlayer)
            CmdSlashEnd();
    }

    [Command]
    void CmdSlashEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        switch (currentWeapon)
        {
            case 0:
                weapons[10].GetComponent<MeleeWeapon>().collidersActive = false;
                break;
            case 1:
                weapons[11].GetComponent<MeleeWeapon>().collidersActive = false;
                break;
            case 2:
                weapons[12].GetComponent<MeleeWeapon>().collidersActive = false;
                break;
        }
    }

    void ScoutStabStart()
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
        RpcStabStart();
    }

    [ClientRpc]
    void RpcStabStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(stabClip);
        MeleeWeapon wepon2;
        switch (currentWeapon)
        {
            case 0:
                wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(stabClip);
                break;
            case 1:
                wepon2 = weapons[11].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(stabClip);
                break;
            case 2:
                wepon2 = weapons[12].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(stabClip);
                break;
        }
    }

    void ScoutStabEnd()
    {
        if (isLocalPlayer)
            CmdStabEnd();
    }

    [Command]
    void CmdStabEnd()
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

    void ScoutOverheadStart()
    {
        if (isLocalPlayer)
        {
            health.CmdChangeStamina(-overheadStaminaUse[currentWeapon]);
            CmdOverheadStart();
        }
    }

    [Command]
    void CmdOverheadStart()
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
        RpcOverheadStart();
    }

    [ClientRpc]
    void RpcOverheadStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(overheadClip);
        MeleeWeapon wepon2;
        switch (currentWeapon)
        {
            case 0:
                wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(overheadClip);
                break;
            case 1:
                wepon2 = weapons[11].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(overheadClip);
                break;
            case 2:
                wepon2 = weapons[12].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(overheadClip);
                break;
        }
    }

    void ScoutOverheadEnd()
    {
        if (isLocalPlayer)
            CmdOverheadEnd();
    }

    [Command]
    void CmdOverheadEnd()
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
    void CmdScoutRedraw()
    {
        if (isServer)
        {
            RpcDraw(2);
            RpcScoutRedraw();
        }
    }

    [ClientRpc]
    void RpcScoutRedraw()
    {
        if (currentWeapon <= 7)
        {
            projectileModels[currentWeapon - 6].SetActive(true);
        }
        else if (currentWeapon == 8)
        {
            projectileModels[2].SetActive(true);
            projectileModels[4].SetActive(false);
        }
        else
        {
            projectileModels[5].SetActive(true);
        }
    }

    [Command]
    void CmdScoutReset()
    {
        if (isServer)
        {
            RpcScoutReset();
        }
    }

    [ClientRpc]
    void RpcScoutReset()
    {
        if (currentWeapon == 8)
        {
            projectileModels[4].SetActive(true);
        }
        else
        {
            projectileModels[currentWeapon - 6].SetActive(true);
            projectileModels[currentWeapon - 4].SetActive(false);
        }
        if (currentWeapon == 9)
        {
            weapons[currentWeapon].GetComponent<AudioSource>().pitch = 1;
            weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(arrowPlaceClip);
        }
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
        if (currentWeapon == 6)
        {
            flashBombFuse.SetActive(true);
        }
        else
        {
            smokeBombFuse.SetActive(true);
        }
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

    [Command]
    void CmdInvisible()
    {
        RpcInvisible();
    }

    [ClientRpc]
    void RpcInvisible()
    {
        if (!isLocalPlayer)
            playerModel.SetActive(false);
    }

    [Command]
    void CmdVisible()
    {
        RpcVisible();
    }

    [ClientRpc]
    void RpcVisible()
    {
        playerModel.SetActive(true);
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
        if (currentWeapon == 9)
        {
            weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
            weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(bowShootClip);
        }
        else
        {
            weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
            weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(throwClip);
        }
    }

    void PullString()
    {
        if (isLocalPlayer)
            CmdPullString();
    }

    [Command]
    void CmdPullString()
    {
        RpcPullString();
    }

    [ClientRpc]
    void RpcPullString()
    {
        weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
        weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(bowPullClip);
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
                weapons[10].GetComponent<MeleeWeapon>().audioSourceDamage.PlayOneShot(blockClip);
                break;
            case 1:
                weapons[11].GetComponent<MeleeWeapon>().audioSourceDamage.PlayOneShot(blockClip);
                break;
            case 2:
                weapons[12].GetComponent<MeleeWeapon>().audioSourceDamage.PlayOneShot(blockClip);
                break;
            case 3:
                weapons[17].GetComponent<AudioSource>().PlayOneShot(blockClip);
                break;
        }
        if (currentWeapon != 3)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().audioSourceDamage.PlayOneShot(blockClip);
        }
    }
}