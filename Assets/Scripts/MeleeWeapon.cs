using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeleeWeapon : NetworkBehaviour {

    public int damage;
    public bool isSlash; //marks whether the weapon can damage more than 1 character.

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var health = other.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.CmdTakeDamage(damage);
            }
            if (!isSlash)
                GetComponent<Collider>().enabled = false;
        }
    }
}
