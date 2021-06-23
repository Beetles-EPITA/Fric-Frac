using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

namespace Manager
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        private PhotonView PV;
        private Random r = new Random();
        private Vector3[] playerPosition = 
        {
            new Vector3(89.6f, 27.7f, -310.8f),
            new Vector3(75.6f, 27.7f, -302.7f),
            new Vector3(77.1f, 27.7f, -290.3f),
            new Vector3(94.8f, 27.7f, -288.5f),
            new Vector3(94.7f, 27.7f, -281.5f),
            new Vector3(85.2f, 27.7f, -271.6f),
            new Vector3(81.6f, 27.7f, -284.5f),
        };

        private void Awake()
        {
            PV = GetComponent<PhotonView>();
        }

        private void Start()
        {
            if (PV.IsMine)
            {
                Cursor.visible = false;
                CreateController();
            }
        }

        private void CreateController()
        {
            Debug.Log("Instantiated Player Controller");
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player", "PlayerController"), playerPosition[r.Next(playerPosition.Length)], 
                Quaternion.identity);
        }
    }
}
