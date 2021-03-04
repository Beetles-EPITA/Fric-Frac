using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Menus
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text serverName;
        [SerializeField] private TMP_Text playersNumber;

        public RoomInfo roomInfo;
        
        public void SetUp(RoomInfo roomInfo)
        {
            this.roomInfo = roomInfo;
            serverName.text = roomInfo.Name;
            playersNumber.text = roomInfo.PlayerCount + "\nPLAYERS";
        }

        public void OnClick()
        {
            if (!PhotonNetwork.InRoom)
            {
                Laucher.Instance.JoinRoom(roomInfo);
            }
        }
    }
}
