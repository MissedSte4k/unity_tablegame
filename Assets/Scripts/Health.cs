using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField] int maxHealth = 100;
    public Text healthText;

    PlayerRespawn pr;
    [SyncVar (hook = "OnHealthChanged")] int health;

    void Awake()
    {
        pr = GetComponent<PlayerRespawn>();
    }

    [ServerCallback]
    void OnEnable()
    {
        health = maxHealth;
    }

    [Server]
    public bool TakeDamage(int amount)
    {
        health = health - amount;

        if (health <= 0)
        {
            RpcTakeDamage(true);
            return true;
        }

        return false;
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        if (died) pr.Die();
    }

    public int CurrentHealth()
    {
        return health;
    }

    void OnHealthChanged(int value)
    {
        health = value;
        if(isLocalPlayer) healthText.text = "Health: " + health;
    }
}
