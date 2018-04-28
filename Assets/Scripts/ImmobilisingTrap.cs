using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImmobilisingTrap : NetworkBehaviour {

    public float stopTime;
    public float lifetime;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        RaycastHit hit2;
        BoxCollider collider = GetComponent<BoxCollider>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (hit.CompareTag("Ground"))
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
            rb.isKinematic = true;
            collider.isTrigger = true;
            if (Physics.Raycast(transform.position, Vector3.down, out hit2))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - hit2.distance, transform.position.z);
            }
            StartCoroutine(DestroyCount(lifetime));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody hitrb = other.GetComponent<Rigidbody>();
        if (other.CompareTag("Player"))
        {
            hitrb.gameObject.transform.position = transform.position;
            hitrb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            StartCoroutine(Stop(stopTime, hitrb));
        }
    }

    IEnumerator Stop(float time, Rigidbody rb)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        rb.constraints = RigidbodyConstraints.None;
    }

    IEnumerator DestroyCount(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
