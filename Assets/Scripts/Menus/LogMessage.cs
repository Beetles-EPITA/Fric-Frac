using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class LogMessage : MonoBehaviourPun
    {

        public static LogMessage Instance;
        [SerializeField] private GameObject messagePrefab;

        private void Awake()
        {
            Instance = this;
        }


        public static void Send(string message, bool time = true)
        {
            Instance.photonView.RPC("ShowMessage", RpcTarget.All, message, time);
        }

        public static void Clear()
        {
            Instance.photonView.RPC("ClearMessages", RpcTarget.All);
        }

        [PunRPC]
        public void ShowMessage(string message, bool time)
        {
            GameObject messageObject = Instantiate(messagePrefab, transform);
            messageObject.GetComponentInChildren<Text>().text = message;
            if(time) StartCoroutine(HideMessage(messageObject));
        }

        [PunRPC]
        public void ClearMessages()
        {
            foreach (var componentsInChild in GetComponentsInChildren<Text>())
            {
                Destroy(componentsInChild);
            }
        }

        IEnumerator HideMessage(GameObject messageObject)
        {
            yield return new WaitForSeconds(7);
            Destroy(messageObject);
        }
        
    }
}