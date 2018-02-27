using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    /*public const int maxHealth = 100;
    public int currentHealth;
    public Text healthText;

    // Use this for initialization
    void Start () {
        currentHealth = maxHealth;
    }
	
	// Update is called once per frame
	void Update () {
        if (isLocalPlayer) healthText.text = "Health: " + currentHealth;
        else healthText.enabled = false;
    }
    
    public void TakeDamage(int amount)
    {
        currentHealth = currentHealth - amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }*/

    [SerializeField] int maxHealth = 100;
    public Text healthText;

    PlayerRespawn pr;
    int health;

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
}
