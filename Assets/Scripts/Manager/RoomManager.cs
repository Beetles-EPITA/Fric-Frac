using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Menus;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private Camera annimationCamera;
    
    public static RoomManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        _director.Play();
        StartCoroutine(WaitAnimation(scene));
    }

    IEnumerator WaitAnimation(Scene scene)
    {
        yield return new WaitForSeconds((int) _director.duration + 1);
        CreatePlayer(scene);
    }

    private void CreatePlayer(Scene scene)
    {
        if (scene.name.Equals("Multiplayer"))
        {
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    private void Update()
    {
        TabList();
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
}
