using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class KnightControl : NetworkBehaviour {

	public int primaryWeapon;
	public int secondaryWeapon;
	public GameObject[] weapons;
	public string[] weaponNames;
	private int currentWeapon;
	private CharacterControl cc;
	private bool isAttacking;

	NetworkAnimator anim;

	// Use this for initialization
	void Start() {
		cc = GetComponent<CharacterControl> ();
		anim = GetComponent<NetworkAnimator> ();
		if (primaryWeapon <= 2) {
			weapons [10].SetActive (true);
		}
		weapons [primaryWeapon].SetActive (true);
		anim.animator.SetBool (weaponNames [primaryWeapon], true);
		currentWeapon = primaryWeapon;
	}

	void Update() {
		if (isLocalPlayer) {
			anim.animator.ResetTrigger("Attack2");
			anim.animator.ResetTrigger("Attack3");

			if (Input.GetButtonDown ("Swap")) {
				SwapWeapons ();
			}

			if (currentWeapon <= 5) {
				// ataka vyksta kol laikomas nuspaustas mygtukas
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
					
				// blokas vykdomas tol, kol laikomas nuspaustas mygtukas
				if (Input.GetButton ("Block")) {
					anim.animator.SetBool ("Block", true);
				} else {
					anim.animator.SetBool ("Block", false);
				}
			} else if (currentWeapon == 9){
				if (Input.GetButtonDown ("Fire1")) {
					anim.SetTrigger ("Attack3");
				}
				if (Input.GetButton ("Block")) {
					anim.animator.SetBool ("Block", true);
				} else {
					anim.animator.SetBool ("Block", false);
				}
			}else {
				if (Input.GetButtonDown ("Fire1")) {
					anim.SetTrigger ("Attack3");
				}
			}
		}
	}

	public void SwapWeapons(){
		if (anim.animator.GetCurrentAnimatorStateInfo (1).IsName ("Running") || anim.animator.GetCurrentAnimatorStateInfo (1).IsName ("Crouched")) {
			if (currentWeapon == primaryWeapon) {
				if (primaryWeapon <= 2) {
					weapons [10].SetActive (false);
				}
				if (secondaryWeapon == 6) {
					weapons [11].SetActive (true);
				}
				weapons [primaryWeapon].SetActive (false);
				weapons [secondaryWeapon].SetActive (true);
				anim.animator.SetBool (weaponNames [primaryWeapon], false);
				anim.animator.SetBool (weaponNames [secondaryWeapon], true);
				currentWeapon = secondaryWeapon;
			} else {
				if (primaryWeapon <= 2) {
					weapons [10].SetActive (true);
				}
				if (secondaryWeapon == 6) {
					weapons [11].SetActive (false);
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