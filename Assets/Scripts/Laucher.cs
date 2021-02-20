using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Menus;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Laucher : MonoBehaviourPunCallbacks
{

    public static Laucher Instance;
    
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom(Text roomName)
    {
        PhotonNetwork.CreateRoom(roomName.text);
        MainMenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MainMenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Multiplayer");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Menu menu = MainMenuManager.Instance.OpenMenu("ErrorMenu");
        Text text = (Text) menu.GetComponentInChildren(typeof(Text));
        text.text = "Cannot connect to the server : " + message;
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
        MainMenuManager.Instance.OpenMenu("Main");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform transform in roomListContent)
        {
            Destroy(transform.gameObject);
        }
        foreach (RoomInfo roomInfo in roomList)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomInfo);
        }
    }
}
