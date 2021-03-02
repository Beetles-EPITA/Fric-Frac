using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NaveMeshPlayerMotor : MonoBehaviour
{
    public NavMeshAgent agent;
    
    [SerializeField]
    public Component player;

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.transform.localPosition);

    }
}
