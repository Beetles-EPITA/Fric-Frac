﻿using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class TabMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI serverName;
        [SerializeField] private TextMeshProUGUI ping;
        [SerializeField] private TextMeshProUGUI timer;
        [SerializeField] private Transform resident;
        [SerializeField] private Transform thief;
        
        [SerializeField] private GameObject playerListItemPrefab;

        private float startTime;

        public static Menu InstanceMenu;

        private void Awake()
        {
            startTime = Time.time;
            InstanceMenu = GetComponent<Menu>();
        }

        private void Start()
        {
            InstanceMenu.Close();
            serverName.text = "Server: " + PhotonNetwork.CurrentRoom.Name;
            Update();
        }

        private void Update()
        {
            timer.text = $"{Mathf.Floor((Time.time-startTime) / 60):0}:{(Time.time-startTime) % 60:00}";
            ping.text = PhotonNetwork.GetPing() + "ms";
        }

        private void UpdateTab()
        {
            foreach (Transform transform in resident)
            {
                Destroy(transform.gameObject);
            }
            foreach (Transform transform in thief)
            {
                Destroy(transform.gameObject);
            }
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if (player.Value.CustomProperties["team"].Equals(Laucher.Team.Resident))
                {
                    Instantiate(playerListItemPrefab, resident).GetComponent<PlayerListItem>().SetUp(player.Value);
                }
                else if (player.Value.CustomProperties["team"].Equals(Laucher.Team.Thief))
                {
                    Instantiate(playerListItemPrefab, thief).GetComponent<PlayerListItem>().SetUp(player.Value);
                }
            }
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            UpdateTab();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            UpdateTab();
        }
    }
}