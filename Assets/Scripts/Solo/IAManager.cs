using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manager;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

public class IAManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "IA", "IAController"), PlayerManager.RandomPosition(), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
