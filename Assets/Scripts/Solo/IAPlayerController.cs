using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
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
        private Vector3[] listOfPosition = new[]
        {
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
        };

        [SerializeField]private GameObject resident;
        [SerializeField] private Material _material;

        private void Start()
        {
            anim = GetComponent<Animator>();
            resident.GetComponent<Renderer>().materials[3] = _material;
        }

        // Update is called once per frame
        void Update()
        {
            if (!navMeshAgent.isOnNavMesh)
            {
                print("Agent lost at:" + navMeshAgent.transform.position.x + ", " + navMeshAgent.transform.position.y + ", " + navMeshAgent.transform.position.z);
            }
            target = GetTheCloserPlayer();
            if(target != null)
                Hit();
            if (CanSee(target) || CanHear(target))
            {
                navMeshAgent.SetDestination(target.transform.position);
            }
            else
            {
                
                if (!navMeshAgent.hasPath)
                {
                    
                }
            }
        }

        public PlayerController GetTheCloserPlayer()
        {
            PlayerController closer = null;
            PlayerController[] players = (PlayerController[])GameObject.FindObjectsOfType(typeof(PlayerController));
            if (players.Length != 0)
                closer = players[0];
            foreach (var player in players)
            {
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


        public void Hit()
        {
            Vector3 agentMiddle = new Vector3(agentCamera.transform.position.x, agentCamera.transform.position.y-1,agentCamera.transform.position.z);
            
            Ray ray = new Ray(agentMiddle, agentCamera.transform.forward);
            Debug.DrawRay(agentMiddle, agentCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                if (target != null && target.Team == Laucher.Team.Thief)
                {
                    if (target == hit.transform.gameObject.GetComponentInParent<PlayerController>())
                    {
                        PhotonView view = target.GetComponent<PhotonView>();
                        view.RPC("Lose", view.Controller, "Captured",
                            "You have been found by " + PhotonNetwork.LocalPlayer.NickName, false);
                        LogMessage.Send(view.Controller.NickName + " has been found by " +
                                        "the IA");
                        target.gameObject.SetActive(false);
                        Hashtable hashtable = view.Controller.CustomProperties;
                        hashtable["death"] = true;
                        view.Controller.SetCustomProperties(hashtable);
                        RoomManager.Instance.photonView.RPC("UpdateTab", RpcTarget.All);
                        RoomManager.Instance.photonView.RPC("CheckWin", RpcTarget.All, (int) Laucher.Team.Resident);
                    }
                }
            }
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