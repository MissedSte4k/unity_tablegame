using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeleeWeapon : NetworkBehaviour
{

    public int damage;
    public bool isSlash; //marks whether the weapon can damage more than 1 character.
    private Collider[] collisions = new Collider[5];
    private int n = 0;
    private bool isClear = true;
    private Collider col;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((GetComponentInParent<NetworkAnimator>().animator.GetCurrentAnimatorStateInfo(1).IsName("Running") ||
            GetComponentInParent<NetworkAnimator>().animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
            && !isClear)
        {
            Debug.Log("FOREACH");
            for (int i = 0; i < 5; i++)
            {
                if (collisions[i] != null) Physics.IgnoreCollision(col, collisions[i], false);
            }
            Array.Clear(collisions, 0, 5);
            n = 0;
            isClear = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var health = other.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.RpcTakeDamage(damage, health.IsFatal(damage));
            }

            if (!isSlash)
            {
                col.enabled = false;
            }
            else
            {
                collisions[n] = other;
                Physics.IgnoreCollision(col, collisions[n], false);
                n++;
                isClear = false;
            }
        }
        else if (other.gameObject.CompareTag("Block"))
        {
            col.enabled = false;
            GetComponentInParent<NetworkAnimator>().SetTrigger("Stop");
        }
    }
}
