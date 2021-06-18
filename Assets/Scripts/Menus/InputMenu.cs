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
                    componentsInChild.onClick.AddListener(() => SetButtonText(tmpText));
                    componentsInChild.onClick.AddListener(() => StartAssignment(type));
                }
            }
        }

        private void OnGUI()
        {
            keyEvent = Event.current;
            if (keyEvent.isKey && waitingForKey)
            {
                waitingForKey = false;
                newKey = keyEvent.keyCode;
            }
        }

        public void SetButtonText(TMP_Text text)
        {
            buttonText = text;
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
        }
        
    }
}