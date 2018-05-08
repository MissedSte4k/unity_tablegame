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
    public Vector3 spin;
    public bool likeARecordBaby;

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
        if (!likeARecordBaby)
        {
            Quaternion rotation = transform.rotation;
            rotation.SetLookRotation(rb.velocity);
            transform.rotation = rotation;
        } else
        {
            Quaternion angleRotation = Quaternion.Euler(spin * Time.deltaTime);
            rb.MoveRotation(rb.rotation * angleRotation);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            collision.gameObject.GetComponent<NetworkAnimator>().SetTrigger("Hurt");
        }
        else if (collision.gameObject.CompareTag("Block"))
        {
            collision.gameObject.GetComponentInParent<NetworkAnimator>().SetTrigger("Block hurt");
        }

        Destroy(gameObject);
    }
}
