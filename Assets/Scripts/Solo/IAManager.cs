using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manager;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

public class IAManager : MonoBehaviour
{
    Random r = new Random();
    private Vector3[] playerPosition = 
    {
        new Vector3(85.8f, 23.7f, -292.7f),
        new Vector3(81.8f, 27.7f, -292.2f),
    };
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Instantiated IAManager");
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "IA", "IAController"), playerPosition[r.Next(playerPosition.Length)],Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
