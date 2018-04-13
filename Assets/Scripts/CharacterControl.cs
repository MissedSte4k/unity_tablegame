using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterControl : NetworkBehaviour {
	
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
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int bulletSpeed;
    private float mouseH = 0.0f;
    private float mouseV = 0.0f;   
    public Camera playerCamera;
    private int currentIncreaseTime;
    private int currentDecreaseTime;
	public float smoothing = 5f;
	NetworkAnimator anim;
	// dvi apatinės skeleto stuburo dalys, naudojamos žiūrėt aukštyn/žemyn
	public Transform spine;
	public Transform spine1;

    [SyncVar(hook = "OnCrouch")] bool isCrouched = false;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody> ();
        health = GetComponent<Health>();
        currentIncreaseTime = increaseTime;
		anim = GetComponent<NetworkAnimator> ();
	}

    void Update() {
        if (isLocalPlayer)
        {
    		anim.animator.ResetTrigger ("Attack2");
			anim.animator.ResetTrigger ("Attack3");
          
            anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
            anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));
                        
            transform.rotation = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, 0);
            
            mouseH += Input.GetAxis("Mouse X") * MenuSettings.instance.mouseSensitivity;
            mouseV -= Input.GetAxis("Mouse Y") * MenuSettings.instance.mouseSensitivity;

            playerCamera.transform.rotation = Quaternion.Euler(Mathf.Clamp(mouseV, -60, 60), mouseH, 0);

            if (Input.GetButtonDown("Crouch"))
            {
                CmdCrouch();
            }
				
            /*if (Input.GetButtonDown("Fire1"))
            {
                if (!health.ChangeStamina(-shootStaminaUse)) CmdFire();
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
                    if (health.ChangeStamina(-sprintStaminaUse)) onSprint = false;
                    currentDecreaseTime = decreaseTime;
                }
                else currentDecreaseTime--;
            }

			// ataka vyksta kol laikomas nuspaustas mygtukas
			if (Input.GetButton ("Fire1")) {
				anim.animator.SetBool ("Attack1", true);
			} else {
				anim.animator.SetBool ("Attack1", false);
			}

			// ataka vyksta 1 kartą nuspaudus mygtuką
			if(Input.GetButtonDown("Fire2"))
			{
				anim.SetTrigger ("Attack2");
				CmdFire();
			}

			// ataka vyksta 1 kartą nuspaudus mygtuką
			if(Input.GetButtonDown("Fire3"))
			{
				anim.SetTrigger ("Attack3");
				CmdFire();
			}

			// ataka vyksta 1 kartą nuspaudus mygtuką
			if(Input.GetButtonDown("Fire4"))
			{
				anim.SetTrigger ("Slam");
				CmdFire();
			}

			// blokas vykdomas tol, kol laikomas nuspaustas mygtukas
			if (Input.GetButton ("Block")) {
				anim.animator.SetBool ("Block", true);
			} else {
				anim.animator.SetBool ("Block", false);
			}

            if (!onSprint)
            {
                if (currentIncreaseTime < 1)
                {
                    health.ChangeStamina(1);
                    currentIncreaseTime = increaseTime;
                }
                else currentIncreaseTime--;
            }

            if (Input.GetButtonDown("Jump") && onGround)
            {
                //rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                if (!health.ChangeStamina(-jumpStaminaUse))
                {
                    rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
                }
            }

            Physics.SyncTransforms();
        }
        else playerCamera.enabled = false;
    }

	void LateUpdate(){
		if (isLocalPlayer) {
			// vykdoma modelio stuburo rotacija aukštyn/žemyn, kad atrodytu lyg veikėjas ten žiūri
			spine.rotation = Quaternion.Euler (spine.rotation.eulerAngles.x, spine.rotation.eulerAngles.y, spine.rotation.eulerAngles.z - Mathf.Clamp (mouseV, -60, 60) / 2); 
			spine1.rotation = Quaternion.Euler (spine.rotation.eulerAngles.x, spine.rotation.eulerAngles.y, spine.rotation.eulerAngles.z - Mathf.Clamp (mouseV, -60, 60) / 2);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
        if (isLocalPlayer)
        {          
            float moveVertical = Input.GetAxis("Vertical");
            float moveHorizontal = Input.GetAxis("Horizontal");            
            float speed = moveSpeed;
            if (onSprint) speed = moveSpeed + sprintSpeedBoost;
            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * speed * moveVertical;
            Vector3 horizontal = new Vector3(transform.right.x, 0, transform.right.z).normalized * speed * moveHorizontal;

            //Vector3 forward = transform.forward * moveSpeed * moveVertical;
            //Vector3 horizontal = transform.right * moveSpeed * moveHorizontal;

            rb.velocity = forward + horizontal + new Vector3(0, rb.velocity.y, 0);
            if(rb.velocity.y > jumpHeight)
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
        }
	}

    private bool isStanding()
    {
        if (rb.velocity.x == 0 && rb.velocity.z == 0) return true;
        else return false;
    }

	private void OnCollisionEnter (Collision collision) {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = true;
				anim.animator.SetBool ("Jump", false);
				anim.animator.SetBool ("Falling", false);
            }
        }
	}

	private void OnCollisionExit (Collision collision) {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = false;
				if (Input.GetButtonDown ("Jump")) {
					anim.animator.SetBool ("Jump", true);
				}
				anim.animator.SetBool ("Falling", true);
            }
        }
	}

	private void OnCollisionStay (Collision collision) {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = true;
				anim.animator.SetBool ("Jump", false);
				anim.animator.SetBool ("Falling", false);
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
    private void CmdCrouch()
    {
        isCrouched = !isCrouched;
    }

    void OnCrouch(bool value)
    {
        hitBox = GetComponent<CapsuleCollider>();
        anim = GetComponent<NetworkAnimator>();
        if (value)
        {
            hitBox.height = Mathf.Lerp(hitBox.height, 1.5f, 0.5f);
            hitBox.center = new Vector3(0, Mathf.Lerp(hitBox.center.y, 0.70f, 0.5f), 0);
            moveSpeed -= crouchSpeedReduction;
            anim.animator.SetBool("Crouched", true);

            //hitBox.height = Mathf.Lerp(hitBox.height, 1.5f, Time.deltaTime * 5);
            //hitBox.center = new Vector3(0, Mathf.Lerp(hitBox.center.y, 0.25f, Time.deltaTime * 5), 0);
        }
        else
        {
            hitBox.height = Mathf.Lerp(hitBox.height, 2.2f, 0.5f);
            hitBox.center = new Vector3(0, Mathf.Lerp(hitBox.center.y, -0.35f, 0.5f), 0);
            moveSpeed += crouchSpeedReduction;
            anim.animator.SetBool("Crouched", false);

            //hitBox.height = Mathf.Lerp(hitBox.height, 2.2f, Time.deltaTime * 5);
            //hitBox.center = new Vector3(0, Mathf.Lerp(hitBox.center.y, 0.1f, Time.deltaTime * 5), 0);
        }
    }
}
