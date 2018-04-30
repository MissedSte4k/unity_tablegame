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
    [SyncVar]
    private int n = 0;
    [SyncVar]
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
            foreach (Collider c in collisions)
            {
                if (c != null && (isLocalPlayer || isClient))
                    CmdUnignoreCollision();
            }
            Array.Clear(collisions, 0, 5);
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
                 health.CmdTakeDamage(damage);
            }

            if (!isSlash)
            {
                if (isLocalPlayer)
                {
                    col.enabled = false;
                    CmdDisableCollider();
                }
            }
            else
            {
                collisions[n] = other;
                if (isLocalPlayer)
                {
                    CmdIgnoreCollision();
                }
                isClear = false;
            }
        }
        else if (other.gameObject.CompareTag("Block"))
        {
            col.enabled = false;
            if (isLocalPlayer)
            {
                CmdDisableCollider();
            }
            GetComponentInParent<NetworkAnimator>().SetTrigger("Stop");
        }
    }

    [Command]
    void CmdDisableCollider()
    {
        RpcDisableCollider();
    }

    [ClientRpc]
    void RpcDisableCollider()
    {
        col.enabled = false;
    }

    [Command]
    void CmdIgnoreCollision()
    {
        RpcIgnoreCollision();
    }

    [ClientRpc]
    void RpcIgnoreCollision()
    {
        Physics.IgnoreCollision(col, collisions[n]);
        n++;
    }

    [Command]
    void CmdUnignoreCollision()
    {
        RpcUnignoreCollision();
    }

    [ClientRpc]
    void RpcUnignoreCollision()
    {
        Physics.IgnoreCollision(col, collisions[n], false);
        n--;
    }
}
