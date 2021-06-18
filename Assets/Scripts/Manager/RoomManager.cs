using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Menus;
using Photon.Pun;
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
    [SerializeField] private Image crosshair;

    [SerializeField] private GameObject[] prefabsItems;
    [SerializeField] private Transform AllPositons;
    private List<Transform> randomPositions;
    
    
    public Dictionary<string, int> ItemsFind = new Dictionary<string, int>();
    
    public static RoomManager Instance;

    private bool skipped;

    private void Awake()
    {
        Instance = this;
        randomPositions = AllPositons.GetComponentsInChildren<Transform>().ToList();
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

    IEnumerator WaitAnimation(Scene scene)
    {
        _director.Play();
        yield return new WaitForSeconds(1);
        greenCar.Play();
        annimationCamera.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds((int) _director.duration + 2);
        if(!skipped) CreatePlayer(scene);
        skipped = true;
    }

    private void CreatePlayer(Scene scene)
    {
        if (scene.name.Equals("Multiplayer"))
        {
            annimationCamera.GetComponent<AudioListener>().enabled = false;
            crosshair.gameObject.SetActive(true);
            if((int) PhotonNetwork.LocalPlayer.CustomProperties["team"] == (int) Laucher.Team.Thief)
                ItemListMenu.Instance.gameObject.SetActive(true);
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player", "PlayerManager"), Vector3.zero, Quaternion.identity);
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

    private void InitItems()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < (PhotonNetwork.CurrentRoom.Players.Count + 1) / 2; i++)
            {
                for (int j = 0; j < new Random().Next(3, 6); j++)
                {
                    int random = new Random().Next(prefabsItems.Length);
                    int randPos = new Random().Next(randomPositions.Count);
                    Item item = prefabsItems[random].GetComponent<Item>();
                    GameObject go = PhotonNetwork.Instantiate(Path.Combine("Objects", "Items", prefabsItems[random].name), randomPositions[randPos].position,
                        randomPositions[randPos].rotation);
                    photonView.RPC("AddItem", RpcTarget.All, item.itemName, false);
                    randomPositions.RemoveAt(randPos);
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
}
