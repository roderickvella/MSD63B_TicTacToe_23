using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lobby : MonoBehaviourPunCallbacks
{
    [Tooltip("Content Object")]
    public GameObject ScrollViewContent;

    [Tooltip("UI ROW Prefab containing the room details")]
    public GameObject RowRoom;

    [Tooltip("Player Name")]
    public GameObject InputPlayerName;

    [Tooltip("Room Name")]
    public GameObject InputRoomName;

    [Tooltip("Status Message")]
    public GameObject Status;

    [Tooltip("Button Create Room")]
    public GameObject BtnCreateRoom;

    [Tooltip("Panel lobby")]
    public GameObject PanelLobby;

    [Tooltip("Panel waiting for players")]
    public GameObject PanelWaitingForPlayers;

    // Start is called before the first frame update
    void Start()
    {
        //automatically synce the scenes between the users. so if the master client moves from the lobby scene to the
        //game scene, then all other clients should migrate to the game scene
        
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            //set the app version before connecting
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "1.0";

            //connect to the photon master server
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster");
        //after we are connected to the master server, we need to connect to the lobby
        PhotonNetwork.JoinLobby(TypedLobby.Default);

    }

    private void OnGUI()
    {
        Status.GetComponent<TextMeshProUGUI>().text = "Status:" + PhotonNetwork.NetworkClientState.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
