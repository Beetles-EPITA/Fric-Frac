using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public static RoomManager Instance;

    private bool skipped;

    private void Awake()
    {
        Instance = this;
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
}
