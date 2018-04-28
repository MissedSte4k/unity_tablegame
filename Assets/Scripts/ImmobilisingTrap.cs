using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmobilisingTrap : MonoBehaviour {

    public int stopTime;
    public int lifetime;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;

        if (hit.CompareTag("Ground"))
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }

        if (hit.CompareTag("Player"))
        {
            hit.GetComponent<Rigidbody>().isKinematic = true;
            hit.GetComponent<Rigidbody>().velocity = Vector3.zero;
            StartCoroutine(Stop(stopTime));
            Destroy(gameObject);
        }

        StartCoroutine(Stop(lifetime));
    }

    IEnumerator Stop(int time)
    {
        yield return new WaitForSeconds(time);
    }
}
