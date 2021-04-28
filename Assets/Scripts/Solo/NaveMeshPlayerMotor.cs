using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class NaveMeshPlayerMotor : MonoBehaviour
{
    [SerializeField]
    public NavMeshAgent agent;
    
    [SerializeField]
    public Component player;
    
    [SerializeField]
    public float minDistanceCloseToTrigger;
    Random r = new Random();

    //Animation :
    private Animator anim;
    
    // Update is called once per frame
    void Update()
    {
        anim = GetComponent<Animator>();
        
        if (IaStatesMachine.Distance(agent, player, minDistanceCloseToTrigger) && !IaStatesMachine.IsObjectBetween(agent, player))
        {
            agent.SetDestination(player.transform.localPosition);
        }
        else
        {
            Vector3 v = new Vector3(r.Next(22), 0 , r.Next(22));
            v.x *= (r.Next(1) == 1 ? 1 : -1);
            v.y *= (r.Next(1) == 1 ? 1 : -1);
            agent.SetDestination(v);
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
