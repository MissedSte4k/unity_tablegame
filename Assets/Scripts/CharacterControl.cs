﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterControl : NetworkBehaviour
{
    [Header("Default movement speed, reference speed used for adjusting animation speeds")]
    [Range(0, 200)]
    public float defaultMoveSpeed;
    [Range(64, 64)]
    public float referenceMoveSpeed;
    [HideInInspector] public float moveSpeed;

    [Header("Speed multipliers when crouching and sprinting")]
    [Range(0, 1)]
    public float crouchSpeedMultiplier;
    [Range(1, 3)]
    public float sprintSpeedMultiplier;

    [Header("Time intervals for stamina recharging by 1, for stamina decreasing when sprinting")]
    [Range(0, 1)]
    public float increaseTime;
    [Range(0, 1)]
    public float decreaseTime;
    private float currentIncreaseTime;
    private float currentDecreaseTime;

    [Header("Stamina used when sprinting every decreaseTime seconds, delay after hitting 0 stamina")]
    [Range(0, 5)]
    public float sprintStaminaUse;
    [Range(0, 5)]
    public float sprintDelay;
    private float sprintDelayRemaining = 0;

    [Header("Stamina use for jumping, jump height")]
    [Range(0, 100)]
    public int jumpStaminaUse;
    [Range(0, 20)]
    public float jumpSpeed;

    [Header("Collider attributes for standing and crouching")]
    [SyncVar(hook = "OnHeightChanged")]
    public float height = 2.2f;
    [SyncVar(hook = "OnCenterChanged")] public float center = 0.1f;
    public float normalHeight = 2.2f;
    public float crouchHeight = 1.5f;
    public float normalCenter = 0.1f;
    public float crouchCenter = 0.25f;
    private float minHeightChangeSpeed = 0.001f;
    private float minCenterChangeSpeed = 0.0001f;

    [Header("Lock cursor to window")]
    public bool lockCursor;

    [Header("Player camera and UI elements")]
    public Camera playerCamera;
    public Image flashbangOverlay;
    private float flashDuration;
    private float flashRemaining;

    [Header("Audio sources and sounds")]
    public AudioSource audioSourceWalk;
    public AudioClip walkClip;
    public AudioClip runClip;
    public AudioSource audioSourceOther;
    public AudioClip jumpClip;
    public AudioClip landClip;

    [Header("Fall damage per 1 second of falling")]
    [Range(0, 100)]
    public int fallDamage;

    [HideInInspector] public float mouseSensitivity;
    private float moveHorizontal = 0;
    private float moveVertical = 0;
    private float mouseH = 0.0f;
    private float mouseV = 0.0f;
    private float prevMouseV;

    private NetworkAnimator anim;
    private Rigidbody rb;
    private Health health;
    private CapsuleCollider hitBox;
    private CapsuleCollider capsule;

    [HideInInspector] public bool onSprint = false;
    [HideInInspector] public bool isCrouched = false;
    private bool onGround = true;
    private bool isBlind = false;

    private int team;
    private int hasFlag = 0;
    private float airTime;

    float time = 0;

    // Use this for initialization
    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Application.targetFrameRate = -1;
        capsule = GetComponent<CapsuleCollider>();
        moveSpeed = defaultMoveSpeed;
        rb = GetComponent<Rigidbody>();
        currentIncreaseTime = increaseTime;
        currentDecreaseTime = decreaseTime;
        anim = GetComponent<NetworkAnimator>();
        health = GetComponent<Health>();
        minHeightChangeSpeed = 0.001f;
        minCenterChangeSpeed = 0.0001f;

        mouseSensitivity = MenuSettings.Instance.mouseSensitivity;
        playerCamera.fieldOfView = MenuSettings.Instance.fieldOfView;
        AudioListener.volume = MenuSettings.Instance.masterVolume;
        QualitySettings.SetQualityLevel(MenuSettings.Instance.activeGraphicsIndex);

        //CmdTeam();
        SetTeam();
    }

    private void OnEnable()
    {
        Spawn();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            Debug.Log(time += Time.deltaTime);
            moveVertical = 0;
            if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(MoveForward)"]) == true)
            {
                if (CanMove(transform.forward))
                    moveVertical = 1;
            }
            if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(MoveBackward)"]) == true)
            {
                if (CanMove(-transform.forward))
                    moveVertical = -1;
            }
            if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(MoveForward)"]) == true &&
                Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(MoveBackward)"]) == true)
            {
                moveVertical = 0;
            }

            moveHorizontal = 0;
            if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(MoveRight)"]) == true)
            {
                if (CanMove(transform.right))
                    moveHorizontal = 1;
            }
            if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(MoveLeft)"]) == true)
            {
                if (CanMove(-transform.right))
                    moveHorizontal = -1;
            }
            if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(MoveRight)"]) == true &&
                Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(MoveLeft)"]) == true)
            {
                moveHorizontal = 0;
            }

            if (isCrouched) moveSpeed *= crouchSpeedMultiplier;

            //anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
            //anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));

            prevMouseV = mouseV;

            mouseH += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseV -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            if (mouseV > 60)
            {
                mouseV = 60;
            }
            else if (mouseV < -60)
            {
                mouseV = -60;
            }

            transform.rotation = Quaternion.Euler(0, mouseH, 0);
            playerCamera.transform.rotation = Quaternion.Euler(mouseV, mouseH, 0);

            anim.animator.SetFloat("Look", (mouseV + 60) / 120);

            if (mouseV - prevMouseV != 0)
                CmdLook(mouseV);

            if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Crouch)"]))
            {
                if (!isCrouched)
                {
                    isCrouched = true;
                    anim.animator.SetBool("Crouched", true);
                    CmdCrouch();
                }
                else
                {
                    isCrouched = false;
                    anim.animator.SetBool("Crouched", false);
                    CmdUncrouch();
                }
            }

            if (height < normalHeight && height > crouchHeight || center > normalCenter && center < crouchCenter)
            {
                if (isCrouched) CmdCrouch();
                else CmdUncrouch();
            }

            /*if (Input.GetButtonDown("Fire1"))
            {
                if (!health.isStaminaZero(-shootStaminaUse)) CmdFire();
            }*/

            if (Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Sprint)"]) && !IsStanding() && onGround && !isCrouched && moveVertical > 0 && moveHorizontal == 0)
            {
                onSprint = true;
            }
            else
            {
                onSprint = false;
            }

            if (sprintDelayRemaining > 0)
                sprintDelayRemaining -= Time.deltaTime;

            if (onSprint)
            {
                if (currentDecreaseTime <= 0 && sprintDelayRemaining <= 0)
                {
                    health.CmdChangeStamina(-sprintStaminaUse);
                    currentDecreaseTime = decreaseTime;
                    if (health.IsStaminaZero())
                    {
                        onSprint = false;
                        sprintDelayRemaining = health.delayRemaining + sprintDelay;
                    }
                }
                else
                {
                    if (sprintDelayRemaining > 0)
                    {
                        onSprint = false;
                    }
                    if (currentDecreaseTime > 0)
                        currentDecreaseTime -= Time.deltaTime;
                }
            }

            if (!onSprint)
            {
                if (currentIncreaseTime <= 0 && !health.IsStaminaMax() && health.canRecharge)
                {
                    health.CmdChangeStamina(1);
                    currentIncreaseTime = increaseTime;
                }
                else
                {
                    if (currentIncreaseTime > 0)
                        currentIncreaseTime -= Time.deltaTime;
                }
            }

            if (!isCrouched)
            {
                float speed = moveSpeed * (onSprint ? sprintSpeedMultiplier : 1);
                if (speed < defaultMoveSpeed)
                {
                    anim.animator.SetFloat("Speed", moveVertical * speed / defaultMoveSpeed);
                    anim.animator.SetFloat("Strafe", moveHorizontal * speed / defaultMoveSpeed);
                    anim.animator.SetFloat("SpeedMultiplier", defaultMoveSpeed / referenceMoveSpeed);
                }
                else
                {
                    anim.animator.SetFloat("Speed", moveVertical);
                    anim.animator.SetFloat("Strafe", moveHorizontal);
                    anim.animator.SetFloat("SpeedMultiplier", speed / referenceMoveSpeed);
                }
            }
            else
            {
                float defaultCrouchedSpeed = defaultMoveSpeed * crouchSpeedMultiplier;
                if (moveSpeed < defaultCrouchedSpeed)
                {
                    anim.animator.SetFloat("Speed", moveVertical * moveSpeed / defaultCrouchedSpeed);
                    anim.animator.SetFloat("Strafe", moveHorizontal * moveSpeed / defaultCrouchedSpeed);
                    anim.animator.SetFloat("SpeedMultiplier", defaultMoveSpeed / referenceMoveSpeed);
                }
                else
                {
                    anim.animator.SetFloat("Speed", moveVertical);
                    anim.animator.SetFloat("Strafe", moveHorizontal);
                    anim.animator.SetFloat("SpeedMultiplier", moveSpeed / crouchSpeedMultiplier / referenceMoveSpeed);
                }
            }

            if (moveHorizontal != 0 || moveVertical != 0 && onGround)
            {
                float pitch;

                if (!isCrouched)
                {
                    pitch = 0.666f * moveSpeed * (onSprint ? sprintSpeedMultiplier : 1) / referenceMoveSpeed;
                }
                else
                {
                    pitch = 0.833f * moveSpeed / referenceMoveSpeed / crouchSpeedMultiplier;
                }

                if (audioSourceWalk.pitch != pitch || !audioSourceWalk.isPlaying)
                    CmdWalkAudio(pitch, true, isCrouched);
            }
            else
            {
                if (audioSourceWalk.isPlaying)
                    CmdWalkAudio(0, false, isCrouched);
            }

            if (!onGround && rb.velocity.y < 0)
            {
                airTime += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyBindManager.MyInstance.Keybinds["Button(Jump)"]) && onGround)
            {
                //rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                if (!health.IsStaminaZero(-jumpStaminaUse))
                {
                    anim.animator.SetBool("Jump", true);
                    anim.animator.SetBool("Falling", true);
                    health.CmdChangeStamina(-jumpStaminaUse);
                    rb.velocity += jumpSpeed * Vector3.up;
                    CmdOtherAudio(1);
                }
            }
            Physics.SyncTransforms();

            if (NetworkServer.active)
            {
                if (!anim.animator.GetCurrentAnimatorStateInfo(2).IsName("Default"))
                {
                    anim.animator.ResetTrigger("Hurt");
                    anim.animator.ResetTrigger("Block hurt");
                }
            }

            if (isBlind)
            {
                flashRemaining -= Time.deltaTime;
                if (flashRemaining <= 2)
                {
                    flashbangOverlay.color = new Color(255, 255, 255, Mathf.Clamp(flashRemaining, 0, 1));
                    Debug.Log(flashRemaining / flashDuration);
                    if (flashRemaining <= 0)
                    {
                        isBlind = false;
                    }
                }
            }
        }
        else playerCamera.enabled = false;
    }

    //void LateUpdate()
    //{
    //    spine.localRotation *= Quaternion.Euler(0, 0, -mouseV);
    //}


    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {

            //float moveHorizontal = Input.GetAxis("Horizontal");
            //float moveVertical = Input.GetAxis("Vertical");
            float speed = moveSpeed * (onSprint ? sprintSpeedMultiplier : 1);
            Vector3 moveDirection = (moveHorizontal * transform.right + moveVertical * transform.forward).normalized;

            //Vector3 forward = transform.forward * moveSpeed * moveVertical;
            //Vector3 horizontal = transform.right * moveSpeed * moveHorizontal;

            rb.velocity = moveDirection * speed * Time.deltaTime * 50 + rb.velocity.y * Vector3.up;
            if (rb.velocity.y > jumpSpeed)
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
        }
    }

    private bool CanMove(Vector3 direction)
    {
        if (!onGround)
        {
            float distanceToPoints = capsule.height / 2 - capsule.radius;
            Vector3 point1 = transform.position + capsule.center + Vector3.up * distanceToPoints;
            Vector3 point2 = point1;

            float radius = capsule.radius * 0.95f;
            float castDistance = 0.5f;

            RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, radius, direction, castDistance);

            foreach (RaycastHit objectHit in hits)
            {
                if (objectHit.transform.tag == "Ground")
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool IsStanding()
    {
        if (Mathf.Abs(rb.velocity.x) < 0.5 && Mathf.Abs(rb.velocity.z) < 0.5) return true;
        else return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                if (!onGround)
                {
                    CmdOtherAudio(0);
                }
                onGround = true;
                anim.animator.SetBool("Jump", false);
                anim.animator.SetBool("Falling", false);
                if ((int)((airTime - 1.05f) * fallDamage) > 0)
                {
                    health.TakeDamage(((int)((airTime - 0.8f) * fallDamage)));
                }
                airTime = 0;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = false;
                anim.animator.SetBool("Falling", true);
                if (rb.velocity.y < 0)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = true;
                anim.animator.SetBool("Jump", false);
                anim.animator.SetBool("Falling", false);
                if (!Input.GetKey(KeyBindManager.MyInstance.Keybinds["Button(Jump)"]))
                {
                    rb.AddForce(0, -3.3f * moveSpeed * (onSprint ? sprintSpeedMultiplier : 1), 0);
                }
            }
        }
    }

    [Command]
    void CmdLook(float mouseV)
    {
        RpcLook(mouseV);
    }

    [ClientRpc]
    void RpcLook(float mouseV)
    {
        if (!isLocalPlayer)
        {
            GetComponent<Animator>().SetFloat("Look", (mouseV + 60) / 120);
        }
    }

    //   [Command]
    //private void CmdFire()
    //{
    //	GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

    //	// Add velocity to the bullet
    //	bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

    //	NetworkServer.Spawn(bullet);

    //	// Destroy the bullet after 2 seconds
    //	Destroy(bullet, 2.0f);
    //}

    [Command]
    private void CmdCrouch()
    {
        float h = Mathf.Max(Mathf.Min(Mathf.Lerp(height, crouchHeight, Time.deltaTime * 5), height - minHeightChangeSpeed / Time.deltaTime), crouchHeight);
        float c = Mathf.Min(Mathf.Max(Mathf.Lerp(center, crouchCenter, Time.deltaTime * 5), center + minCenterChangeSpeed / Time.deltaTime), crouchCenter);
        CrouchUncrouch(h, c);
    }

    [Command]
    private void CmdUncrouch()
    {
        float h = Mathf.Min(Mathf.Max(Mathf.Lerp(height, normalHeight, Time.deltaTime * 5), height + minHeightChangeSpeed / (Time.deltaTime)), normalHeight);
        float c = Mathf.Max(Mathf.Min(Mathf.Lerp(center, normalCenter, Time.deltaTime * 5), center - minCenterChangeSpeed / (Time.deltaTime)), normalCenter);
        CrouchUncrouch(h, c);
    }

    [Server]
    private void CrouchUncrouch(float h, float c)
    {
        height = h;
        center = c;
    }

    void OnHeightChanged(float value)
    {
        height = value;
        hitBox = GetComponent<CapsuleCollider>();
        hitBox.height = value;
    }

    void OnCenterChanged(float value)
    {
        center = value;
        hitBox = GetComponent<CapsuleCollider>();
        hitBox.center = new Vector3(0, value, 0);
    }

    [ClientRpc]
    public void RpcHitBlock()
    {
        if (isLocalPlayer)
        {
            anim.SetTrigger("Hurt");
            anim.SetTrigger("Stop");
        }
    }

    [ClientRpc]
    public void RpcBlockHurt()
    {
        if (isLocalPlayer)
            anim.SetTrigger("Block hurt");
    }

    /*[Command]
    private void CmdTeam()
    {
        SetTeam();
    }

    [Server]
    private void SetTeam()
    {
        RpcSetTeam(FindObjectOfType<TeamControl>().Team());
    }

    [ClientRpc]
    private void RpcSetTeam(int team)
    {
        Team(team);
    }

    private void Team(int team)
    {
        this.team = team;
        health.SetTeamText(this.team);
        Spawn();
    }*/

    private void SetTeam()
    {
        switch (FindObjectOfType<PlayOptions>().teamIndex)
        {
            case 0:
                team = 1;
                break;
            case 1:
                team = 2;
                break;
            default:
                team = 0;
                break;
        }
        CmdSetTeam(team);
        health.SetTeamText(team);
        Spawn();
    }

    [Command]
    void CmdSetTeam(int t)
    {
        RpcSetTeam(t);
    }

    [ClientRpc]
    void RpcSetTeam(int t)
    {
        team = t;
    }

    [Command]
    void CmdWalkAudio(float pitch, bool active, bool crouch)
    {
        RpcWalkAudio(pitch, active, crouch);
    }

    [ClientRpc]
    void RpcWalkAudio(float pitch, bool active, bool crouch)
    {
        if (active)
        {
            if (crouch)
                audioSourceWalk.clip = walkClip;
            else
                audioSourceWalk.clip = runClip;

            audioSourceWalk.pitch = pitch;

            if (!audioSourceWalk.isPlaying)
                audioSourceWalk.Play();
        }
        else
        {
            audioSourceWalk.Stop();
        }
    }

    [Command]
    void CmdOtherAudio(int action)
    {
        RpcOtherAudio(action);
    }

    [ClientRpc]
    void RpcOtherAudio(int action)
    {
        switch (action)
        {
            case 0:
                audioSourceOther.pitch = 0.5f;
                audioSourceOther.PlayOneShot(landClip);
                break;
            case 1:
                audioSourceOther.pitch = 1;
                audioSourceOther.PlayOneShot(jumpClip);
                break;
        }
    }

    void MakeInvisible()
    {

    }

    public int Team()
    {
        return team;
    }

    private void Spawn()
    {
        GameObject spawnpointA = GameObject.Find("Spawn Point A1");
        GameObject spawnpointB = GameObject.Find("Spawn Point B1");
        if (team == 1) transform.position = PointAvalaible(spawnpointB.transform.position);
        else if (team == 2) transform.position = PointAvalaible(spawnpointA.transform.position);
    }

    private Vector3 PointAvalaible(Vector3 center)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 point = new Vector3(center.x + Random.Range(-3, 3), center.y, center.z + Random.Range(-3, 3));
            bool avalaible = true;
            foreach (CharacterControl cc in FindObjectsOfType<CharacterControl>())
            {
                Vector3 pos = cc.GetComponent<Transform>().transform.position;
                if (Mathf.Abs(pos.x - center.x) <= 0.5 && Mathf.Abs(pos.z - center.z) <= 0.5)
                {
                    avalaible = false;
                    break;
                }
            }
            if (avalaible) return point;
        }
        return center;
    }

    public void FlagGot()
    {
        hasFlag = 1;
    }

    public void FlagLost()
    {
        hasFlag = 0;
    }
    public void FlagCaptured()
    {
        hasFlag = 2;
    }
    public int FlagStatus()
    {
        return hasFlag;
    }

    public void Blind(Vector3 position, float duration)
    {
        Vector3 screenPoint = playerCamera.WorldToViewportPoint(position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        if (onScreen)
        {
            flashDuration = flashRemaining = duration;
            isBlind = true;
            flashbangOverlay.color = new Color(255, 255, 255, 255);
        }
    }
}