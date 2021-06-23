using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Menus;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public GameObject cameraHolder;
    [SerializeField] private float mouseSensitivity, sprintSpeed, walkSpeed, jumpFoce, smoothTime;

    public Laucher.Team Team;
    public static PlayerController myController;

    private Rigidbody _rigidbody;

    private float _verticalLookRotation;
    //private bool _grounded;
    private PlayerJumpAction _jumpAction;
    private Vector3 _smoothMoveVelocity;
    private Vector3 _moveAmount;

    private PhotonView _photonView;

    //Sound:
    [SerializeField] private AudioSource _audioSource;
    private soundState audioState;
    private enum soundState
    {
        standBy,
        walk,
        run,
        jump
    }

    [SerializeField] private AudioClip standByClip;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip walkInsideClip;
    [SerializeField] private AudioClip runClip;
    [SerializeField] private AudioClip runInsideClip;
    [SerializeField] private AudioClip JumpClip;
    
    [SerializeField] private GameObject mains;
    [SerializeField] private GameObject thief;
    [SerializeField] private GameObject resident;

    private bool inHouse;

    //Animation :
    private Animator anim;

    public List<Item> Items = new List<Item>();
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
        audioState = soundState.standBy;
        _jumpAction = GetComponentInChildren<PlayerJumpAction>();
    }

    private void Update()
    {
        print(inHouse);
        if (!_photonView.IsMine) return;
        ToggleInventory();
        PickItem();
        Hit();
        if (Pause.isPause)
        {
            anim.SetInteger("Speed", 0);
            _moveAmount = Vector3.zero;
            return;
        }
        Look();
        Move();
        Jump();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        Team = (Laucher.Team) _photonView.Owner.CustomProperties["team"];

        if (!_photonView.IsMine)
        {
            Destroy(cameraHolder);
            Destroy(Team == Laucher.Team.Resident ? thief.gameObject : resident.gameObject);
            
        }
        else
        {
            Camera.SetupCurrent(cameraHolder.GetComponent<Camera>());
            myController = this;
            mains.gameObject.SetActive(true);
            Destroy(resident.gameObject);
            Destroy(thief.gameObject);
        }
    }


    /**
     * Move
     */
    private void Move()
    {
        Vector3 moveDirection = GameManager.Instance.getMoveDirection().normalized;

        _moveAmount = Vector3.SmoothDamp(_moveAmount,
            moveDirection * (GameManager.Instance.GetKey(GameManager.KeyType.Sprint) ? sprintSpeed : walkSpeed), ref _smoothMoveVelocity,
            smoothTime);

        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            if (GameManager.Instance.GetKey(GameManager.KeyType.Sprint))
            {//sprint
                int speed = anim.GetInteger("Speed");
                anim.SetInteger("Speed", 2);
                if(speed != 2)
                    _photonView.RPC("PlaySound", RpcTarget.All, (int)soundState.run);
            }
            else
            {//walk
                int speed = anim.GetInteger("Speed");
                anim.SetInteger("Speed", 1);
                if(speed != 1)
                    _photonView.RPC("PlaySound", RpcTarget.All, (int)soundState.walk);
            }
        }
        else
        {
            int speed = anim.GetInteger("Speed");
            anim.SetInteger("Speed", 0);
            if(speed != 0)
                _photonView.RPC("PlaySound", RpcTarget.All, (int)soundState.standBy);
        }
    }
    
    /**
     * Look
     */
    private void Look()
    {
        transform.Rotate(Vector3.up * (Input.GetAxisRaw("Mouse X") * mouseSensitivity));
            
        _verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);
        cameraHolder.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
    }

    /**
     * Jump
    */

    private void Jump()
    {
        if (GameManager.Instance.GetKey(GameManager.KeyType.Jump) && _jumpAction.isOnGround)
        {
            _rigidbody.AddForce(transform.up * jumpFoce);
            StartCoroutine(JumpAnim());
        }
    }

    private IEnumerator JumpAnim()
    {
        anim.SetBool("isJumping", true);
        yield return new WaitForEndOfFrame();
        anim.SetBool("isJumping", false);
        _photonView.RPC("PlaySound", RpcTarget.All, (int)soundState.jump);
    }

    [PunRPC]
    private void PlaySound(int soundState)
    {
        //check if it is this
        soundState state = ((soundState) soundState);
        if(state != PlayerController.soundState.jump)
            _audioSource.Stop();
        switch (state)
        {
            case PlayerController.soundState.standBy:
                //already break
                break;
            case PlayerController.soundState.walk:
                _audioSource.clip = _jumpAction.inHouse? walkClip: walkInsideClip;
                _audioSource.Play();;
                break;
            case PlayerController.soundState.run:
                _audioSource.clip = _jumpAction.inHouse? runClip: runInsideClip;
                _audioSource.Play();
                break;
            case PlayerController.soundState.jump:
                //_audioSource.clip = JumpClip;
                _audioSource.PlayOneShot(JumpClip);
                break;
            default:
                throw new Exception("sound manager goes brrr");
                
        }
    }
    
    private IEnumerator HitAnim()
    {
        anim.SetBool("isAttacking", true);
        yield return new WaitForEndOfFrame();
        anim.SetBool("isAttacking", false);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        
    }

    private void OnCollisionExit(Collision other)
    {
        
    }

    private void OnCollisionStay(Collision other)
    {
        if(other.gameObject.GetComponentInParent<ColliderScript>() != null)
        {
            if (!inHouse)
            {
                inHouse = true;
            }
        }
        else
        {
            if (inHouse)
            {
                inHouse = false;
            }
        }
    }

    /**
     * Fix Movement
     */
    private void FixedUpdate()
    {
        if (!_photonView.IsMine) return;
        if (Pause.isPause) return;
        _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
    }

    /**
     * Inventory
     */

    private void ToggleInventory()
    {
        if (Input.GetKeyUp(GameManager.Instance.inputs[GameManager.KeyType.Inventory]))
        {
            if (!Inventory.Instance.inInventory && Pause.isPause) return;
            if(!Inventory.Instance.inInventory)
                Inventory.Instance.Open(this);
            else 
                Inventory.Instance.Close();
        }
        if (Input.GetKeyUp(KeyCode.Escape) && Inventory.Instance.inInventory)
        {
            Inventory.Instance.Close();
        }
    }

    private Outline lastHitObject;
    
    private void Hit()
    {
        if (Team == Laucher.Team.Resident)
        {
            if (lastHitObject != null)
            {
                lastHitObject.enabled = false;
            }
            Ray ray = new Ray(cameraHolder.transform.position, cameraHolder.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                PlayerController target = hit.transform.gameObject.GetComponentInParent<PlayerController>();
                if (target != null && target.Team == Laucher.Team.Thief)
                {
                    if (Input.GetKeyDown(GameManager.Instance.inputs[GameManager.KeyType.Interaction]) &&
                        !Pause.isPause)
                    {
                        PhotonView view = target.GetComponent<PhotonView>();
                        view.RPC("Lose", view.Controller, "Captured",
                            "You have been found by " + PhotonNetwork.LocalPlayer.NickName, false);
                        LogMessage.Send(view.Controller.NickName + " has been found by " +
                                        PhotonNetwork.LocalPlayer.NickName);
                        target.gameObject.SetActive(false);
                        Hashtable hashtable = view.Controller.CustomProperties;
                        hashtable["death"] = true;
                        view.Controller.SetCustomProperties(hashtable);
                        RoomManager.Instance.photonView.RPC("UpdateTab", RpcTarget.All);
                        RoomManager.Instance.photonView.RPC("CheckWin", RpcTarget.All, (int) Laucher.Team.Resident);
                    }
                }
                StartCoroutine(HitAnim());
            }
        }
    }
    
    [PunRPC]
    private void Lose(string title, string message, bool endGame)
    {
        RoomManager.Instance.LoseScreen.SetUp(title, message, endGame);
        Camera.SetupCurrent(RoomManager.Instance.spectatorCamera);
        Transform cameraTransform = RoomManager.Instance.spectatorCamera.transform;
        cameraTransform.position = cameraHolder.transform.position;
        cameraTransform.rotation = cameraHolder.transform.rotation;
        PhotonNetwork.Destroy(gameObject);
    }

    
    private Outline lastPickObject;
    
    private void PickItem()
    {
        if (Team == Laucher.Team.Thief)
        {
            RaycastHit hit;
            Ray ray = new Ray(cameraHolder.transform.position, cameraHolder.transform.forward);
            if (lastPickObject != null)
            {
                lastPickObject.enabled = false;
            }
            
            if (Physics.Raycast(ray, out hit))
            {
                
                GameObject target = hit.transform.gameObject;
                
                if(target != null && target.GetComponentInParent<Item>() != null)
                {
                    
                    if (Input.GetMouseButtonDown(1) && !Pause.isPause)
                    {
                        Items.Add(target.GetComponentInParent<Item>());
                        RoomManager.Instance.photonView.RPC("RemoveItem", RpcTarget.All, target.GetComponentInParent<Item>().itemName, true);
                        PhotonView view = target.GetComponentInParent<PhotonView>();
                        view.RPC("Delete", view.Controller);
                        view.gameObject.SetActive(false);
                        StartCoroutine(HitAnim());
                    }
                    else
                    {
                        Outline outline = target.GetComponentInParent<Outline>();
                        if (outline != null)
                        {
                            outline.enabled = true;
                            lastPickObject = outline; 
                        }
                    }
                }
            }
        }
    }
}
