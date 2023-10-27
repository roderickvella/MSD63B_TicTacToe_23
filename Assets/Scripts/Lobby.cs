using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    List<RoomInfo> availableRooms = new List<RoomInfo>();

    UnityEngine.Events.UnityAction buttonCallback;

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

        InputRoomName.GetComponent<TMP_InputField>().text = "Room1";
        InputPlayerName.GetComponent<TMP_InputField>().text = "Player1";
    }

    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster");
        //after we are connected to the master server, we need to connect to the lobby
        PhotonNetwork.JoinLobby(TypedLobby.Default);

    }

    public void CreateRoom()
    {
        //creating settings for our room
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;

        //create room
        PhotonNetwork.JoinOrCreateRoom(InputRoomName.GetComponent<TMP_InputField>().text,
            roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        print("Created Room");
        //set our player name
        PhotonNetwork.NickName = InputPlayerName.GetComponent<TMP_InputField>().text;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Cannot create room: Error Code:" + returnCode + "; Message: " + message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
       
    {
        print("Number of Rooms in Lobby" + roomList.Count);
        availableRooms = roomList;
        if (roomList.Count > 0) InputPlayerName.GetComponent<TMP_InputField>().text = "Player2";
        UpdateRoomList();
    }

    private void UpdateRoomList()
    {
        foreach(RoomInfo roomInfo in availableRooms)
        {
            GameObject rowRoom = Instantiate(RowRoom);
            //attach it to the scrollview
            rowRoom.transform.parent = ScrollViewContent.transform;
            rowRoom.transform.localScale = Vector3.one;

            //now we need to update the room name and number of players
            rowRoom.transform.Find("RoomName").GetComponent<TextMeshProUGUI>().text = roomInfo.Name;
            rowRoom.transform.Find("RoomPlayers").GetComponent<TextMeshProUGUI>().text =
                roomInfo.PlayerCount.ToString();

            //we need to make the join button clickable
            buttonCallback = () => OnClickJoinRoom(roomInfo.Name);
            rowRoom.transform.Find("BtnJoin").GetComponent<Button>()
                .onClick.AddListener(buttonCallback);
        }
    }

    public void OnClickJoinRoom(string roomName)
    {
        //set our player name
        PhotonNetwork.NickName = InputPlayerName.GetComponent<TMP_InputField>().text;

        //join the room by room name
        
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        PanelLobby.SetActive(false);
        PanelWaitingForPlayers.SetActive(true);
        print("OnJoinedRoom");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //this means if in the current room there are 2 players,then we can start the game
        //IMP! do not forget to add the scenes in the build settings for this to work
      if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("MainGame");
        }
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
