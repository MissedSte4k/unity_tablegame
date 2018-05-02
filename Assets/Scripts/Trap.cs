using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class Trap : NetworkBehaviour {

    public float stopTime;
    public float explosionRadius;
    public float explosionPower;
    public int explosionDamage;
    [SyncVar]
    public bool isExplosive;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

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
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isExplosive)
        {
            if (other.CompareTag("Player"))
            {
                Vector3 explosionPosition = transform.position;
                Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
                foreach (Collider hit in colliders)
                {
                    if (hit.CompareTag("Player"))
                    {
                        hit.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, explosionPosition, explosionRadius, 3.0f);
                        Vector3 closestPoint = hit.ClosestPoint(transform.position);
                        float distance = Vector3.Distance(closestPoint, transform.position);

                        int damage = Convert.ToInt32((1 - Mathf.Clamp01(distance / explosionRadius)) * explosionDamage);
                        hit.GetComponent<Health>().TakeDamage(damage);
                    }
                }
                Destroy(gameObject);
            }
        }
        else
        {
            Rigidbody hitrb = other.GetComponent<Rigidbody>();
            if (other.CompareTag("Player"))
            {
                hitrb.gameObject.transform.position = transform.position;
                hitrb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                StartCoroutine(Stop(stopTime, hitrb));
            }
        }
    }

    IEnumerator Stop(float time, Rigidbody rb)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        rb.constraints = RigidbodyConstraints.None;
    }
}
