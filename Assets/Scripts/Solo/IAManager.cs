using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manager;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

public class IAManager : MonoBehaviour
{
    Random random = new Random();
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "IA", "IAController"), new Vector3(random.Next(12) + 93, 18, random.Next(9) - 284), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
