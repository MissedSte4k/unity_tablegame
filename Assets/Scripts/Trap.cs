using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class Trap : NetworkBehaviour {

    public float stopTime;
    public float explosionRadius;
    public float explosionPower;
    public GameObject[] models;
    public int explosionDamage;
    [SyncVar]
    public bool isExplosive;
    [SyncVar]
    public NetworkInstanceId spawnedBy;
    private Rigidbody hitrb;

    // Use this for initialization
    void Start()
    {
        if (isExplosive)
        {
            models[0].SetActive(true);
        } else
        {
            models[2].SetActive(true);
        }
        GameObject obj = ClientScene.FindLocalObject(spawnedBy);
        Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());
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
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
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
                Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
                foreach (Collider hit in colliders)
                {
                    if (hit.CompareTag("Player"))
                    {
                        hit.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, explosionPosition, explosionRadius, 3.0f);
                        Vector3 closestPoint = hit.ClosestPoint(explosionPosition);
                        float distance = Vector3.Distance(closestPoint, explosionPosition);

                        int damage = Convert.ToInt32((1 - Mathf.Clamp01(distance / explosionRadius)) * explosionDamage);
                        hit.GetComponent<Health>().TakeDamage(damage);
                        other.GetComponent<NetworkAnimator>().SetTrigger("Hurt");
                    }
                }
                Destroy(gameObject);
            }
        }
        else
        {
            hitrb = other.GetComponent<Rigidbody>();
            if (other.CompareTag("Player"))
            {
                hitrb.gameObject.transform.position = transform.position;
                hitrb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                StartCoroutine(Stop(stopTime));
            }
        }
    }

    IEnumerator Stop(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        hitrb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        RpcNoConstraints();
    }

    [ClientRpc]
    void RpcNoConstraints()
    {
        hitrb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
}
