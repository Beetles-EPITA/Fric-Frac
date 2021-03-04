using System;
using System.IO;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

namespace Manager
{
    public class PlayerManager : MonoBehaviour
    {
        private PhotonView PV;

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
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player", "PlayerController"), new Vector3(137, 19.5f, -325), 
                Quaternion.identity);
        }

        private Vector3 RandomPosition()
        {
            Random random = new Random();
            return new Vector3(random.Next(30) - 15, 0, random.Next(30) - 15);
        }
    }
}
