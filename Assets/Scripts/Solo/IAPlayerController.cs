using System.Security.Cryptography;
using System;
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
        [SerializeField]
        public NavMeshAgent navMeshAgent;
        [SerializeField]
        public Camera agentCamera;


        public PlayerController target;
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
    
            if (!navMeshAgent.isOnNavMesh)
            {
                print("Agent lost at:" + navMeshAgent.transform.position.x + ", " + navMeshAgent.transform.position.y + ", " + navMeshAgent.transform.position.z);
            }
            
           
        }
        
        public static bool Distance(Component ia, Component player, float distanceMin)
        {
            Vector3 a = ia.transform.position;
    
            Vector3 b = player.transform.position;
            return Vector3.Distance(a, b) < distanceMin;
        }
    
        
        
        public static bool IsObjectBetween(Component ia, Component player)
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