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
    [SerializeField] float respawnTime = 5f;
    float timeLeft = 0;

    GameObject mainCamera;
    RespawnCount rc;

    void Start()
    {
        foreach(GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.tag == "MainCamera")
            {
                mainCamera = obj;
                break;
            }
        }
        EnablePlayer();
    }

    private void DisablePlayer()
    {
        timeLeft = respawnTime;
        onToggleShared.Invoke(false);

        if (isLocalPlayer)
        {
            mainCamera.GetComponent<Camera>().enabled = true;
            mainCamera.GetComponent<AudioListener>().enabled = true;
            mainCamera.GetComponent<FlareLayer>().enabled = true;
            rc = mainCamera.GetComponent<RespawnCount>();
            rc.SetCount((int)timeLeft);
            onToggleLocal.Invoke(false);
        }

        else onToggleRemote.Invoke(false);
    }

    void EnablePlayer()
    {
        onToggleShared.Invoke(true);

        if (isLocalPlayer)
        {
            mainCamera.GetComponent<Camera>().enabled = false;
            mainCamera.GetComponent<AudioListener>().enabled = false;
            mainCamera.GetComponent<FlareLayer>().enabled = false;
            mainCamera.GetComponent<RespawnCount>().Clear();
            onToggleLocal.Invoke(true);
        }
        else onToggleRemote.Invoke(true);
    }

    public void Die()
    {
        DisablePlayer();
        if(rc != null && isLocalPlayer) Invoke("Count", 1);
        else Invoke("Respawn", respawnTime);
    }

    void Count()
    {
        timeLeft--;
        rc.SetCount((int)timeLeft);
        if (timeLeft <= 1) Invoke("Respawn", timeLeft);
        else Invoke("Count", 1);
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
}
