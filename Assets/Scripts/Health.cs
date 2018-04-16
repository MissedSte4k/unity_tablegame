using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField] int maxHealth = 100;
    [SerializeField] int maxStamina = 100;
    [SerializeField] bool dependOnHealth = false;
    public Text teamText;
    public Slider healthSlider;
    public Slider staminaSlider;

    PlayerRespawn pr;
    [SyncVar (hook = "OnHealthChanged")] int health;
    [SyncVar (hook = "OnStaminaChanged")] int stamina;

    void Awake()
    {
        pr = GetComponent<PlayerRespawn>();
    }

    public void SetTeamText(int team)
    {
        switch (team)
        {
            case 1:
                teamText.text = "Team Blue";
                teamText.color = Color.blue;
                break;
            case 2:
                teamText.text = "Team Red";
                teamText.color = Color.red;
                break;
            default:
                teamText.text = "Spectator";
                teamText.color = Color.gray;
                break;
        }
    }

    [ServerCallback]
    void OnEnable()
    {
        health = maxHealth;
        if (dependOnHealth) stamina = maxHealth;
        else stamina = maxStamina;
    }

    [Command]
    public void CmdTakeDamage(int amount)
    {
        TakeDamage(amount);
    }

    [Server]
    public bool TakeDamage(int amount)
    {
        health = health - amount;
        if (dependOnHealth && stamina > health) ChangeStamina(0);
        if (health <= 0)
        {
            OnHealthChanged(0);
            OnStaminaChanged(0);
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

    public bool isStaminaMax()
    {
        if (dependOnHealth && stamina >= health || !dependOnHealth && stamina >= maxHealth) return true;
        else return false;
    }

    public bool isStaminaZero()
    {
        if (stamina <= 0) return true;
        else return false;
    }

    public bool isStaminaZero(int value)
    {
        if (stamina + value <= 0) return true;
        else return false;
    }

    public void ChangeStamina(int value)
    {
        stamina = stamina + value;
    }

    public int CurrentHealth()
    {
        return health;
    }

    void OnHealthChanged(int value)
    {
        health = value;
        if (isLocalPlayer) healthSlider.value = value;
    }

    void OnStaminaChanged(int value)
    {
        stamina = value;
        if (dependOnHealth && stamina > health) stamina = health;
        if (!dependOnHealth && stamina > maxStamina) stamina = maxStamina;
        if (stamina < 0) stamina = 0;
        if (isLocalPlayer) staminaSlider.value = value;
    }
}
