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
	private CapsuleCollider hitBox;
	private bool isCrouched = false;
	private bool onGround = true;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int bulletSpeed;
    private float mouseH = 0.0f;
    private float mouseV = 0.0f;
    public float mouseSensitivity;
    public Camera playerCamera;
	public float smoothing = 5f;
	NetworkAnimator anim;
	// dvi apatinės skeleto stuburo dalys, naudojamos žiūrėt aukštyn/žemyn
	public Transform spine;
	public Transform spine1;


    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<NetworkAnimator> ();
		hitBox = GetComponent<CapsuleCollider> ();
	}

	void Update() {
        if (isLocalPlayer)
        {
			anim.animator.ResetTrigger ("Attack2");
			anim.animator.ResetTrigger ("Attack3");

			anim.animator.SetFloat ("Speed", Input.GetAxis ("Vertical"));
			anim.animator.SetFloat ("Strafe", Input.GetAxis ("Horizontal"));

            transform.rotation = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, 0);

            mouseH += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseV -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            playerCamera.transform.rotation = Quaternion.Euler(Mathf.Clamp(mouseV, -60, 60), mouseH, 0);


			if (isCrouched == false) {
				hitBox.height = Mathf.Lerp (hitBox.height, 2.2f, Time.deltaTime * 5);
				hitBox.center = new Vector3 (0, Mathf.Lerp(hitBox.center.y, 0.1f, Time.deltaTime * 5), 0);

			} else {
				hitBox.height = Mathf.Lerp (hitBox.height, 1.5f, Time.deltaTime * 5);
				hitBox.center = new Vector3 (0, Mathf.Lerp(hitBox.center.y, 0.25f, Time.deltaTime * 5), 0);
			}

            if (Input.GetButtonDown("Crouch"))
            {
                if (isCrouched == false)
                {
                    isCrouched = true;
                    moveSpeed -= crouchSpeedReduction;
					anim.animator.SetBool ("Crouched", true);
                }
                else
                {
                    isCrouched = false;
                    moveSpeed += crouchSpeedReduction;
					anim.animator.SetBool ("Crouched", false);
                }
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
}
