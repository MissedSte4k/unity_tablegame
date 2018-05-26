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
    [SyncVar]
    public int bombType = 0;
    [SyncVar]
    public int team;
    public GameObject explosionParticles;
    public GameObject smokeParticles;
    public int explosionDamage;
    [SyncVar]
    private int activeBomb;
    public float flashDuration;

    // Use this for initialization
    void Start()
    {
        switch (bombType)
        {
            case 0:
                if (team == 1)
                {
                    models[1].SetActive(true);
                    activeBomb = 1;
                }
                else
                {
                    models[0].SetActive(true);
                    activeBomb = 0;
                }
                break;
            case 1:
                if (team == 1)
                {
                    models[3].SetActive(true);
                    activeBomb = 3;
                }
                else
                {
                    models[2].SetActive(true);
                    activeBomb = 2;
                }
                break;
            case 2:
                if (team == 1)
                {
                    models[5].SetActive(true);
                    activeBomb = 5;
                }
                else
                {
                    models[4].SetActive(true);
                    activeBomb = 4;
                }
                break;
        }
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
        GameObject obj = ClientScene.FindLocalObject(spawnedBy);
        Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>(), false);
        if (bombType == 0) //flashbang
        {
            CharacterControl[] C = FindObjectsOfType<CharacterControl>();
            foreach (CharacterControl c in C)
            {
                c.Blind(transform.position, flashDuration);
            }
            Destroy(gameObject);
        }
        else if (bombType == 1) //smokebomb
        {
            ParticleSystem exp = smokeParticles.GetComponent<ParticleSystem>();
            exp.Play();
            Destroy(gameObject, exp.main.duration);
        }
        else if (bombType == 2) //grenade
        {
            ParticleSystem exp = explosionParticles.GetComponent<ParticleSystem>();
            exp.Play();
            models[activeBomb].SetActive(false);
            Vector3 explosionPosition = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
            foreach (Collider hit in colliders)
            {
                if (hit.CompareTag("Player"))
                {
                    Vector3 closestPoint = hit.ClosestPoint(explosionPosition);
                    float distance = Vector3.Distance(closestPoint, explosionPosition);

                    int damage = Convert.ToInt32((1 - Mathf.Clamp01(distance / explosionRadius)) * explosionDamage);
                    if (isServer)
                        hit.GetComponent<Health>().TakeDamage(damage);
                }
            }
            Destroy(gameObject, exp.main.duration);
        }
    }

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        Boom();
    }
}
