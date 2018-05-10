using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PrefabControl : NetworkManager
{
    public GameObject archerBlue;
    public GameObject archerRed;
    public GameObject berserkerBlue;
    public GameObject berserkerRed;
    public GameObject knightBlue;
    public GameObject knightRed;
    public GameObject scoutBlue;
    public GameObject scoutRed;

    public class NetworkMessage : MessageBase
    {
        public int team;
        public int character;
        public int primaryWeapon;
        public int secondaryWeapon;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {     
        NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage>();

        switch (message.team)
        {
            case 0:
                switch (message.character)
                {
                    case 0:
                        LoadPlayer(knightBlue, conn, playerControllerId);
                        break;
                    case 1:
                        LoadPlayer(scoutBlue, conn, playerControllerId);
                        break;
                    case 2:
                        LoadPlayer(berserkerBlue, conn, playerControllerId);
                        break;
                    case 3:
                        LoadPlayer(archerBlue, conn, playerControllerId);
                        break;
                    default:
                        break;
                }
                break;
            case 1:
                switch (message.character)
                {
                    case 0:
                        LoadPlayer(knightRed, conn, playerControllerId);
                        break;
                    case 1:
                        LoadPlayer(scoutRed, conn, playerControllerId);
                        break;
                    case 2:
                        LoadPlayer(berserkerRed, conn, playerControllerId);
                        break;
                    case 3:
                        LoadPlayer(archerRed, conn, playerControllerId);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        PlayOptions po = FindObjectOfType<PlayOptions>();
        NetworkMessage message = new NetworkMessage();
        message.team = po.teamIndex;
        message.character = po.characterIndex;
        message.primaryWeapon = po.primaryWeaponIndex;
        message.secondaryWeapon = po.secondaryWeaponIndex;
        ClientScene.AddPlayer(conn, 0, message);
    }

    private void LoadPlayer(GameObject prefab, NetworkConnection conn, short playerControllerId)
    {
        Debug.Log(prefab.ToString() + ", " + playerControllerId);
        GameObject player = Instantiate(prefab) as GameObject;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public int Team()
    {
        switch(FindObjectOfType<PlayOptions>().teamIndex)
        {
            case 0:
                return 1;
            case 1:
                return 2;
            default:
                return 0;
        }
    }
}

