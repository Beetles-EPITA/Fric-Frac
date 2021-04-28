using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static IaStatesMachine;
using Random = System.Random;

public class NaveMeshPlayerMotor : MonoBehaviour
{
    [SerializeField]
    public NavMeshAgent agent;

    [SerializeField]
    public Component player;
    
    [SerializeField]
    public float minDistanceCloseToHear;
    [SerializeField]
    public float minDistanceCloseToSee;
    
    Random r = new Random();

    //Animation :
    private Animator anim;
    
    // Update is called once per frame
    void Update()
    {
        anim = GetComponent<Animator>();

        if (!agent.isOnNavMesh)
        {
            print("Agent lost at:" + agent.transform.position.x + ", " + agent.transform.position.y + ", " + agent.transform.position.z);
        }
        
        if ((Distance(agent, player, minDistanceCloseToHear) || CanSeeThePlayer(agent, player, minDistanceCloseToSee)) && !IsObjectBetween(agent, player))
        {
            agent.SetDestination(player.transform.localPosition);
        }
        else
        {
            if (!agent.hasPath)
            {
                Vector3 v = new Vector3(r.Next(22), 0 , r.Next(22));
                v.x *= (r.Next(1) == 1 ? 1 : -1);
                v.z *= (r.Next(1) == 1 ? 1 : -1);
                agent.SetDestination(v);
                print(v.x + ", " + v.y + ", " +  v.z);
            }
            
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
