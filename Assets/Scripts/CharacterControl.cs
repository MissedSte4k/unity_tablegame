using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {
	
	public float moveSpeed;
	public float crouchSpeedReduction;
	public float mouseSpeed;
	public float jumpHeight;
	public GameObject playerCamera;
	private Rigidbody rb;
	private bool isCrouched = false;
	private bool onGround = true;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	void Update() {
		
		transform.rotation = Quaternion.Euler (0, playerCamera.transform.rotation.eulerAngles.y, 0);

		if (Input.GetButtonDown ("Crouch")) {
			if (isCrouched == false) {
				isCrouched = true;
				transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * 0.5f, transform.localScale.z);
				transform.position = new Vector3 (transform.position.x, transform.localScale.y * 0.5f, transform.position.z);
				moveSpeed -= crouchSpeedReduction;
			} else {
				isCrouched = false;
				transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
				transform.position = new Vector3 (transform.position.x, transform.localScale.y * 0.5f, transform.position.z);
				moveSpeed += crouchSpeedReduction;
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () {

		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 forward = transform.forward * moveSpeed * moveVertical;
		Vector3 horizontal = transform.right * moveSpeed * moveHorizontal;

		rb.velocity = forward + horizontal + new Vector3 (0, rb.velocity.y, 0);

		if (Input.GetButtonDown ("Jump") && onGround) {
			rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
		}
			
	}

	private void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.CompareTag ("Ground")) {
			onGround = true;
		}
	}

	private void OnCollisionExit (Collision collision) {
		if (collision.gameObject.CompareTag ("Ground")) {
			onGround = false;
		}
	}

	private void OnCollisionStay (Collision collision) {
		if (collision.gameObject.CompareTag ("Ground")) {
			onGround = true;
		}
	}
}
