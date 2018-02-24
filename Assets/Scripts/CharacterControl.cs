using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterControl : NetworkBehaviour {
	
	public float moveSpeed;
	public float crouchSpeedReduction;
	public float mouseSpeed;
	public float jumpHeight;
	public GameObject playerCamera;
	private Rigidbody rb;
	private bool isCrouched = false;
	private bool onGround = true;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	void Update() {
        if (isLocalPlayer)
        {
            transform.rotation = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, 0);
            
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
	}

	// Update is called once per frame
	void FixedUpdate () {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 forward = transform.forward * moveSpeed * moveVertical;
            Vector3 horizontal = transform.right * moveSpeed * moveHorizontal;

            rb.velocity = forward + horizontal + new Vector3(0, rb.velocity.y, 0);

            if (Input.GetButtonDown("Jump") && onGround)
            {
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
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
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}
