using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Menus
{
    public class InputMenu : MonoBehaviour
    {
        private Event keyEvent;
        private TMP_Text buttonText;
        private Button button;
        private KeyCode newKey;

        private bool waitingForKey;
        
        private void Start()
        {
            waitingForKey = false;
            foreach (var componentsInChild in GetComponentsInChildren<Button>())
            {
                if (Enum.TryParse(componentsInChild.gameObject.name, out GameManager.KeyType type))
                {
                    TMP_Text tmpText = componentsInChild.GetComponentInChildren<TMP_Text>();
                    tmpText.text =
                        GameManager.Instance.inputs[type].ToString();
                    componentsInChild.onClick.AddListener(() => SetButton(tmpText, componentsInChild));
                    componentsInChild.onClick.AddListener(() => StartAssignment(type));
                }
            }
        }

        private void OnGUI()
        {
            keyEvent = Event.current;
            if ((keyEvent.isKey || keyEvent.isMouse) && waitingForKey)
            {
                waitingForKey = false;
                if (keyEvent.isKey)
                    newKey = keyEvent.keyCode;
                else
                    newKey = (KeyCode) 323 + keyEvent.button;
            }
        }

        public void SetButton(TMP_Text text, Button buttonComponent)
        {
            buttonComponent.interactable = false;
            buttonText = text;
            button = buttonComponent;
        }

        public void StartAssignment(GameManager.KeyType keyType)
        {
            if (!waitingForKey)
                StartCoroutine(AssignKey(keyType));
        }

        public IEnumerator AssignKey(GameManager.KeyType keyType)
        {
            waitingForKey = true;
            
            yield return new WaitUntil(() => !waitingForKey);
            if (newKey != KeyCode.Escape)
            {
                GameManager.Instance.SetKey(keyType, newKey);
                buttonText.text = newKey.ToString();
            }
            button.interactable = true;
        }
        
    }
}