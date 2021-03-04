using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance != null)
        { 
            Destroy(Instance.gameObject);
        }
        gameObject.AddComponent<PhotonView>();
        photonView.ViewID = new Random().Next(999) + 1000;
        DontDestroyOnLoad(gameObject);
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
        if (scene.name.Equals("Multiplayer"))
        {
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
}
