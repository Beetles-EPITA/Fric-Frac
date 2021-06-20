using System;
using System.IO;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

namespace Manager
{
    public class PlayerManager : MonoBehaviourPunCallbacks
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
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player", "PlayerController"), RandomPosition(), 
                Quaternion.identity);
        }

        public static Vector3 RandomPosition()
        {
            Random random = new Random();
            switch ((Laucher.Team) PhotonNetwork.LocalPlayer.CustomProperties["team"])
            {
                case Laucher.Team.Resident:
                    return new Vector3(random.Next(12) + 93, 18, random.Next(9) - 284);
                case Laucher.Team.Thief:
                    return new Vector3(random.Next(17) + 45, 18, random.Next(20) - 300);
            }

            return new Vector3(62, 18, -281);
        }
    }
}
