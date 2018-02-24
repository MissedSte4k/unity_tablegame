using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    public const int maxHealth = 100;
    public int currentHealth = maxHealth;
    public Text healthText;

    // Use this for initialization
    void Start () {
        currentHealth = maxHealth;
        healthText.text = "Health: " + currentHealth;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(int amount)
    {
        currentHealth = currentHealth - amount;
        healthText.text = "Health: " + currentHealth;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Dead!");
        }
    }
}
