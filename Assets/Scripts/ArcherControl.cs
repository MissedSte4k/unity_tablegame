using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArcherControl : NetworkBehaviour
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
    public GameObject[] back;

    [Header("BulletSpawn in camera and CharacterControl script")]
    public Transform bulletSpawn;

    [Header("Ranged attack projectile speeds")]
    [Range(0, 200)]
    public int longbowArrowSpeed;
    [Range(0, 200)]
    public int shortbowArrowSpeed;
    [Range(0, 200)]
    public int lightCrossbowArrowSpeed;
    [Range(0, 200)]
    public int heavyCrossbowArrowSpeed;
    [Range(0, 200)]
    public int javelinSpeed;
    [Range(0, 200)]
    public int trapSpeed;

    [Header("Ranged attack projectile damage")]
    [Range(0, 200)]
    public int longbowDamage;
    [Range(0, 200)]
    public int shortbowDamage;
    [Range(0, 200)]
    public int lightCrossbowDamage;
    [Range(0, 200)]
    public int heavyCrossbowDamage;
    [Range(0, 200)]
    public int javelinDamage;

    [Header("Melee weapon slash damage")]
    [Range(0, 200)]
    public int javelinSlashDamage;
    [Range(0, 200)]
    public int handaxeSlashDamage;
    [Range(0, 200)]
    public int shortswordSlashDamage;
    [Range(0, 200)]
    public int daggerSlashDamage;

    [Header("Melee weapon overhead swing damage")]
    [Range(0, 200)]
    public int javelinOverheadDamage;
    [Range(0, 200)]
    public int handaxeOverheadDamage;
    [Range(0, 200)]
    public int shortswordOverheadDamage;
    [Range(0, 200)]
    public int daggerOverheadDamage;

    [Header("Melee weapon stab damage")]
    [Range(0, 200)]
    public int javelinStabDamage;
    [Range(0, 200)]
    public int handaxeStabDamage;
    [Range(0, 200)]
    public int shortswordStabDamage;
    [Range(0, 200)]
    public int daggerStabDamage;

    [Header("Multipliers for movement speed")]
    [Range(0, 1)]
    public float shortbowShootSpeedMultiplier;
    [Range(0, 1)]
    public float longbowShootSpeedMultiplier;
    [Range(0, 1)]
    public float lightCrossbowShootSpeedMultiplier;
    [Range(0, 1)]
    public float heavyCrossbowShootSpeedMultiplier;
    [Range(0, 1)]
    public float javelinThrowSpeedMultiplier;
    [Range(0, 1)]
    public float trapThrowSpeedMultiplier;
    [Range(0, 1)]
    public float trapReloadSpeedMultiplier;
    [Range(0, 1)]
    public float weaponSwapSpeedMultiplier;

    [Header("Misc stamina uses")]
    [Range(0, 100)]
    public int javelinThrowStaminaUse;
    [Range(0, 100)]
    public int blockStaminaUse;

    [Header("Match lighting settings")]
    [Range(0, 60)]
    public float matchLightDuration;
    [Range(0, 5)]
    public float litDamageMultiplier;

    [Header("Match particles")]
    public GameObject fire;
    public GameObject sparks;

    [Header("UI elements and GameObjects")]
    public Text ammoText;
    public Slider matchSlider;
    public GameObject fireSprite;

    [Header("Audio sources and sounds")]
    public AudioSource audioSource;
    public AudioClip slashClip;
    public AudioClip stabClip;
    public AudioClip overheadClip;
    public AudioClip throwClip;
    public AudioClip bowShootClip;
    public AudioClip crossbowShootClip;
    public AudioClip arrowPlaceClip;
    public AudioClip bowPullClip;
    public AudioClip blockClip;
    public AudioClip drawPrimary;
    public AudioClip drawSecondary;
    public AudioClip drawAmmo;

    [SyncVar] private int currentWeapon;
    private NetworkAnimator anim;

    [HideInInspector] [SyncVar]
    public int primaryWeaponAmmo;
    [HideInInspector] [SyncVar]
    public int secondaryWeaponAmmo;

    private CharacterControl characterControl;
    private Health health;
    private float remainingDuration;
    [SyncVar] private bool isLitFam;
    private bool isShooting = false;

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
        else secondaryWeapon = 5;
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
        isLitFam = false;

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
            anim.animator.ResetTrigger("Light");
            anim.animator.ResetTrigger("Throw");
            anim.animator.ResetTrigger("Stahp");
        }

        if (isLitFam)
        {
            if (remainingDuration < 0 || isShooting)
            {
                isLitFam = false;
                FireEnd();
            }
            else if (remainingDuration >= 0)
            {
                remainingDuration -= Time.deltaTime;
                matchSlider.value = remainingDuration / matchLightDuration;
            }
        }

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Draw"))
        {
            characterControl.moveSpeed *= weaponSwapSpeedMultiplier;
        }

        if (characterControl.onSprint)
            return;

        if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
        {
            if (!isLitFam)
            {
                //if (Input.GetButtonDown("Swap") && (secondaryWeaponAmmo > 0 || maxAmmo[secondaryWeapon] == 0) && currentWeapon != 4)
                if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(SwapWeapon)"]) && (secondaryWeaponAmmo > 0 || maxAmmo[secondaryWeapon] == 0) && currentWeapon != 4)
                {
                    if (currentWeapon == primaryWeapon) CmdDraw(1);
                    else CmdDraw(0);
                    if (currentWeapon < 5)
                    {
                        anim.animator.SetBool("OutOfTraps", false);
                    }
                    SwapWeapons();
                    if ((currentWeapon == 2 || currentWeapon == 3) && primaryWeaponAmmo > 0)
                    {
                        anim.animator.SetBool("Empty", false);
                    }
                }
                if (primaryWeaponAmmo > 0 && primaryWeapon == 4 && matchSlider.value < 1)
                {
                    matchSlider.value = 1;
                }
            }
            if (isShooting)
                isShooting = false;
        }

        switch (currentWeapon)
        {
            case 0:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_draw") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_idle") ||
                    anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_shoot") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_attack_ammo") ||
                    anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Bow_attack_empty"))
                {
                    characterControl.moveSpeed *= longbowShootSpeedMultiplier;
                }
                if (primaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetBool("Empty") == true)
                    {
                        anim.animator.SetBool("Empty", false);
                        CmdActivateWeapon(currentWeapon);
                    }
                    anim.animator.SetBool("Empty", false);
                    //if (Input.GetButton("Fire1"))
                    if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]))
                    {
                        anim.animator.SetBool("Attack1", true);
                        if (primaryWeaponAmmo <= 1)
                        {
                            anim.animator.SetBool("Empty", true);
                        }
                    }
                    else
                    {
                        anim.animator.SetBool("Attack1", false);
                    }

                    //if (Input.GetButtonDown("Block"))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
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
                    characterControl.moveSpeed *= shortbowShootSpeedMultiplier;
                }
                if (primaryWeaponAmmo > 0)
                {
                    if (anim.animator.GetBool("Empty") == true)
                    {
                        anim.animator.SetBool("Empty", false);
                        CmdActivateWeapon(currentWeapon);
                    }
                    //if (Input.GetButton("Fire1"))
                    if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]))
                    {
                        anim.animator.SetBool("Attack1", true);
                        if (primaryWeaponAmmo <= 1)
                        {
                            anim.animator.SetBool("Empty", true);
                        }
                    }
                    else
                    {
                        anim.animator.SetBool("Attack1", false);
                    }

                    //if (Input.GetButtonDown("Block"))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Stahp");
                    }
                }
                else anim.animator.SetBool("Empty", true);
                break;
            case 2:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Shoot"))
                {
                    characterControl.moveSpeed *= heavyCrossbowShootSpeedMultiplier;
                }
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (primaryWeaponAmmo > 0)
                    {
                        //if (Input.GetButtonDown("Fire1") && anim.animator.GetBool("Empty") == false)
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && anim.animator.GetBool("Empty") == false)
                        {
                            anim.animator.SetBool("Empty", true);
                            anim.SetTrigger("Attack3");
                        }

                        //if (Input.GetButtonDown("Reload") && anim.animator.GetBool("Empty") == true)
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Reload)"]) && anim.animator.GetBool("Empty") == true)
                        {
                            anim.animator.SetBool("Empty", false);
                            anim.SetTrigger("Reload");

                        }
                    }
                }
                break;
            case 3:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Shoot"))
                {
                    characterControl.moveSpeed *= lightCrossbowShootSpeedMultiplier;
                }
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (primaryWeaponAmmo > 0)
                    {
                        //if (Input.GetButtonDown("Fire1") && anim.animator.GetBool("Empty") == false)
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]) && anim.animator.GetBool("Empty") == false)
                        {
                            anim.animator.SetBool("Empty", true);
                            anim.SetTrigger("Attack3");
                        }

                        //if (Input.GetButtonDown("Reload") && anim.animator.GetBool("Empty") == true)
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Reload)"]) && anim.animator.GetBool("Empty") == true)
                        {
                            anim.animator.SetBool("Empty", false);
                            anim.SetTrigger("Reload");
                        }
                    }
                }
                break;
            case 4:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed *= javelinThrowSpeedMultiplier;
                }

                //if (Input.GetButton("Block"))
                if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                {
                    anim.animator.SetBool("Block", true);
                    CmdBlockStart();
                }
                else
                {
                    anim.animator.SetBool("Block", false);
                    CmdBlockEnd();
                }

                if (primaryWeaponAmmo > 0)
                {
                    if (weapons[currentWeapon].activeInHierarchy == false && !isShooting)
                    {
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
                        //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                        {
                            anim.SetTrigger("Attack2");
                        }

                        //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                        {
                            anim.SetTrigger("Attack3");
                        }

                        //if (Input.GetButtonDown("Fire4") && !health.IsStaminaZero(javelinThrowStaminaUse))
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack4)"]) && !health.IsStaminaZero(javelinThrowStaminaUse))
                        {
                            anim.SetTrigger("Throw");
                        }

                        //if (Input.GetButtonDown("Reload"))
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Reload)"]))
                        {
                            if (!isLitFam)
                            {
                                anim.SetTrigger("Light");
                                CmdJavelinDamageUp();
                            }
                        }
                    }
                }
                break;
            case 5:
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
                    //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    //if (Input.GetButtonDown("Block"))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 6:
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
                    //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    //if (Input.GetButtonDown("Block"))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 7:
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
                    //if (Input.GetButtonDown("Fire2") && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack2)"]) && !health.IsStaminaZero(-overheadStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack2");
                    }

                    //if (Input.GetButtonDown("Fire3") && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack3)"]) && !health.IsStaminaZero(-stabStaminaUse[currentWeapon]))
                    {
                        anim.SetTrigger("Attack3");
                    }

                    //if (Input.GetButtonDown("Block"))
                    if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Block)"]))
                    {
                        anim.SetTrigger("Parry");
                        CmdBlockStart();
                    }
                }
                break;
            case 8:
                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
                {
                    characterControl.moveSpeed *= trapThrowSpeedMultiplier;
                }

                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (secondaryWeaponAmmo > 0)
                    {
                        //if (Input.GetButtonDown("Fire1"))
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]))
                        {
                            anim.SetTrigger("Attack3");
                        }
                    }
                    else
                    {
                        anim.animator.SetBool("OutOfTraps", true);
                        if (currentWeapon == primaryWeapon) CmdDraw(1);
                        else CmdDraw(0);
                        SwapWeapons();
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
                    characterControl.moveSpeed *= trapThrowSpeedMultiplier;
                }

                if (anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Running") || anim.animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
                {
                    if (secondaryWeaponAmmo > 0)
                    {
                        //if (Input.GetButtonDown("Fire1"))
                        if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Attack1)"]))
                        {
                            anim.SetTrigger("Attack3");
                        }
                    }
                    else
                    {
                        anim.animator.SetBool("OutOfTraps", true);
                        if (currentWeapon == primaryWeapon) CmdDraw(1);
                        else CmdDraw(0);
                        SwapWeapons();
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
            if (isLocalPlayer)
            {
                anim.animator.SetBool(weaponNames[primaryWeapon], false);
                anim.animator.SetBool(weaponNames[secondaryWeapon], true);
                anim.animator.SetFloat("Attack speed", attackSpeeds[secondaryWeapon]);
            }
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
            if (isLocalPlayer)
            {
                anim.animator.SetBool(weaponNames[secondaryWeapon], false);
                anim.animator.SetBool(weaponNames[primaryWeapon], true);
                anim.animator.SetFloat("Attack speed", attackSpeeds[primaryWeapon]);
            }
            if (primaryWeaponAmmo == 0)
                projectileModels[primaryWeapon].SetActive(false);
            else if (primaryWeaponAmmo > 0)
                projectileModels[currentWeapon].SetActive(true);
        }

        if (primaryWeapon < 4)
        {
            back[0].SetActive(true);
        }
        else
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

    void BowArrowTake()
    {
        if (isLocalPlayer && primaryWeaponAmmo > 0)
        {
            CmdDraw(2);
            CmdBowArrowTake();
        }
    }

    [Command]
    void CmdBowArrowTake()
    {
        if (isServer)
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
        AudioSource[] audioSources = weapons[currentWeapon].GetComponents<AudioSource>();
        if (currentWeapon == 0 || currentWeapon == 1)
        {
            audioSources[0].pitch = 1;
            audioSources[0].PlayOneShot(arrowPlaceClip);
            audioSources[1].pitch = attackSpeeds[currentWeapon];
            audioSources[1].PlayOneShot(bowPullClip);

        }
        else if (currentWeapon == 2 || currentWeapon == 3)
        {
            audioSources[0].pitch = 1;
            audioSources[0].PlayOneShot(bowPullClip);
        }

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
        }
        else
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
        RpcDraw(2);
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
        if (isLocalPlayer)
        {
            isShooting = true;
            if (currentWeapon < 5)
                SetAmmoText(--primaryWeaponAmmo);
            else SetAmmoText(--secondaryWeaponAmmo);
            if (currentWeapon == 9)
                health.CmdChangeStamina(-javelinThrowStaminaUse);
        }

        CmdArcherShoot(projPrefab);
    }

    [Command]
    void CmdArcherShoot(GameObject projPrefab)
    {
        if (isServer)
            RpcArcherShoot();

        switch (currentWeapon)
        {
            case 0:
                bulletSpawn.localPosition = new Vector3(0.372f, 0.005f, 0.797f);
                break;
            case 1:
                bulletSpawn.localPosition = new Vector3(0.391f, 0.005f, 0.779f);
                break;
            case 2:
                bulletSpawn.localPosition = new Vector3(0.541f, -0.272f, 1.397f);
                break;
            case 3:
                bulletSpawn.localPosition = new Vector3(0.535f, -0.329f, 1.144f);
                break;
            case 4:
                bulletSpawn.localPosition = new Vector3(0.559f, 0.399f, 0.037f);
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
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
                break;
            case 1:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * shortbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = shortbowDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
                break;
            case 2:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * heavyCrossbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = heavyCrossbowDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
                break;
            case 3:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * lightCrossbowArrowSpeed;
                proj.GetComponent<Projectile>().damage = lightCrossbowDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
                break;
            case 4:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * javelinSpeed;
                proj.GetComponent<Projectile>().damage = javelinDamage;
                proj.GetComponent<Projectile>().spawnedBy = netId;
                proj.GetComponent<Projectile>().team = GetComponent<CharacterControl>().Team();
                if (isLitFam)
                {
                    proj.GetComponent<Projectile>().isOnFire = true;
                }
                break;
            case 8:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * trapSpeed;
                proj.GetComponent<Trap>().isExplosive = false;
                proj.GetComponent<Trap>().spawnedBy = netId;
                proj.GetComponent<Trap>().team = GetComponent<CharacterControl>().Team();
                break;
            case 9:
                proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * trapSpeed;
                proj.GetComponent<Trap>().isExplosive = true;
                proj.GetComponent<Trap>().spawnedBy = netId;
                proj.GetComponent<Trap>().team = GetComponent<CharacterControl>().Team();
                break;
        }
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
        CmdJavelinDamageDown();
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
        if (isLocalPlayer)
            SetAmmoText(--primaryWeaponAmmo);
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
        if (isLocalPlayer)
        {
            health.CmdChangeStamina(-slashStaminaUse[currentWeapon]);
            CmdSlashStart();
        }
    }

    [Command]
    void CmdSlashStart()
    {
        var wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.collidersActive = true;
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
        RpcSlashStart();
    }

    [ClientRpc]
    void RpcSlashStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(slashClip);
    }

    void ArcherSlashEnd()
    {
        if (isLocalPlayer)
            CmdSlashEnd();
    }

    [Command]
    void CmdSlashEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
    }

    void ArcherStabStart()
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
        var wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.collidersActive = true;
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
        RpcStabStart();
    }

    [ClientRpc]
    void RpcStabStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(stabClip);
    }

    void ArcherStabEnd()
    {
        if (isLocalPlayer)
            CmdStabEnd();
    }

    [Command]
    void CmdStabEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
    }

    void ArcherOverheadStart()
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
        var wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.collidersActive = true;
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
        RpcOverheadStart();
    }

    [ClientRpc]
    void RpcOverheadStart()
    {
        MeleeWeapon wepon = weapons[currentWeapon].GetComponent<MeleeWeapon>();
        wepon.audioSourceSlash.pitch = attackSpeeds[currentWeapon];
        wepon.audioSourceSlash.PlayOneShot(overheadClip);
    }

    void ArcherOverheadEnd()
    {
        if (isLocalPlayer)
            CmdOverheadEnd();
    }

    [Command]
    void CmdOverheadEnd()
    {
        weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
    }

    [Command]
    void CmdBlockStart()
    {
        if (!isServer)
            return;

        if (currentWeapon == 4)
        {
            weapons[10].GetComponent<Collider>().enabled = true;
        }
        else
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

        if (currentWeapon == 4)
        {
            weapons[10].GetComponent<Collider>().enabled = false;
        }
        else
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().collidersActive = false;
            weapons[currentWeapon].tag = "Weapon";
            weapons[currentWeapon].GetComponent<MeleeWeapon>().isTrigger = true;
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

    [Command]
    void CmdJavelinDamageUp()
    {
        javelinDamage = (int)(javelinDamage * litDamageMultiplier);
        javelinOverheadDamage = (int)(javelinOverheadDamage * litDamageMultiplier);
        javelinSlashDamage = (int)(javelinSlashDamage * litDamageMultiplier);
        javelinStabDamage = (int)(javelinStabDamage * litDamageMultiplier);
    }

    [Command]
    void CmdJavelinDamageDown()
    {
        javelinDamage = (int)(javelinDamage / litDamageMultiplier);
        javelinOverheadDamage = (int)(javelinOverheadDamage / litDamageMultiplier);
        javelinSlashDamage = (int)(javelinSlashDamage / litDamageMultiplier);
        javelinStabDamage = (int)(javelinStabDamage / litDamageMultiplier);
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
        if (currentWeapon <= 1)
        {
            weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
            weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(bowShootClip);
        } else if (currentWeapon <= 3)
        {
            weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
            weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(crossbowShootClip);
        } else if (currentWeapon == 4)
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().audioSourceSlash.pitch = attackSpeeds[currentWeapon];
            weapons[currentWeapon].GetComponent<MeleeWeapon>().audioSourceSlash.PlayOneShot(throwClip);
        } else
        {
            weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
            weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(throwClip);
        }
    }

    void BowPullSound()
    {
        if (isLocalPlayer)
            CmdBowPullSound();
    }

    [Command]
    void CmdBowPullSound()
    {
        RpcBowPullSound();
    }

    [ClientRpc]
    void RpcBowPullSound()
    {
        weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
        weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(bowPullClip);
    }

    void ClickSound()
    {
        if (isLocalPlayer)
            CmdClickSound();
    }

    [Command]
    void CmdClickSound()
    {
        RpcClickSound();
    }

    [ClientRpc]
    void RpcClickSound()
    {

        weapons[currentWeapon].GetComponent<AudioSource>().pitch = attackSpeeds[currentWeapon];
        weapons[currentWeapon].GetComponent<AudioSource>().PlayOneShot(arrowPlaceClip);
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
            weapons[10].GetComponent<AudioSource>().PlayOneShot(blockClip);
        }
        else
        {
            weapons[currentWeapon].GetComponent<MeleeWeapon>().audioSourceDamage.PlayOneShot(blockClip);
        }
    }
}