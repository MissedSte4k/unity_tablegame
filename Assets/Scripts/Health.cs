﻿using System.Collections;
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
    public Text redTeamKillsText;
    public Text blueTeamKillsText;
    public Text TargetInfoText;
    public Text winText;

    PlayerRespawn pr;
    [SyncVar (hook = "OnHealthChanged")] int health;
    [SyncVar (hook = "OnStaminaChanged")] int stamina;

    void Start()
    {
        CmdCurrentScore();
    }

    void Awake()
    {
        pr = GetComponent<PlayerRespawn>();
        CmdCurrentScore();
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
        TakeDamage(amount, GetComponent<CharacterControl>().Team());
    }

    [Command]
    private void CmdCurrentScore()
    {
        CurrentScore();
    }

    [Server]
    private void CurrentScore()
    {
        RpcCurrentScore(FindObjectOfType<TeamControl>().CurrentScore());
    }

    public bool IsFatal(int amount)
    {
        if (health - amount > 0) return false;
        else return true;
    }

    [Server]
    private bool TakeDamage(int amount, int team)
    {
        health = health - amount;
        if (dependOnHealth && stamina > health) ChangeStamina(0);
        if (health <= 0)
        {
            FindObjectOfType<TeamControl>().IncreaseByOne(team);
            OnHealthChanged(0);
            OnStaminaChanged(0);
            RpcTakeDamage(true);
            return true;
        }

        return false;
    }

    [ClientRpc]
    public void RpcCurrentScore(int[] C)
    {
        UpdateKillText(C[0], C[1], C[2], C[3]);
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        if (died) pr.Die();
    }

    public bool IsStaminaMax()
    {
        if (dependOnHealth && stamina >= health || !dependOnHealth && stamina >= maxHealth) return true;
        else return false;
    }

    public bool IsStaminaZero()
    {
        if (stamina <= 0) return true;
        else return false;
    }

    public bool IsStaminaZero(int value)
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

    public void UpdateKillText(int target, int blue, int red, int won)
    {
        TargetInfoText.text = "TARGET\n" + target;
        blueTeamKillsText.text = blue.ToString();
        redTeamKillsText.text = red.ToString();

        if (won == 1)
        {
            winText.color = Color.blue;
            winText.text = "TEAM BLUE WINS!";
        }
        if (won == 2)
        {
            winText.color = Color.red;
            winText.text = "TEAM RED WINS!";
        }
    }
}
