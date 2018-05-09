using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeleeWeapon : NetworkBehaviour
{

    [HideInInspector] public int damage;
    [HideInInspector] public bool isSlash; //marks whether the weapon can damage more than 1 character.
    public Collider[] weaponColliders;
    private Collider[] collisions = new Collider[5];
    private int n = 0;
    private bool isClear = true;
    [HideInInspector] public bool collidersActive;
    [HideInInspector] public NetworkInstanceId attacker;
    [HideInInspector] public bool isTrigger;

    // Use this for initialization
    void Start()
    {
        collidersActive = false;
        isTrigger = true;
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
        if (isTrigger)
        {
            foreach (Collider c in weaponColliders)
            {
                c.isTrigger = true;
            }
        }
        else
        {
            foreach (Collider c in weaponColliders)
            {
                c.isTrigger = false;
            }
        }
    }

    void FixedUpdate()
    {
        if ((GetComponentInParent<NetworkAnimator>().animator.GetCurrentAnimatorStateInfo(1).IsName("Running") ||
            GetComponentInParent<NetworkAnimator>().animator.GetCurrentAnimatorStateInfo(1).IsName("Crouched"))
            && !isClear)
        {
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
                {
                    health.RpcTakeDamage(damage, health.IsFatal(damage));
                }
            }

            if (!isSlash)
            {
                collidersActive = false;
            }
            else
            {
                foreach (Collider c in weaponColliders)
                {
                    Physics.IgnoreCollision(c, other);
                }
                collisions[n] = other;
                n++;
                isClear = false;
            }
        }
        else if (other.gameObject.CompareTag("Block"))
        {
            collidersActive = false;
            GetComponentInParent<CharacterControl>().RpcHitBlock();
            other.GetComponentInParent<CharacterControl>().RpcBlockHurt();
        }
    }
}
