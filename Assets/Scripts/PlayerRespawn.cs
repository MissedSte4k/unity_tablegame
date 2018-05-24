using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }

public class PlayerRespawn : NetworkBehaviour {

    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;
    [SerializeField] int respawnTime = 5;

    //GameObject mainCamera;

    void Start()
    {
        EnablePlayer();
    }

    void DisablePlayer()
    {
        onToggleShared.Invoke(false);

        if (isLocalPlayer) onToggleLocal.Invoke(false);

        else onToggleRemote.Invoke(false);
    }

    void EnablePlayer()
    {
        onToggleShared.Invoke(true);

        if (isLocalPlayer) onToggleLocal.Invoke(true);
        else onToggleRemote.Invoke(true);
    }

    public void Die()
    {
        DisablePlayer();

        Health health = GetComponent<Health>();
        health.currentRespawnCount = respawnTime;
        health.UpdateRespawnCount();
        Invoke("RespawnCount", 1);
    }

    void Respawn()
    {
        if (isLocalPlayer)
        {
            Transform spawn = NetworkManager.singleton.GetStartPosition();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }

        EnablePlayer();
    }

    public void RespawnCount()
    {
        Health health = GetComponent<Health>();
        health.currentRespawnCount--;
        health.UpdateRespawnCount();
        if (health.currentRespawnCount < 1) Respawn();
        else Invoke("RespawnCount", 1);
    }
}
