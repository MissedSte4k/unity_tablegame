using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterControl : NetworkBehaviour {

	public float mouseSensitivity = MenuSettings.instance.mouseSensitivity;
    public float moveSpeed;
    public float crouchSpeedReduction;
    public float sprintSpeedBoost;
    public int increaseTime;
    public int decreaseTime;
    public int sprintStaminaUse;
    public int jumpStaminaUse;
    public int shootStaminaUse;
    public float jumpHeight;
    private Rigidbody rb;
    private Health health;
    private CapsuleCollider hitBox;
    private bool onGround = true;
    private bool onSprint = false;
    private bool isCrouched = false;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int bulletSpeed;
    private float mouseH = 0.0f;
    private float mouseV = 0.0f;   
    public Camera playerCamera;
    private int currentIncreaseTime;
    private int currentDecreaseTime;
    private float normalHeight = 2.2f;
    private float crouchHeight = 1.5f;
    private float normalCenter = 0.1f;
    private float crouchCenter = 0.25f;
    private float minHeightChangeSpeed = 0.01f;
    private float minCenterChangeSpeed = 0.01f;
    NetworkAnimator anim;
    // dvi apatinės skeleto stuburo dalys, naudojamos žiūrėt aukštyn/žemyn
    public Transform spine;

    [SyncVar(hook = "OnHeightChanged")] float height = 2.2f;
    [SyncVar(hook = "OnCenterChanged")] float center = 0.1f;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        currentIncreaseTime = increaseTime;
        currentDecreaseTime = decreaseTime;
        anim = GetComponent<NetworkAnimator>();
        health = GetComponent<Health>();
    }

    void Update() {
        if (isLocalPlayer)
        {
            anim.animator.ResetTrigger("Attack2");
            anim.animator.ResetTrigger("Attack3");

            anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
            anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));

            transform.rotation = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, 0);
            
			mouseH += Input.GetAxis("Mouse X") * mouseSensitivity;
			mouseV -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            playerCamera.transform.rotation = Quaternion.Euler(Mathf.Clamp(mouseV, -60, 60), mouseH, 0);

            if (Input.GetButtonDown("Crouch"))
            {
                if (!isCrouched)
                {
                    isCrouched = true;
                    moveSpeed -= crouchSpeedReduction;
                    anim.animator.SetBool("Crouched", true);
                    CmdCrouch();
                }
                else
                {
                    isCrouched = false;
                    moveSpeed += crouchSpeedReduction;
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

            if (Input.GetButtonDown("Sprint") && !isStanding() && onGround && !isCrouched)
            {
                onSprint = true;
            }

            if (Input.GetButtonUp("Sprint") || isStanding() || !onGround || isCrouched)
            {
                onSprint = false;
            }

            if (onSprint)
            {
                if (currentDecreaseTime < 1)
                {
                    CmdChangeStamina(-sprintStaminaUse);
                    if (health.isStaminaZero()) onSprint = false;
                    currentDecreaseTime = decreaseTime;
                }
                else currentDecreaseTime--;
            }

            // ataka vyksta kol laikomas nuspaustas mygtukas
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
                CmdFire();
            }

            // ataka vyksta 1 kartą nuspaudus mygtuką
            if (Input.GetButtonDown("Fire3"))
            {
                anim.SetTrigger("Attack3");
                CmdFire();
            }

            // ataka vyksta 1 kartą nuspaudus mygtuką
            if (Input.GetButtonDown("Fire4"))
            {
                anim.SetTrigger("Slam");
                CmdFire();
            }

            // blokas vykdomas tol, kol laikomas nuspaustas mygtukas
            if (Input.GetButton("Block"))
            {
                anim.animator.SetBool("Block", true);
            }
            else
            {
                anim.animator.SetBool("Block", false);
            }

            if (!onSprint)
            {
                if (currentIncreaseTime < 1 && !health.isStaminaMax())
                {
                    CmdChangeStamina(1);
                    currentIncreaseTime = increaseTime;
                }
                else currentIncreaseTime--;
            }

            if (Input.GetButtonDown("Jump") && onGround)
            {
                //rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                if (!health.isStaminaZero(-jumpStaminaUse))
                {
					anim.animator.SetBool("Jump", true);
					anim.animator.SetBool("Falling", true);
                    CmdChangeStamina(-jumpStaminaUse);
                    rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
                }
            }
            Physics.SyncTransforms();
        }
        else playerCamera.enabled = false;
    }

    void LateUpdate() {
        if (isLocalPlayer) {
            // vykdoma modelio stuburo rotacija aukštyn/žemyn, kad atrodytu lyg veikėjas ten žiūri
			spine.localRotation = Quaternion.Euler(spine.localRotation.eulerAngles.x, spine.localRotation.eulerAngles.y, spine.localRotation.eulerAngles.z - Mathf.Clamp(mouseV, -60, 60));
		}
	}

    // Update is called once per frame
    void FixedUpdate() {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            float speed = moveSpeed;
            if (onSprint) speed = moveSpeed + sprintSpeedBoost;
            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * speed * moveVertical;
            Vector3 horizontal = new Vector3(transform.right.x, 0, transform.right.z).normalized * speed * moveHorizontal;

            //Vector3 forward = transform.forward * moveSpeed * moveVertical;
            //Vector3 horizontal = transform.right * moveSpeed * moveHorizontal;

            rb.velocity = forward + horizontal + new Vector3(0, rb.velocity.y, 0);
            if (rb.velocity.y > jumpHeight)
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
        }
    }

    private bool isStanding()
    {
        if (rb.velocity.x == 0 && rb.velocity.z == 0) return true;
        else return false;
    }

    private void OnCollisionEnter(Collision collision) {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = true;
                anim.animator.SetBool("Jump", false);
                anim.animator.SetBool("Falling", false);
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = false;
                anim.animator.SetBool("Falling", true);
            }
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = true;
                anim.animator.SetBool("Jump", false);
                anim.animator.SetBool("Falling", false);
            }
        }
    }

    [Command]
    private void CmdFire()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

    [Command]
    private void CmdChangeStamina(int value)
    {
        ChangeStamina(value);
    }

    [Server]
    private void ChangeStamina(int value)
    {
        health = GetComponent<Health>();
        health.ChangeStamina(value);
    }

    [Command]
    private void CmdCrouch()
    {
        float h = Mathf.Max(Mathf.Min(Mathf.Lerp(height, crouchHeight, Time.deltaTime * 5), height - minHeightChangeSpeed), crouchHeight);
        float c = Mathf.Min(Mathf.Max(Mathf.Lerp(center, crouchCenter, Time.deltaTime * 5), center + minCenterChangeSpeed), crouchCenter);
        CrouchUncrouch(h, c);
    }

    [Command]
    private void CmdUncrouch()
    {
        float h = Mathf.Min(Mathf.Max(Mathf.Lerp(height, normalHeight, Time.deltaTime * 5), height + minHeightChangeSpeed), normalHeight);
        float c = Mathf.Max(Mathf.Min(Mathf.Lerp(center, normalCenter, Time.deltaTime * 5), center - minCenterChangeSpeed), normalCenter);
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
}
