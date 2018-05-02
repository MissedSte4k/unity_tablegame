using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {

    public int damage;
    private Rigidbody rb;
    public GameObject fire;
    [SyncVar]
    public bool isOnFire = false;
    [SyncVar]
    public NetworkInstanceId spawnedBy;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        if (isOnFire)
        {
            fire.SetActive(true);
        }
        GameObject obj = ClientScene.FindLocalObject(spawnedBy);
        Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Quaternion rotation = transform.rotation;
        rotation.SetLookRotation(rb.velocity);
        transform.rotation = rotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        var health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
