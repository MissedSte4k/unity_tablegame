using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {

    public int damage;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Quaternion rotation = transform.rotation;
        rotation.SetLookRotation(rb.velocity);
        transform.rotation = rotation;
	}

    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        
        var health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.CmdTakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
