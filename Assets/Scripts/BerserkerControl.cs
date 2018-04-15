using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BerserkerControl : NetworkBehaviour {

	private float mouseV = 0.0f;
	public int primaryWeapon;
	public int secondaryWeapon;
	public GameObject[] weapons;
	public string[] weaponNames;
	private int currentWeapon;
	private CharacterControl cc;
	private bool isAttacking;
	bool isThrow1 = true;

	NetworkAnimator anim;
	// dvi apatinės skeleto stuburo dalys, naudojamos žiūrėt aukštyn/žemyn

	// Use this for initialization
	void Start() {
		cc = GetComponent<CharacterControl> ();
		anim = GetComponent<NetworkAnimator> ();
		if (primaryWeapon == 4) {
			weapons [9].SetActive (true);
		}
		weapons [primaryWeapon].SetActive (true);
		anim.animator.SetBool (weaponNames [primaryWeapon], true);
		currentWeapon = primaryWeapon;
	}

	void Update() {
		if (isLocalPlayer) {
			anim.animator.ResetTrigger("Attack2");
			anim.animator.ResetTrigger("Attack3");
			anim.animator.ResetTrigger("Throw1");
			anim.animator.ResetTrigger("Throw2");

			mouseV -= Input.GetAxis("Mouse Y") * cc.mouseSensitivity;

			if (mouseV > 60) {
				mouseV = 60;
			} else if (mouseV < -60) {
				mouseV = -60;
			}

			if (Input.GetButtonDown ("Swap")) {
				SwapWeapons ();
			}

			switch (currentWeapon){
			case 0:
				if (Input.GetButton ("Fire1")) {
					anim.animator.SetBool ("Attack1", true);
				} else {
					anim.animator.SetBool ("Attack1", false);
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire2")) {
					anim.SetTrigger ("Attack2");
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire3")) {
					anim.SetTrigger ("Attack3");
				}

				if (Input.GetButton ("Block")) {
					anim.animator.SetBool ("Block", true);
				} else {
					anim.animator.SetBool ("Block", false);
				}
				break;
			case 1:
				if (Input.GetButton ("Fire1")) {
					anim.animator.SetBool ("Attack1", true);
				} else {
					anim.animator.SetBool ("Attack1", false);
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire2")) {
					anim.SetTrigger ("Attack2");
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire3")) {
					anim.SetTrigger ("Attack3");
				}

				if (Input.GetButton ("Block")) {
					anim.animator.SetBool ("Block", true);
				} else {
					anim.animator.SetBool ("Block", false);
				}
				break;
			case 2:
				if (Input.GetButton ("Fire1")) {
					anim.animator.SetBool ("Attack1", true);
				} else {
					anim.animator.SetBool ("Attack1", false);
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire2")) {
					anim.SetTrigger ("Attack2");
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire3")) {
					anim.SetTrigger ("Attack3");
				}

				if (Input.GetButton ("Block")) {
					anim.animator.SetBool ("Block", true);
				} else {
					anim.animator.SetBool ("Block", false);
				}
				break;
			case 3:
				if (Input.GetButton ("Fire1")) {
					anim.animator.SetBool ("Attack1", true);
				} else {
					anim.animator.SetBool ("Attack1", false);
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire2")) {
					anim.SetTrigger ("Attack2");
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire3")) {
					anim.SetTrigger ("Attack3");
				}

				if (Input.GetButton ("Block")) {
					anim.animator.SetBool ("Block", true);
				} else {
					anim.animator.SetBool ("Block", false);
				}
				break;
			case 4:
				if (Input.GetButton ("Fire1")) {
					anim.animator.SetBool ("Attack1", true);
				} else {
					anim.animator.SetBool ("Attack1", false);
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire2")) {
					anim.SetTrigger ("Attack2");
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire3")) {
					anim.SetTrigger ("Attack3");
				}

				if (Input.GetButtonDown ("Fire4")) {
					if (isThrow1){
						anim.SetTrigger ("Throw1");
						isThrow1 = false;
					} else {
						anim.SetTrigger ("Throw2");
						isThrow1 = true;
					}
				}

				if (Input.GetButton ("Block")) {
					anim.animator.SetBool ("Block", true);
				} else {
					anim.animator.SetBool ("Block", false);
				}
				break;
			case 5:
				if (Input.GetButtonDown ("Fire1")) {
					anim.SetTrigger ("Attack3");
				}
				break;
			case 6:
				if (Input.GetButtonDown ("Fire1")) {
					anim.SetTrigger ("Attack3");
				}
				break;
			case 7:
				if (Input.GetButtonDown ("Fire1")) {
					anim.SetTrigger ("Attack3");
				}
				break;
			case 8:
				if (Input.GetButton ("Fire1")) {
					anim.animator.SetBool ("Attack1", true);
				} else {
					anim.animator.SetBool ("Attack1", false);
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire2")) {
					anim.SetTrigger ("Attack2");
				}

				// ataka vyksta 1 kartą nuspaudus mygtuką
				if (Input.GetButtonDown ("Fire3")) {
					anim.SetTrigger ("Attack3");
				}

				if (Input.GetButton ("Block")) {
					anim.animator.SetBool ("Block", true);
				} else {
					anim.animator.SetBool ("Block", false);
				}
				break;
			}
		}
	}

	public void SwapWeapons(){
		if (anim.animator.GetCurrentAnimatorStateInfo (1).IsName ("Running") || anim.animator.GetCurrentAnimatorStateInfo (1).IsName ("Crouched")) {
			if (currentWeapon == primaryWeapon) {
				if (primaryWeapon == 4) {
					weapons [9].SetActive (false);
				}
				if (secondaryWeapon == 8) {
					weapons [10].SetActive (true);
				}
				weapons [primaryWeapon].SetActive (false);
				weapons [secondaryWeapon].SetActive (true);
				anim.animator.SetBool (weaponNames [primaryWeapon], false);
				anim.animator.SetBool (weaponNames [secondaryWeapon], true);
				currentWeapon = secondaryWeapon;
			} else {
				if (primaryWeapon == 4) {
					weapons [9].SetActive (true);
				}
				if (secondaryWeapon == 8) {
					weapons [10].SetActive (false);
				}
				weapons [primaryWeapon].SetActive (true);
				weapons [secondaryWeapon].SetActive (false);
				anim.animator.SetBool (weaponNames [secondaryWeapon], false);
				anim.animator.SetBool (weaponNames [primaryWeapon], true);
				currentWeapon = primaryWeapon;
			}
		}
	}
}