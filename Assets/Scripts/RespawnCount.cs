using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;

public class RespawnCount : NetworkBehaviour {

    private Text countText;

    void Start()
    {
        countText = GetComponentInChildren<Text>();
        countText.text = "";
    }

    public void SetCount(int value)
    {
        countText.text = value.ToString();
    }

    public void Clear()
    {
        countText.text = "";
    }
}
