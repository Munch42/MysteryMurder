using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_InputField roomNameInputField;
    [SerializeField]
    TMP_Text errorText;
    [SerializeField]
    TMP_Text roomNameText;

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
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        MenuManager.Instance.OpenMenu("Title");
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
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("Error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
    }
}