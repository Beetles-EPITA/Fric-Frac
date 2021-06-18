using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class LogMessage : MonoBehaviourPun
    {

        private static LogMessage Instance;
        [SerializeField] private GameObject messagePrefab;

        private void Awake()
        {
            Instance = this;
        }


        public static void SendMessage(string message)
        {
            Instance.photonView.RPC("ShowMessage", RpcTarget.All, message);
        }

        [PunRPC]
        public void ShowMessage(string message)
        {
            GameObject messageObject = Instantiate(messagePrefab, transform);
            messageObject.GetComponentInChildren<Text>().text = message;
            StartCoroutine(HideMessage(messageObject));
        }

        IEnumerator HideMessage(GameObject messageObject)
        {
            yield return new WaitForSeconds(3);
            Destroy(messageObject);
        }
        
    }
}