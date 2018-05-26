﻿using System.Collections;
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
    [SyncVar]
    public int team;

    // Use this for initialization
    void Start () {
        if (team == 1)
            GetComponent<TrailRenderer>().startColor = new Color(0, 0, 255);
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
                    if (isServer)
                        health.TakeDamage(damage);
                }
            }
            else if (collision.gameObject.CompareTag("Block"))
            {
                collision.gameObject.GetComponentInParent<CharacterControl>().RpcBlockHurt();
            }

            Destroy(gameObject);
        }
}
