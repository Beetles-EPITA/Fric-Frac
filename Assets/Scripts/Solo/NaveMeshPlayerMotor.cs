using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NaveMeshPlayerMotor : MonoBehaviour
{
    [SerializeField]
    public NavMeshAgent agent;
    
    [SerializeField]
    public Component player;

    // Update is called once per frame
    void Update()
    {
        
        if (IaStatesMachine.Distance(agent, player, 225f) && !IaStatesMachine.IsObjectBetween(agent, player))
        {
            agent.SetDestination(player.transform.localPosition);
        }
    }
}
