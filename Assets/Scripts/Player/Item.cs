
using System;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class Item : MonoBehaviour
{

    [SerializeField] public string itemName;
    [SerializeField] public Sprite icon;

    [PunRPC]
    public void Delete()
    {
        if (gameObject != null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        
    }

}
