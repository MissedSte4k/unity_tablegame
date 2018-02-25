using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public const int maxHealth = 100;
    public int currentHealth = maxHealth;
    public Text healthText;

    // Use this for initialization
    void Start () {

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
    }
}
