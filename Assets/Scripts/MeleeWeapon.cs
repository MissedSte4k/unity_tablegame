using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeleeWeapon : NetworkBehaviour
{
    [Header("This weapon's colliders")]
    public Collider[] weaponColliders;

    private Collider[] collisions = new Collider[5];
    private int n = 0;
    private bool isClear = true;
    [HideInInspector] public bool collidersActive;
    [HideInInspector] public bool isTrigger;
    [HideInInspector] public int damage;
    [HideInInspector] public bool isSlash; //marks whether the weapon can damage more than 1 character.

    [Header("Audio sources and sounds")]
    public AudioSource audioSourceDamage;
    public AudioSource audioSourceSlash;
    public AudioClip damageClip;
    public AudioClip blockHitClip;

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
        if (!isClear && !collidersActive)
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
            audioSourceDamage.PlayOneShot(damageClip);
            var health = other.gameObject.GetComponent<Health>();
            if (health != null)
            {
                    health.RpcTakeDamage(damage, health.IsFatal(damage), other.GetComponent<CharacterControl>().Team());
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
            audioSourceDamage.PlayOneShot(blockHitClip);
            collidersActive = false;
            GetComponentInParent<CharacterControl>().RpcHitBlock();
            other.GetComponentInParent<CharacterControl>().RpcBlockHurt();
        } else if (other.gameObject.CompareTag("Ground"))
        {
            audioSourceDamage.PlayOneShot(blockHitClip);
            collidersActive = false;
            GetComponentInParent<CharacterControl>().RpcHitBlock();
        }
        else audioSourceDamage.PlayOneShot(blockHitClip);
    }
}
