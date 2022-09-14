using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField]
    TMP_InputField roomNameInputField;
    [SerializeField]
    TMP_Text errorText;
    [SerializeField]
    TMP_Text roomNameText;
    [SerializeField]
    Transform roomListContent;
    [SerializeField]
    Transform playerListContent;
    [SerializeField]
    GameObject roomListItemPrefab;
    [SerializeField]
    GameObject playerListItemPrefab;
    [SerializeField]
    GameObject startGameButton;
    [SerializeField]
    GameObject exitRoomMenuButton;
    // This is set to 265 because we want it to be in the -275 position for Pos x in the Rect Transform but for some reason it adds 540 to it and we were setting -275 to the normal x meaning that it was 275 over or something. 
    // Basically -275 + 540 = 265 and the correct pos.
    public float exitRoomOwnerButtonXPos = 265f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Master");
        // Connects to the photon master servers using the settings set in the PhotonServerSettings file. This will connect us to the fixed region we set of us west.
        PhotonNetwork.ConnectUsingSettings();
    }

    // This is a callback from Photon that is called when we successfully connect to the master server.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        PhotonNetwork.JoinLobby();

        // This will make it so that when the host switches scenes using PhotonNetwork.LoadLevel(), all players will be moved to that scene. 
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        MenuManager.Instance.OpenMenu("Title");

        // Here we are just giving a random nickname to the player that joins.
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }

    public void CreateRoom()
    {
        // If the name field is empty, we just return. Otherwise, we create the room.
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        PhotonNetwork.CreateRoom(roomNameInputField.text);

        // To stop players clicking buttons while it loads
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        // This will clear all the players in our client's player list content so that none of the previous players will be in the next session or will double up or anything.
        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in players)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(player);
        }

        // If the player is the master client/the host, then we set the start game button to be visible since IsMasterClient will be true, otherwise it will be false and it will be deactivated.
        UpdateHostButtons();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // If the master client has left the game and it has switched to someone else, then we must make sure that the start button can show up on their screen too. So we check then on every client if the person is the new master one
        // And just as before, we then set the button to be active or not based on that.
        UpdateHostButtons();
    }

    // This deals with making sure that the start game button is visible only for the host and with moving the exit room button over so that both fit on the screen since we have a limited x width for the screen.
    private void UpdateHostButtons()
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient)
        {
            RectTransform rectTrans = exitRoomMenuButton.GetComponent<RectTransform>();

            //Debug.Log(exitRoomOwnerButtonXPos);
            //Debug.Log(rectTrans.position.x);

            rectTrans.position = new Vector3(exitRoomOwnerButtonXPos, rectTrans.position.y);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("Error");
    }

    public void StartGame()
    {
        // We use 1 in the load level function since it is the build index of the "Game" scene which we want to load. This can be changed to load whatever scene.
        // PhotonNetwork.LoadLevel lets all players load at the same time as opposed to Unity's normal scene management. This is setup in OnConnectedToMaster() above where we set it to automatically sync the scene.
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        // We will join the room with the name matching the one they clicked and then we will open the loading menu since it will take some time for them to join.
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");

        // From here, when it has joined the room it will automatically call the OnJoinedRoom function above which will open the room menu.
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
    }

    // Gives us a list of roominfo classes which is Photon's room info stuff.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform transform in roomListContent)
        {
            // This will clear the list each time we receive an update of the list of rooms.
            Destroy(transform.gameObject);
        }

        foreach(RoomInfo info in roomList)
        {
            // For Photon, it does not remove the room from the list when it is empty/done but sets the info.RemovedFromList to true so if it is true, we skip it and don't display it.
            if (info.RemovedFromList)
                continue;

            // Room List Content is the room list container thing in our find room menu where all the buttons are held.
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(info);
        }
    }

    // This is only called when another player enters the room. Hence why we need to instantiate the player list item for all players whenever we enter a room in OnJoinedRoom.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // This adds the name of the player to the list.
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }
}