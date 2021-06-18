using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Menus
{
    public class ItemListMenu : MonoBehaviour
    {

        [SerializeField] private GameObject itemPrefab;
        
        public static ItemListMenu Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void CreateList(Dictionary<string, int> items)
        {
            foreach (Item components in GetComponentsInChildren<Item>())
            {
                Destroy(components.gameObject);
            }
            foreach (var item in items)
            {
                GameObject gameObject = Instantiate(itemPrefab, transform);
                TMP_Text text = gameObject.GetComponent<TMP_Text>();
                text.text = "- " + (item.Value > 1 ? item.Value + " " : "") + item.Key;
                gameObject.name = item.Key;
                gameObject.AddComponent<Item>();
            }
        }

        public void UpdateList(Dictionary<string, int> items)
        {
            foreach (Item components in GetComponentsInChildren<Item>())
            {
                TMP_Text text = components.GetComponent<TMP_Text>();
                string name = components.gameObject.name;
                if (items.ContainsKey(name))
                    text.text = "- " + (items[name] > 1 ? items[name] + " " : "") + name;
                else
                    text.fontStyle = FontStyles.Strikethrough;
            }
        }
    }
}
