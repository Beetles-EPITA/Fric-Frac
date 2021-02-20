using System;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Menus
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text serverName;
        [SerializeField] private TMP_Text playersNumber;

        private RoomInfo _roomInfo;
        
        public void SetUp(RoomInfo roomInfo)
        {
            _roomInfo = roomInfo;
            serverName.text = _roomInfo.Name;
            playersNumber.text = _roomInfo.PlayerCount + "\nPLAYERS";
        }

        public void OnClick()
        {
            Laucher.Instance.JoinRoom(_roomInfo);
        }
    }
}
