﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class KillOnTouchWithoutDestroying : NetworkBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (isServer)
        {
            var hit = collision.gameObject;
            var health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(health.CurrentHealth());
            }
        }
    }

}

