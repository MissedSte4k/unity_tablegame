using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BerserkerControl : NetworkBehaviour
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
    public float axeThrowSpeedMultiplier;
    [Range(0, 1)]
    public float weaponSwapSpeedMultiplier;

    [Header("Misc stamina uses")]
    [Range(0, 100)]
    public int axeThrowStaminaUse;
    [Range(0, 100)]
    public int boulderThrowStaminaUse;
    [Range(0, 100)]
    public int blockStaminaUse;

    [Header("Food settings")]
    [Range(0, 60)]
    public float energyPillDuration;
    [Range(0, 10)]
    public float energyPillSpeedMultiplier;
    [Range(0, 200)]
    public int peanutHealTotal;

    [Header("UI elements and GameObjects")]
    public Text ammoText;
    public Slider energySlider;

    [Header("Audio sources and sounds")]
    public AudioSource audioSource;
    public AudioClip slashClip;
    public AudioClip stabClip;
    public AudioClip overheadClip;
    public AudioClip throwClip;
    public AudioClip nomClip;
    public AudioClip blockClip;
    public AudioClip drawPrimary;
    public AudioClip drawSecondary;
    public AudioClip drawAmmo;

    [SyncVar] private int currentWeapon;
    private bool isThrow1;
    private NetworkAnimator anim;

    [HideInInspector]
    [SyncVar]
    public int primaryWeaponAmmo;
    [HideInInspector]
    [SyncVar]
    public int secondaryWeaponAmmo;

    private bool isCaffeinated = false;
    private float remainingDuration;

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
        if (primaryWeapon != 4)
            secondaryWeapon = po.secondaryWeaponIndex + 5;
        else secondaryWeapon = 8;
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
        isThrow1 = false;
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
            anim.animator.ResetTrigger("Throw1");
            anim.animator.ResetTrigger("Throw2");
            anim.animator.ResetTrigger("Parry");
        }

        if (isCaffeinated && !anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Eat"))
        {
            if (remainingDuration < 0)
            {
                isCaffeinated = false;
            }
            else if (remainingDuration >= 0)
            {
                energySlider.value = remainingDuration / energyPillDuration;
                remainingDuration -= Time.deltaTime;
            }
            characterControl.moveSpeed *= energyPillSpeedMultiplier;
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Draw"))
        {
            characterControl.moveSpeed *= weaponSwapSpeedMultiplier;
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Eat"))
        {
            characterControl.moveSpeed *= eatSpeedMultiplier;
        }

        if (characterControl.onSprint)
            return;

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
        {
            //if (Input.GetButtonDown("Swap") && (secondaryWeaponAmmo > 0 || maxAmmo[secondaryWeapon] == 0) && currentWeapon != 4)
            if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(SwapWeapon)"]) && (secondaryWeaponAmmo > 0 || maxAmmo[secondaryWeapon] == 0) && currentWeapon != 4)
            {
                if (currentWeapon < 5)
                {
                    anim.animator.SetBool("OutOfSecondary", false);
                }
                if (currentWeapon == primaryWeapon) CmdDraw(1);
                else CmdDraw(0);
                SwapWeapons();
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
            case 4:
                if (primaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                    {
                        characterControl.moveSpeed *= axeThrowSpeedMultiplier;
                    }
                    if (anim.animator.GetBool("Empty") == true)
                    {
                        if (primaryWeaponAmmo >= 2)
                            anim.animator.SetBool("Empty", false);
                        CmdActivateWeapon(currentWeapon);
                    }
                    //if (Input.GetButton("Fire1") && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
                    if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-slashStaminaUse[currentWeapon]))
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

                        //if (Input.GetButtonDown("Fire4") && !health.IsStaminaZero(-axeThrowStaminaUse))
                        if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack4)"]) && !health.IsStaminaZero(-axeThrowStaminaUse))
                        {
                            if (primaryWeaponAmmo <= 2)
                            {
                                anim.animator.SetBool("Empty", true);
                            }
                            if (isThrow1)
                            {
                                anim.SetTrigger("Throw1");
                            }
                            else
                            {
                                anim.SetTrigger("Throw2");
                            }
                        }

                        //if (Input.GetButtonDown("Block"))
                        if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
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
                        //if (Input.GetButtonDown("Fire1") && !health.IsStaminaZero(-axeThrowStaminaUse))
                        if(Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && !health.IsStaminaZero(-axeThrowStaminaUse))
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
                        if (currentWeapon == primaryWeapon) CmdDraw(1);
                        else CmdDraw(0);
                        SwapWeapons();

                    }
                }
                break;
            case 6:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (secondaryWeaponAmmo > 0)
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
                    else
                    {
                        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                        {
                            if (currentWeapon == primaryWeapon) CmdDraw(1);
                            else CmdDraw(0);
                            SwapWeapons();
                        }
                    }
                }
                break;
            case 7:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (secondaryWeaponAmmo > 0)
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
                    else
                    {
                        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                        {
                            if (currentWeapon == primaryWeapon) CmdDraw(1);
                            else CmdDraw(0);
                            SwapWeapons();
                        }
                    }
                }
                break;
            case 8:
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
        }
    }

    public void ActivateWeapon(int value)
    {
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
            if (isLocalPlayer)
            {
                anim.animator.SetBool(weaponNames[secondaryWeapon], false);
                anim.animator.SetBool(weaponNames[primaryWeapon], true);
                anim.animator.SetFloat("Attack speed", attackSpeeds[primaryWeapon]);
            }
            if (primaryWeaponAmmo == 0)
            {
                projectileModels[0].SetActive(false);
                projectileModels[1].SetActive(false);
            }
            if (primaryWeaponAmmo == 1)
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

        if (currentWeapon == 4)
        {
            weapons[9].GetComponent<MeleeWeapon>().collidersActive = true;
            weapons[9].GetComponent<MeleeWeapon>().damage = 0;
            weapons[9].tag = "Block";
            weapons[9].GetComponent<MeleeWeapon>().isTrigger = false;
        }
        else if (currentWeapon == 8)
        {
            weapons[10].GetComponent<MeleeWeapon>().collidersActive = true;
            weapons[10].GetComponent<MeleeWeapon>().damage = 0;
            weapons[10].tag = "Block";
            weapons[10].GetComponent<MeleeWeapon>().isTrigger = false;
        }
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = true;
        weapons[currentWeapon].GetComponent<MeleeWeapon>().damage = 0;
        weapons[currentWeapon].tag = "Block";
        weapons[currentWeapon].GetComponent<MeleeWeapon>().isTrigger = false;
    }

    [Command]
    void CmdBlockEnd()
    {
        if (!isServer)
            return;

        if (currentWeapon == 4)
        {
            weapons[9].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[9].tag = "Weapon";
            weapons[9].GetComponent<MeleeWeapon>().isTrigger = true;
        }
        else if (currentWeapon == 8)
        {
            weapons[10].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[10].tag = "Weapon";
            weapons[10].GetComponent<MeleeWeapon>().isTrigger = true;
        }
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
        weapons[currentWeapon].tag = "Weapon";
        weapons[currentWeapon].GetComponent<MeleeWeapon>().isTrigger = true;
    }

    void BerserkerShoot(GameObject projPrefab)
    {
        if (isLocalPlayer)
        {
            if (currentWeapon == 4)
            {
                health.CmdChangeStamina(-axeThrowStaminaUse);
                SetAmmoText(--primaryWeaponAmmo);
            }
            else
            {
                health.CmdChangeStamina(-boulderThrowStaminaUse);
                SetAmmoText(--secondaryWeaponAmmo);
            }
        }
        Debug.Log("Potato");
        CmdBerserkerShoot(projPrefab);
    }

    [Command]
    void CmdBerserkerShoot(GameObject projPrefab)
    {
        if (isServer)
            RpcBerserkerShoot();

        switch (currentWeapon)
        {
            case 4:
                if (isThrow1)
                {
                    bulletSpawn.localPosition = new Vector3(0.737f, -0.122f, 1.315f);
                }
                else
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
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
                break;
            case 5:
                proj = Instantiate(projPrefab, bulletSpawn.position, bulletSpawn.rotation);
                proj.GetComponent<Rigidbody>().velocity = bulletSpawn.forward * boulderSpeed;
                proj.GetComponent<Projectile>().damage = boulderDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
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
        }
        else
        {
            if (isThrow1 && (projectileModels[1].activeInHierarchy == true || primaryWeaponAmmo == 0))
            {
                projectileModels[0].SetActive(false);
                isThrow1 = false;
            }
            else if (!isThrow1 && (projectileModels[0].activeInHierarchy == true || primaryWeaponAmmo == 0))
            {
                projectileModels[1].SetActive(false);
                isThrow1 = true;
            }
        }
    }

    [Command]
    void CmdBerserkerRedraw()
    {
        if (isServer)
        {
            RpcDraw(2);
            RpcBerserkerRedraw();
        }
    }

    [ClientRpc]
    void RpcBerserkerRedraw()
    {
        if (currentWeapon == 4)
        {
            if (!isThrow1)
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
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Fists_punch_1"))
                {
                    wepon.damage = fistSlashDamage;
                    wepon.isSlash = false;
                }
                else
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
        RpcSlashStart();
    }

    [ClientRpc]
    void RpcSlashStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        if (currentWeapon < 4)
        {
            wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            wepon.audioSourceSlash.PlayOneShot(slashClip);
        }
        else if (currentWeapon == 4)
        {
            MeleeWeapon wepon2 = weapons[9].GetComponent<MeleeWeapon>();
            wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            wepon2.audioSourceSlash.PlayOneShot(slashClip);

            wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            wepon.audioSourceSlash.PlayOneShot(slashClip);
        }
        else if (currentWeapon == 8)
        {
            if (anim != null && anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Fists_punch_1"))
            {
                wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon.audioSourceSlash.PlayOneShot(slashClip);
            }
            else if (anim != null)
            {
                MeleeWeapon wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(slashClip);
            }
        }
    }

    void BerserkerSlashEnd()
    {
        if (isLocalPlayer)
            CmdSlashEnd();
    }

    [Command]
    void CmdSlashEnd()
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
        RpcStabStart();
    }

    [ClientRpc]
    void RpcStabStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        if (currentWeapon < 4)
        {
            wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            wepon.audioSourceSlash.PlayOneShot(stabClip);
        }
        else if (currentWeapon == 4)
        {
            MeleeWeapon wepon2 = weapons[9].GetComponent<MeleeWeapon>();
            wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            wepon2.audioSourceSlash.PlayOneShot(stabClip);

            wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            wepon.audioSourceSlash.PlayOneShot(stabClip);
        }
        else if (currentWeapon == 8)
        {
            if (anim != null && anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Fists_punch_1"))
            {
                wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon.audioSourceSlash.PlayOneShot(stabClip);
            }
            else if (anim != null)
            {
                MeleeWeapon wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(stabClip);
            }
        }
    }


    void BerserkerStabEnd()
    {
        if (isLocalPlayer)
            CmdStabEnd();
    }

    [Command]
    void CmdStabEnd()
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
        RpcOverheadStart();
    }

    [ClientRpc]
    void RpcOverheadStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        if (currentWeapon < 4)
        {
            wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            wepon.audioSourceSlash.PlayOneShot(overheadClip);
        }
        else if (currentWeapon == 4)
        {
            MeleeWeapon wepon2 = weapons[9].GetComponent<MeleeWeapon>();
            wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            wepon2.audioSourceSlash.PlayOneShot(overheadClip);

            wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            wepon.audioSourceSlash.PlayOneShot(overheadClip);
        }
        else if (currentWeapon == 8)
        {
            if (anim != null && anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Fists_punch_1"))
            {
                wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon.audioSourceSlash.PlayOneShot(overheadClip);
            }
            else if (anim != null)
            {
                MeleeWeapon wepon2 = weapons[10].GetComponent<MeleeWeapon>();
                wepon2.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                wepon2.audioSourceSlash.PlayOneShot(overheadClip);
            }
        }
    }

    void BerserkerOverheadEnd()
    {
        if (isLocalPlayer)
            CmdOverheadEnd();
    }

    [Command]
    void CmdOverheadEnd()
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
                        SetAmmoText(--secondaryWeaponAmmo);
                        CmdDeactivateFood();
                    }
                    CmdNom();
                    energySlider.value += 0.5f;
                    break;
                case 7:
                    if (instance == 1)
                    {
                        SetAmmoText(--secondaryWeaponAmmo);
                        CmdDeactivateFood();
                    }
                    CmdNom();
                    health.Heal((int)peanutHealTotal / 2);
                    break;
            }
        }
    }
    //Farmed Locally, Very Locally

    [Command]
    void CmdDeactivateFood()
    {
        if (isServer)
        {
            RpcDeactivateFood();
        }
    }

    [ClientRpc]
    void RpcDeactivateFood()
    {
        weapons[currentWeapon].SetActive(false);
    }

    [Command]
    void CmdNom()
    {
        RpcNom();
    }

    [ClientRpc]
    void RpcNom()
    {
        audioSource.PlayOneShot(nomClip);
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
        if (currentWeapon == 5)
        {
            weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
            weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(throwClip);
        }
        else
        {
            if (isThrow1)
            {
                projectileModels[0].GetComponent<MeleeWeapon>().audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                projectileModels[0].GetComponent<MeleeWeapon>().audioSourceSlash.PlayOneShot(throwClip);
            }
            else if (!isThrow1)
            {
                projectileModels[1].GetComponent<MeleeWeapon>().audioSourceSlash.pitch = attackSpeeds[currentWeapon];
                projectileModels[1].GetComponent<MeleeWeapon>().audioSourceSlash.PlayOneShot(throwClip);
            }
        }
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
        if (currentWeapon == 4)
        {
            weapons[9].GetComponent<MeleeWeapon>().audioSourceDamage.PlayOneShot(blockClip);
        }
        else if (currentWeapon == 8)
        {
            weapons[10].GetComponent<MeleeWeapon>().audioSourceDamage.PlayOneShot(blockClip);
        }
        weapons[currentWeapon].GetComponent<MeleeWeapon>().audioSourceDamage.PlayOneShot(blockClip);
    }
}