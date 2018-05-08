using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class Bomb : NetworkBehaviour {

    public float explosionRadius;
    public GameObject[] models;
    [SyncVar]
    public NetworkInstanceId spawnedBy;
    private Rigidbody hitrb;
    public Vector3 spin;
    private Rigidbody rb;
    public float fuseTime;
    private bool isCollided = false;
    public int bombType = 0;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        models[bombType * 2].SetActive(true);
        GameObject obj = ClientScene.FindLocalObject(spawnedBy);
        Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());
        StartCoroutine(Delay(fuseTime));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       if (!isCollided)
       {
            Quaternion angleRotation = Quaternion.Euler(spin * Time.deltaTime);
            rb.MoveRotation(rb.rotation * angleRotation);
       }
    }

    void OnCollisionEnter()
    {
        isCollided = true;
    }

    void Boom()
    {
        //flashbang
        if (bombType == 0)
        {
            Vector3 explosionPosition = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
            foreach (Collider hit in colliders)
            {
                if (hit.CompareTag("Player"))
                {
                    Vector3 closestPoint = hit.ClosestPoint(explosionPosition);
                    float distance = Vector3.Distance(closestPoint, explosionPosition);

                    int damage = Convert.ToInt32((1 - Mathf.Clamp01(distance / explosionRadius)));
                    hit.GetComponent<Health>().TakeDamage(damage);
                }
            }
        }
        //smokebomb
        else if (bombType == 1)
        {
            Vector3 explosionPosition = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
            foreach (Collider hit in colliders)
            {
                if (hit.CompareTag("Player"))
                {
                    Vector3 closestPoint = hit.ClosestPoint(explosionPosition);
                    float distance = Vector3.Distance(closestPoint, explosionPosition);

                    int damage = Convert.ToInt32((1 - Mathf.Clamp01(distance / explosionRadius)));
                    hit.GetComponent<Health>().TakeDamage(damage);
                }
            }
        }
        //grenade
        else if (bombType == 2)
        {
            Vector3 explosionPosition = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
            foreach (Collider hit in colliders)
            {
                if (hit.CompareTag("Player"))
                {
                    Vector3 closestPoint = hit.ClosestPoint(explosionPosition);
                    float distance = Vector3.Distance(closestPoint, explosionPosition);

                    int damage = Convert.ToInt32((1 - Mathf.Clamp01(distance / explosionRadius)));
                    hit.GetComponent<Health>().TakeDamage(damage);
                    hit.GetComponent<NetworkAnimator>().SetTrigger("Hurt");
                }
            }
        }
        Destroy(gameObject);
    }

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        Boom();
    }
}
