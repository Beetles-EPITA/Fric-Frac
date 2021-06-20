
using System;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class Item : MonoBehaviour
{

    [SerializeField] public string itemName;
    [SerializeField] public GameObject objectPrefab;
    [SerializeField] public Sprite icon;

    [PunRPC]
    public void Delete()
    {
        PhotonNetwork.Destroy(gameObject);
    }

}
