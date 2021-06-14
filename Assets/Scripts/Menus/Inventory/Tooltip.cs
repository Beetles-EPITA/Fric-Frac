using System;
using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.EventSystems;
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
            transform.position = Input.mousePosition + new Vector3(0, 20);
            gameObject.SetActive(true);
        }

        public IEnumerator Hide()
        {
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);
        }
    }
}