using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeleeWeapon : NetworkBehaviour
{

    public int damage;
    public bool isSlash; //marks whether the weapon can damage more than 1 character.
    public Collider[] weaponColliders;
    private Collider[] collisions = new Collider[5];
    private int n = 0;
    private bool isClear = true;
    public bool collidersActive = false;

    // Use this for initialization
    void Start()
    {
    }

    void Update()
    {
        if (collidersActive)
        {
            foreach (Collider c in weaponColliders)
            {
                c.enabled = true;
            }
        } else
        {
            foreach (Collider c in weaponColliders)
            {
                c.enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        if ((GetComponentInParent<NetworkAnimator>().animator.GetCurrentAnimatorStateInfo(1).IsName("Running") ||
            GetComponentInParent<NetworkAnimator>().animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
            && !isClear)
        {
            Debug.Log("FOREACH");
            for (int i = 0; i < 5; i++)
            {
                foreach (Collider c in weaponColliders)
                {
                    if (collisions[i] != null) Physics.IgnoreCollision(c, collisions[i], false);
                }
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
                collidersActive = false;
            }
            else
            {
                foreach (Collider c in weaponColliders)
                {
                    Physics.IgnoreCollision(c, other, false);
                }
                collisions[n] = other;
                n++;
                isClear = false;
            }
        }
        else if (other.gameObject.CompareTag("Block"))
        {
            collidersActive = false;
            GetComponentInParent<NetworkAnimator>().SetTrigger("Stop");
        }
    }
}
