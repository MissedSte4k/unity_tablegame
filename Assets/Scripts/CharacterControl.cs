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
	private bool isCrouched = false;
    private bool isJumped = false;
	private bool onGround = true;
    private bool onSprint = false;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int bulletSpeed;
    private float mouseH = 0.0f;
    private float mouseV = 0.0f;
    public float mouseSensitivity;
    public Camera playerCamera;
    private int currentIncreaseTime;
    private int currentDecreaseTime;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody> ();
        health = GetComponent<Health>();
        currentIncreaseTime = increaseTime;
	}

	void Update() {
        if (isLocalPlayer)
        {
            //Health health = GetComponent<Health>();
            //healthText.text = "Health: " + health.CurrentHealth();
            transform.rotation = Quaternion.Euler(playerCamera.transform.rotation.eulerAngles.x, playerCamera.transform.rotation.eulerAngles.y, playerCamera.transform.rotation.eulerAngles.z);

            mouseH += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseV -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            playerCamera.transform.rotation = Quaternion.Euler(Mathf.Clamp(mouseV, -90, 90), mouseH, 0);

            if (Input.GetButtonDown("Crouch"))
            {
                if (isCrouched == false)
                {
                    isCrouched = true;
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.5f, transform.localScale.z);
                    transform.position = new Vector3(transform.position.x, transform.localScale.y * 0.5f, transform.position.z);
                    moveSpeed -= crouchSpeedReduction;
                }
                else
                {
                    isCrouched = false;
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
                    transform.position = new Vector3(transform.position.x, transform.localScale.y * 0.5f, transform.position.z);
                    moveSpeed += crouchSpeedReduction;
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (!health.ChangeStamina(-shootStaminaUse)) CmdFire();
            }

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
        }
        else playerCamera.enabled = false;
    }

	// Update is called once per frame
	void FixedUpdate () {
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
            if(rb.velocity.y > jumpHeight)
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;

            /*if (Input.GetButtonDown("Jump") && onGround)
            {
                //rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                if (!health.ChangeStamina(-jumpStaminaUse))
                {
                    rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
                }
            }*/
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
            }
        }
	}

	private void OnCollisionExit (Collision collision) {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = false;
            }
        }
	}

	private void OnCollisionStay (Collision collision) {
        if (isLocalPlayer)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                onGround = true;
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
}
