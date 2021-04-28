using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class Inventory : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GridLayoutGroup layout;
        [SerializeField] private GameObject itemPrefab;
        
        private PlayerController _player;
        private Menu _menu;
        public bool inInventory;

        public static Inventory Instance;

        private void Awake()
        {
            _menu = GetComponent<Menu>();
            Instance = this;
            inInventory = false;
        }
        
        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Open(PlayerController player)
        {
            _player = player;
            UpdateItems();
            _menu.Open();
            inInventory = true;
            Pause.isPause = true;
        }

        public void Close()
        {
            _menu.Close();
            inInventory = false;
            Pause.isPause = false;
            Tooltip.Instance.Hide();
        }

        private void UpdateItems()
        {
            foreach (Transform transform in layout.transform)
            {
                Destroy(transform.gameObject);
            }
            foreach (Item item in _player.Items)
            {
                Instantiate(itemPrefab, layout.transform).GetComponent<ItemContent>().SetUp(item);
            }
        }
    }
}