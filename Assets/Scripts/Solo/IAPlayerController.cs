using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
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
        private static Vector3[] listOfPosition = new[]
        {
            //rdc
            new Vector3(74.9f, 18.3f, -272.1f),
            new Vector3(75.8f, 18.3f, -287.4f),
            new Vector3(74.2f, 18.3f, -306.1f),
            new Vector3(86.3f, 18.3f, -304.6f),
            new Vector3(87.9f, 18.3f, 291.8f),
            new Vector3(91.0f, 18.3f, 283.8f),
            //1er
            new Vector3(80.1f, 23.1f, -285.5f),
            new Vector3(82.0f, 23.1f, -287.0f),
            new Vector3(95.0f, 23.1f, -285.3f),
            new Vector3(92.0f, 23.1f, -290.7f),
            new Vector3(76.0f, 23.1f, -290.6f),
            new Vector3(75.0f, 23.1f, -308.9f),
            new Vector3(90.7f, 23.1f, -300.9f),
            new Vector3(89.1f, 23.1f, -310.3f),
            //2eme
            new Vector3(80.8f, 27.7f, -308.2f), 
            new Vector3(75.4f, 27.7f, -300.5f), 
            new Vector3(82.0f, 27.7f, -300.8f), 
            new Vector3(93.7f, 27.7f, -289.4f), 
            new Vector3(92.2f, 27.7f, -280.8f), 
            new Vector3(86.6f, 27.7f, -280.3f), 
            new Vector3(79.5f, 27.7f, -272.1f), 
            new Vector3(81.0f, 27.7f, -285.1f), 
        };
        private List<Vector3> listOfPosionTemp = new List<Vector3>(listOfPosition);

        [SerializeField]private GameObject resident;
        [SerializeField] private Material _material;

        private void Start()
        {
            anim = GetComponent<Animator>();
            resident.GetComponent<Renderer>().materials[3] = _material;
            anim.SetInteger("Speed", 1);
        }

        // Update is called once per frame
        void Update()
        {
            if (!navMeshAgent.isOnNavMesh)
            {
                print("Agent lost at:" + navMeshAgent.transform.position.x + ", " + navMeshAgent.transform.position.y + ", " + navMeshAgent.transform.position.z);
            }
            target = GetTheCloserPlayer();
            if (target == null) return;
            Hit();
            if (CanSee(target) || CanHear(target))
            {
                navMeshAgent.SetDestination(target.transform.position);
            }
            else
            {
                if (!navMeshAgent.hasPath)
                {
                    if (listOfPosionTemp.Count == 0)
                    {
                        listOfPosionTemp = new List<Vector3>(listOfPosition);
                    }
                    Vector3 move = listOfPosionTemp[r.Next(listOfPosition.Length)];
                    navMeshAgent.SetDestination(move);
                    listOfPosionTemp.Remove(move);
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
            Debug.DrawRay(agentMiddle, agentCamera.transform.forward, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
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