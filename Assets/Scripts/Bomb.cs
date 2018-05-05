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

    // Use this for initialization
    void Start()
    {
        GameObject obj = ClientScene.FindLocalObject(spawnedBy);
        Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        Quaternion angleRotation = Quaternion.Euler(spin * Time.deltaTime);
        rb.MoveRotation(rb.rotation * angleRotation);
    }

    void OnTriggerEnter(Collider other)
    {
       
    }


}
