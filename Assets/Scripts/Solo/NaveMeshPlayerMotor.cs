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

    //Animation :
    private Animator anim;
    private float speed;
    
    // Update is called once per frame
    void Update()
    {
        anim = GetComponent<Animator>();
        
        if (IaStatesMachine.Distance(agent, player, 225f) && !IaStatesMachine.IsObjectBetween(agent, player))
        {
            agent.SetDestination(player.transform.localPosition);
        }

        if (agent.hasPath)
        {
            anim.SetFloat("Speed", 3);
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }

    }
}
