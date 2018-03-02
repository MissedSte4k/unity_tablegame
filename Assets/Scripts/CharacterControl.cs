using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterControl : NetworkBehaviour {
	
	public float moveSpeed;
	public float crouchSpeedReduction;
	public float jumpHeight;
	private Rigidbody rb;
	private bool isCrouched = false;
    private bool isJumped = false;
	private bool onGround = true;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int bulletSpeed;
    public Text healthText;
    private float mouseH = 0.0f;
    private float mouseV = 0.0f;
    public float mouseSensitivity;
    public Camera playerCamera;


    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	void Update() {
        if (isLocalPlayer)
        {
            Health health = GetComponent<Health>();
            healthText.text = "Health: " + health.CurrentHealth();
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

            if(Input.GetButtonDown("Fire1"))
            {
                CmdFire();
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

            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * moveSpeed * moveVertical;
            Vector3 horizontal = new Vector3(transform.right.x, 0, transform.right.z).normalized * moveSpeed * moveHorizontal;

            //Vector3 forward = transform.forward * moveSpeed * moveVertical;
            //Vector3 horizontal = transform.right * moveSpeed * moveHorizontal;

            rb.velocity = forward + horizontal + new Vector3(0, rb.velocity.y, 0);
            if(rb.velocity.y > jumpHeight)
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;

            if (Input.GetButtonDown("Jump") && onGround)
            {
                //rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
            }
        }
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
