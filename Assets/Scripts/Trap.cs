using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class Trap : NetworkBehaviour {

    [Header("Trap models")]
    public GameObject[] models;

    [Header("Particles used by explosive trap")]
    public GameObject explosionParticles;

    [Header("Explosion damage and radius")]
    [Range(0, 200)]
    public int explosionDamage;
    [Range(0, 50)]
    public float explosionRadius;

    [Header("Duration that the immobilizing trap stops you")]
    [Range(0, 10)]
    public float stopTime;

    [Header("Audio sources and sounds")]
    public AudioSource audioSourceTrigger;
    public AudioSource audioSourceClank;

    private Rigidbody hitrb;
    [SyncVar] private int activeTrap;
    [SyncVar] [HideInInspector] public bool isExplosive;
    [SyncVar] [HideInInspector] public NetworkInstanceId spawnedBy;
    [SyncVar] [HideInInspector] public int team;

    // Use this for initialization
    void Start()
    {
        if (isExplosive)
        {
            models[0].SetActive(true);
            activeTrap = 0;
        }
        else
        {
            if (team == 1)
            {
                models[1].SetActive(true);
                activeTrap = 1;
            }
            else
            {
                models[2].SetActive(true);
                activeTrap = 2;
            }
        }
        GameObject obj = ClientScene.FindLocalObject(spawnedBy);
        Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());
    }

    // Update is called once per frame

    void OnCollisionEnter(Collision collision)
    {
        audioSourceClank.Play();

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
        if (other.CompareTag("Player") && other.GetComponent<CharacterControl>().Team() != team)
        {
            audioSourceClank.Play();

            if (isExplosive)
            {
                ParticleSystem exp = explosionParticles.GetComponent<ParticleSystem>();
                exp.Play();
                models[activeTrap].SetActive(false);
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
                            hit.GetComponent<Health>().RpcTakeDamage(damage, hit.GetComponent<Health>().IsFatal(damage), hit.GetComponent<CharacterControl>().Team());
                    }
                }
                Destroy(gameObject, exp.main.duration);

            }
            else
            {
                hitrb = other.GetComponent<Rigidbody>();
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
