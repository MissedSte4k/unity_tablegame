using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArcherControl : NetworkBehaviour {

	private NetworkAnimator anim;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<NetworkAnimator>();
    }

	void Update() {
		if (isLocalPlayer) {
                anim.animator.ResetTrigger("Stahp");
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