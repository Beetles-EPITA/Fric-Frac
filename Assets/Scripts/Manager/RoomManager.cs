using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Menus;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private Camera annimationCamera;
    [SerializeField] private AudioSource greenCar;
    [SerializeField] public Image crosshair;
    [SerializeField] public FinalScreen FinalScreen;
    [SerializeField] public Text infoText;
    
    private List<Item> items;


    public Dictionary<string, int> ItemsFind = new Dictionary<string, int>();
    
    [SerializeField] public Camera spectatorCamera;
    [SerializeField] public LoseScreenMenu LoseScreen;
    
    public static RoomManager Instance;

    private bool skipped;

    private void Awake()
    {
        Instance = this;
        items = ((Item[]) FindObjectsOfType(typeof(Item))).ToList();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        //UPDATE RPC
        DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence
        {
            largeImageKey = "icon", largeImageText = Application.version, details = "In Game", 
            state = PhotonNetwork.CurrentRoom.Name + " server", 
            partySize = PhotonNetwork.CurrentRoom.PlayerCount,
            partyMax = PhotonNetwork.CurrentRoom.MaxPlayers,
            startTimestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()
        };
        DiscordRpc.UpdatePresence(presence);
    }
    
    private void OnApplicationQuit()
    {
        DiscordRpc.ClearPresence();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(WaitAnimation(scene));
        InitItems();
    }

    public bool start = false;
    private int count = 0;
    
    IEnumerator WaitAnimation(Scene scene)
    {
        LogMessage.Instance.ShowMessage("Waiting for players...", false);
        photonView.RPC("AddPlayer", RpcTarget.MasterClient);
        yield return new WaitUntil(() =>
            PhotonNetwork.IsMasterClient && count == PhotonNetwork.CurrentRoom.PlayerCount || start);
        if(PhotonNetwork.IsMasterClient) photonView.RPC("StartGame", RpcTarget.AllBuffered);
        LogMessage.Instance.ClearMessages();
        _director.Play();
        yield return new WaitForSeconds(1);
        greenCar.Play();
        annimationCamera.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds((int) _director.duration + 2);
        if(!skipped) CreatePlayer(scene);
        skipped = true;
    }

    [PunRPC]
    public void StartGame()
    {
        start = true;
    }

    [PunRPC]
    public void AddPlayer()
    {
        count++;
    }

    private void CreatePlayer(Scene scene)
    {
        if (scene.name.Equals("Multiplayer"))
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Players.Count % 2 != 0)
            {
                Debug.Log("Instantiated IA");
                PhotonNetwork.Instantiate(Path.Combine("Prefabs", "IA", "IAManager"), Vector3.zero, Quaternion.identity);
            }
            
            annimationCamera.GetComponent<AudioListener>().enabled = false;
            crosshair.gameObject.SetActive(true);
            if((int) PhotonNetwork.LocalPlayer.CustomProperties["team"] == (int) Laucher.Team.Thief)
                ItemListMenu.Instance.gameObject.SetActive(true);
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player", "PlayerManager"), Vector3.zero, Quaternion.identity);
            spectatorCamera.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        Skip();
        TabList();
        Pause();
    }
    
    /**
     * Tab Menu
     */
    
    private void TabList()
    {
        if(TabMenu.InstanceMenu == null)
            return;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TabMenu.InstanceMenu.Open();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            TabMenu.InstanceMenu.Close();
        }
    }

    /**
     * Pause Menu
     */

    private void Pause()
    {
        Cursor.visible = Menus.Pause.isPause;
        Cursor.lockState = Menus.Pause.isPause ? 
            CursorLockMode.None : CursorLockMode.Locked;
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Menus.Pause.Instance.setPause(!Menus.Pause.isPause);
        }
    }

    private void Skip()
    {
        if (!skipped && Input.GetKeyDown(KeyCode.F6))
        {
            skipped = true;
            annimationCamera.GetComponent<AudioSource>().Stop();
            greenCar.Stop();
            CreatePlayer(SceneManager.GetActiveScene());
        }
    }

    private int viewID = 9200;
    private void InitItems()
    {
        foreach (var item in items)
        {
            item.gameObject.AddComponent<PhotonView>().ViewID = viewID;
            Outline outline = item.gameObject.AddComponent<Outline>();
            outline.OutlineWidth = 6f;
            outline.enabled = false;
            viewID++;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < (PhotonNetwork.CurrentRoom.Players.Count + 1) / 2; i++)
            {
                for (int j = 0; j < new Random().Next(3, 6); j++)
                {
                    int random = new Random().Next(items.Count);
                    Item item = items[random];
                    photonView.RPC("AddItem", RpcTarget.All, item.itemName, false);
                    items.Remove(item);
                }
            }
            photonView.RPC("CreateListItems", RpcTarget.All);
        }
    }

    [PunRPC]
    private void CreateListItems()
    {
        ItemListMenu.Instance.CreateList(ItemsFind);
    }
    
    [PunRPC]
    private void AddItem(string itemName, bool update)
    {
        if (ItemsFind.ContainsKey(itemName))
            ItemsFind[itemName] += 1;
        else
            ItemsFind.Add(itemName, 1);
        if(update) ItemListMenu.Instance.UpdateList(ItemsFind);
    }

    [PunRPC]
    private void RemoveItem(string itemName, bool update)
    {
        if (ItemsFind.ContainsKey(itemName))
        {
            if (ItemsFind[itemName] <= 1)
                ItemsFind.Remove(itemName);
            else
                ItemsFind[itemName] -= 1;
            if(update) ItemListMenu.Instance.UpdateList(ItemsFind);
        }
    }

    [PunRPC]
    private void UpdateTab()
    {
        TabMenu.InstanceMenu.GetComponent<TabMenu>().UpdateTab();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        LogMessage.Send(otherPlayer.NickName + " left the game");
        UpdateTab();
    }

    [PunRPC]
    private void CheckWin(int team)
    {
        if ((Laucher.Team) team == Laucher.Team.Resident)
        {
            if (SkyCycle.losed)
            {
                FinalScreen.SetUp("The day has dawned, the thieves have taken too long", 
                    PlayerController.myController.Team == Laucher.Team.Resident, PhotonNetwork.IsMasterClient);
                return;
            }
            bool found = false;
            foreach (var currentRoomPlayer in PhotonNetwork.CurrentRoom.Players)
            {
                if ((Laucher.Team) currentRoomPlayer.Value.CustomProperties["team"] == Laucher.Team.Thief &&
                    !(bool) currentRoomPlayer.Value.CustomProperties["death"])
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                FinalScreen.SetUp("All the thieves have been caught", 
                        PlayerController.myController.Team == Laucher.Team.Resident, PhotonNetwork.IsMasterClient);
            }
        }
        else
        {
            if(ItemsFind.Count == 0)
                FinalScreen.SetUp("The thieves have recovered all the objects", 
                    PlayerController.myController.Team == Laucher.Team.Thief, PhotonNetwork.IsMasterClient);
        }
    }
}
