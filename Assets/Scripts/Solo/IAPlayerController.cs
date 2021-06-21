using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using Menus;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace Solo
{
    public class IAPlayerController : MonoBehaviour
    {
        [SerializeField] public NavMeshAgent navMeshAgent;
        [SerializeField] public Camera agentCamera;
        
        [SerializeField] public float minDistanceCloseToHear;
        [SerializeField] public float minDistanceCloseToSee;
        
        Random r = new Random();
    
        //Animation :
        private Animator anim;
        
        //STATE
        private PlayerController target;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!navMeshAgent.isOnNavMesh)
            {
                print("Agent lost at:" + navMeshAgent.transform.position.x + ", " + navMeshAgent.transform.position.y + ", " + navMeshAgent.transform.position.z);
            }

            target = GetTheCloserPlayer();

            print(Distance(navMeshAgent.transform, GetTheCloserPlayer().transform) + " see:" + CanSee(target)+ " hear:" + CanHear(target));
            if (!navMeshAgent.hasPath)
            {
                if (Distance(target.transform, navMeshAgent.transform) < minDistanceCloseToHear)
                {
                    //navMeshAgent.SetDestination(target.transform.position);
                }
            }

        }

        public PlayerController GetTheCloserPlayer()
        {
            PlayerController closer = null;
            PlayerController[] players = (PlayerController[])GameObject.FindObjectsOfType(typeof(PlayerController));
            foreach (var player in players)
            {
                if (players.Length != 0)
                    closer = players[0];
                if (player.Team == Laucher.Team.Thief && Distance(navMeshAgent.transform, player.transform) <
                    Distance(navMeshAgent.transform, closer.transform))
                    closer = player;
            }
            return closer;
        }
        
        public float Distance(Transform ia, Transform player)
        {
            Vector3 a = ia.position;
    
            Vector3 b = player.position;
            return Vector3.Distance(a, b);
        }


        public bool CanSee(PlayerController player)
        {
            Vector3 screenPoint = agentCamera.WorldToViewportPoint(player.transform.position);
            return (Distance(navMeshAgent.transform, player.transform) < minDistanceCloseToSee) && !IsAnObjectBetween(target) && (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1);
        }

        public bool CanHear(PlayerController player)
        {
            return Distance(navMeshAgent.transform, player.transform) < minDistanceCloseToHear;
        }

        public bool IsAnObjectBetween(PlayerController player)
        {
            Ray r = new Ray(agentCamera.transform.position,  player.cameraHolder.transform.position - agentCamera.transform.position);
            RaycastHit raycastHit;
            return Physics.Raycast(r, out raycastHit, Distance(agentCamera.transform, player.cameraHolder.transform)) && raycastHit.transform.GetComponentInParent<PlayerController>() == null;
        }
    }
}