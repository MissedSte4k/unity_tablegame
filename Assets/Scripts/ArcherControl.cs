using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArcherControl : NetworkBehaviour {

	private CharacterControl cc;
	private NetworkAnimator anim;

    // Use this for initialization
    void Start()
    {
        cc = GetComponent<CharacterControl>();
        anim = GetComponent<NetworkAnimator>();
    }

	void Update() {
		if (isLocalPlayer) {
                anim.animator.ResetTrigger("Stahp");
				// ataka vyksta kol laikomas nuspaustas mygtukas
				if (Input.GetButton ("Fire1")) {
					anim.animator.SetBool ("Attack1", true);
				} else {
					anim.animator.SetBool ("Attack1", false);
				}

					
				// blokas vykdomas tol, kol laikomas nuspaustas mygtukas
				if (Input.GetButtonDown ("Block")) {
					anim.SetTrigger ("Stahp");
				}
		}
	}
}