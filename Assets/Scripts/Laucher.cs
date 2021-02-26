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
using Random = UnityEngine.Random;

public class Laucher : MonoBehaviourPunCallbacks
{

    public static Laucher Instance;
    
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private Scrollbar scrollBarLoading;
    [SerializeField] private GameObject startGameButton;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Server...");
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }

    public void CreateRoom(Text roomName)
    {
        MainMenuManager.Instance.OpenMenu("Loading");
        scrollBarLoading.size = 0.5F;
        PhotonNetwork.CreateRoom(roomName.text);
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

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Multiplayer");
        
    }

    public override void OnJoinedRoom()
    {
        Menu menu = MainMenuManager.Instance.OpenMenu("LobbyMenu");
        Text menuName = menu.GetComponentInChildren<Text>();
        menuName.text = PhotonNetwork.CurrentRoom.Name;
        
        foreach (Transform transform in playerListContent)
        {
            Destroy(transform.gameObject);
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        }
        
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    IEnumerator ProgessBarLoading(AsyncOperation operation)
    {
        while (!operation.isDone)
        {
            scrollBarLoading.size = 0.1F + operation.progress * 0.9F;
            yield return null;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Menu menu = MainMenuManager.Instance.OpenMenu("ErrorMenu");
        Text text = menu.GetComponentInChildren<Text>();
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
            if(roomInfo.RemovedFromList)
                continue;
            GameObject button = Instantiate(roomListItemPrefab, roomListContent);
            button.GetComponent<RoomListItem>().SetUp(roomInfo);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
    
    
    
}
