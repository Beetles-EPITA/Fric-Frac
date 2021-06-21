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
            print(Distance(navMeshAgent.transform, GetTheCloserPlayer().transform));
            if (!navMeshAgent.hasPath)
            {
                target = GetTheCloserPlayer();
                if (Distance(target.transform, navMeshAgent.transform) < minDistanceCloseToHear)
                {
                    navMeshAgent.SetDestination(target.transform.position);
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
                if (player.team == Laucher.Team.Thief && Distance(navMeshAgent.transform, player.transform) <
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
        
        
        public bool IsObjectBetween(Component ia, Component player)
        {
            Ray ray = new Ray(ia.transform.position, player.transform.localPosition);
            RaycastHit hit;
    
            if (Physics.Raycast(ray, out hit))
            {
                return true;
            }
    
            return false;
        }
    }
}