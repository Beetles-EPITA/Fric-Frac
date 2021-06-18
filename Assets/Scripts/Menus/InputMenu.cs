using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class InputMenu : MonoBehaviour
    {
        private Event keyEvent;
        private Text buttonText;
        private KeyCode newKey;

        private bool waitingForKey;
        
        private void Start()
        {
            gameObject.SetActive(false);
            waitingForKey = false;
            foreach (var componentsInChild in GetComponentsInChildren<Button>())
            {
                if (Enum.TryParse(componentsInChild.gameObject.name, out GameManager.KeyType type))
                {
                    componentsInChild.GetComponentInChildren<TMP_Text>().text =
                        GameManager.Instance.inputs[type].ToString();
                }
            }
        }
    }
}