using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class Tooltip : MonoBehaviour
    {

        private Text _text;
        public static Tooltip Instance;
        
        private void Awake()
        {
            _text = GetComponentInChildren<Text>();
            Instance = this;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.position = Input.mousePosition + new Vector3(0, 20);
        }

        public void Show(ItemContent item)
        {
            _text.text = item.item.itemName;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}