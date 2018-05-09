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

    //public int chosenCharacter = 0;

    //subclass for sending network messages
    /*public class NetworkMessage : MessageBase
    {
        public int chosenClass;
    }*/

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) //, NetworkReader extraMessageReader)
    {
        //NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage>();
        //int selectedClass = message.chosenClass;
        //Debug.Log("server add with message " + selectedClass);

        /*if (selectedClass == 0)
        {
            GameObject player = Instantiate(Resources.Load("Characters/A", typeof(GameObject))) as GameObject;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        if (selectedClass == 1)
        {
            GameObject player = Instantiate(Resources.Load("Characters/B", typeof(GameObject))) as GameObject;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }*/
        switch (FindObjectOfType<PlayOptions>().teamIndex)
        {
            case 0:
                switch (FindObjectOfType<PlayOptions>().characterIndex)
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
                switch (FindObjectOfType<PlayOptions>().characterIndex)
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

    /*public override void OnClientConnect(NetworkConnection conn)
    {
        NetworkMessage test = new NetworkMessage();
        test.chosenClass = chosenCharacter;

        ClientScene.AddPlayer(conn, 0, test);
    }*/


    /*public override void OnClientSceneChanged(NetworkConnection conn)
    {
        //base.OnClientSceneChanged(conn);
    }*/

    private void LoadPlayer(GameObject prefab, NetworkConnection conn, short playerControllerId)
    {
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

