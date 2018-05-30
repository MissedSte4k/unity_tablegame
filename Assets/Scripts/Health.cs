using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    [Header("Maximum character health and stamina")]
    [Range(0, 200)]
    [SerializeField]
    private int maxHealth;
    [Range(0, 200)]
    [SerializeField]
    private int maxStamina;

    [Header("Makes maximum stamina dependent on current health")]
    [SerializeField]
    private bool dependOnHealth = false;

    [Header("Delay until stamina can recharge after being used")]
    [Range(0, 5)]
    public float rechargeDelay;
    [HideInInspector] public float delayRemaining;
    [HideInInspector] public bool canRecharge = true;

    [Header("UI elements")]
    public Text teamText;
    public Slider healthSlider;
    public Slider staminaSlider;
    public Text redTeamKillsText;
    public Text blueTeamKillsText;
    public Text TargetInfoText;
    public Text winText;

    [Header("Audio sources and sounds")]
    public AudioSource audioSourceHurt;
    public AudioClip hurtAudio;

    private PlayerRespawn pr;

    [SyncVar(hook = "OnHealthChanged")] private int health;
    [SyncVar(hook = "OnStaminaChanged")] private float stamina;

    void Start()
    {
        if (isLocalPlayer)
        {
            CmdCurrentScore();

            canRecharge = true;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            if (dependOnHealth)
            {
                staminaSlider.maxValue = maxHealth;
                staminaSlider.value = maxHealth;
            }
            else
            {
                staminaSlider.maxValue = maxStamina;
                staminaSlider.value = maxStamina;
            }
        }
    }

    void Awake()
    {
        pr = GetComponent<PlayerRespawn>();
        if (isLocalPlayer)
            CmdCurrentScore();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (!canRecharge)
            {
                delayRemaining -= Time.deltaTime;
                if (delayRemaining <= 0)
                {
                    canRecharge = true;
                }
            }
        }
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

    public void TakeDamage(int amount)
    {
        CmdTakeDamage(amount, GetComponent<CharacterControl>().Team(), IsFatal(amount));
    }

    public void Heal(int amount)
    {
        CmdHeal(amount);
    }

    [Command]
    public void CmdTakeDamage(int amount, int team, bool fatal)
    {
        ServerTakeDamage(amount, team, fatal);
    }

    [Command]
    public void CmdHeal(int amount)
    {
        RpcHeal(amount);
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
    public void ServerTakeDamage(int amount, int team, bool fatal)
    {
        RpcTakeDamage(amount, fatal, team);
    }

    [ClientRpc]
    public void RpcCurrentScore(int[] C)
    {
        UpdateKillText(C[0], C[1], C[2], C[3]);
    }

    [ClientRpc]
    public void RpcTakeDamage(int amount, bool died, int team)
    {
        audioSourceHurt.PlayOneShot(hurtAudio);
        if (died)
        {
            health = 0;
            stamina = 0;
            pr.Die();
            if (isLocalPlayer)
            {
                CmdUpScore(team);
            }
        }
        else
        {
            if (isLocalPlayer)
            {
                GetComponent<NetworkAnimator>().SetTrigger("Hurt");
            }
            health = health - amount;
            if (dependOnHealth && stamina > health) ChangeStamina(-1);
        }
    }
    
    [Command]
    void CmdUpScore(int team)
    {
        FindObjectOfType<TeamControl>().IncreaseByOne(team);
    }

    [ClientRpc]
    public void RpcHeal(int amount)
    {
        if (health + amount > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += amount;
        }
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

    public void ChangeStamina(float value)
    {
        if (dependOnHealth && stamina + value > health) stamina = health;
        else if (!dependOnHealth && stamina + value > maxStamina) stamina = maxStamina;
        else if (stamina + value < 0) stamina = 0;
        else stamina = stamina + value;
        if (isServer && value < 0)
        {
            RpcSetRecharge();
        }
    }

    [Command]
    public void CmdChangeStamina(float value)
    {
        ChangeStaminaServer(value);
    }

    [Server]
    public void ChangeStaminaServer(float value)
    {
        ChangeStamina(value);
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

    void OnStaminaChanged(float value)
    {
        stamina = value;
        if (dependOnHealth && stamina > health) stamina = health;
        if (!dependOnHealth && stamina > maxStamina) stamina = maxStamina;
        if (stamina < 0) stamina = 0;
        if (isLocalPlayer) staminaSlider.value = stamina;
    }

    public void UpdateKillText(int target, int blue, int red, int won)
    {
        if (isLocalPlayer)
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

    [ClientRpc]
    void RpcSetRecharge()
    {
        if (isLocalPlayer)
        {
            canRecharge = false;
            delayRemaining = rechargeDelay;
        }
    }
}
