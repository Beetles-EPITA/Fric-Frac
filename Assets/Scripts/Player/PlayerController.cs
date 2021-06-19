using System;
using System.Collections;
using System.Collections.Generic;
using Menus;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float mouseSensitivity, sprintSpeed, walkSpeed, jumpFoce, smoothTime;

    private Laucher.Team _team;

    private Rigidbody _rigidbody;

    private float _verticalLookRotation;
    private bool _grounded;
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
    [SerializeField] private AudioClip runClip;
    [SerializeField] private AudioClip JumpClip;
    
    //Animation :
    private Animator anim;
    private int jumpHash = Animator.StringToHash("Jump");

    public List<Item> Items = new List<Item>();
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
        audioState = soundState.standBy;
    }

    private void Update()
    {
        SoundManager();
        if (!_photonView.IsMine) return;
        ToggleInventory();
        PickItem();
        Hit();
        if (Pause.isPause)
        {
            anim.SetFloat("Speed", 0);
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
        _team = (Laucher.Team) _photonView.Owner.CustomProperties["team"];
        
        if (!_photonView.IsMine)
        {
            Destroy(cameraHolder);
        }
        else
        {
            Camera.SetupCurrent(cameraHolder.GetComponent<Camera>());
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
            {
                anim.SetFloat("Speed", sprintSpeed);
            }
            else
            {
                anim.SetFloat("Speed", walkSpeed);
            }
        }
        else
        {
            anim.SetFloat("Speed", 0);
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
        if (GameManager.Instance.GetKey(GameManager.KeyType.Jump) && _grounded)
        {
            _rigidbody.AddForce(transform.up * jumpFoce);
            anim.SetTrigger(jumpHash);
        }
    }

    private void SoundManager()
    {
        soundState oldSoundState = audioState;
        if (_moveAmount.magnitude <= 0.1)
        {
            audioState = soundState.standBy;
        }
        if (_moveAmount.magnitude >=0.2 && anim.speed < 3.1)
        {
            audioState = soundState.walk;
        }
        if (_moveAmount.magnitude >= 3.1)
        {
            audioState = soundState.run;
        }

        if (!_grounded) //working
        {
            audioState = soundState.jump;
        }

        if (oldSoundState != audioState)
        {
            _audioSource.Stop();
            switch (audioState)
            {
                case soundState.standBy:
                    //_audioSource.clip = standByClip;
                    //_audioSource.Play();
                    break;
                case soundState.walk:
                    _audioSource.clip = walkClip;
                    _audioSource.Play();;
                    break;
                case soundState.run:
                    _audioSource.clip = runClip;
                    _audioSource.Play();
                    break;
                case soundState.jump:
                    _audioSource.clip = JumpClip;
                    _audioSource.Play();
                    break;
                default:
                    throw new Exception("sound manager goes brrr");
            }
        }
        else
        {
            if (!_audioSource.isPlaying && audioState != soundState.jump && audioState != soundState.standBy)
            {
                _audioSource.Play();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != gameObject)
        {
            _grounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != gameObject)
        {
            _grounded = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != gameObject)
        {
            _grounded = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != gameObject)
        {
            _grounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject != gameObject)
        {
            _grounded = false;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject != gameObject)
        {
            _grounded = true;
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

    private void Hit()
    {
        if (_team == Laucher.Team.Resident)
        {
            if (Input.GetKeyDown(GameManager.Instance.inputs[GameManager.KeyType.Interaction]) && !Pause.isPause)
            {
                Ray ray = new Ray(cameraHolder.transform.position, cameraHolder.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 10f))
                {
                    PlayerController target = hit.transform.gameObject.GetComponentInParent<PlayerController>();
                    if (target != null && target._team == Laucher.Team.Thief)
                    {
                        _photonView.RPC("Lose", _photonView.Controller, "Captured", "You have been found by " + PhotonNetwork.LocalPlayer.NickName, false);
                        LogMessage.SendMessage(_photonView.Controller.NickName + "has been found by " + PhotonNetwork.LocalPlayer.NickName);
                        //TODO CHECK WIN
                    }

                }
            }
        }
    }

    [PunRPC]
    private void Lose(string title, string message, bool endGame)
    {
        RoomManager.Instance.LoseScreen.SetUp(title, message, endGame);
        Camera.SetupCurrent(RoomManager.Instance.spectatorCamera);
        PhotonNetwork.Destroy(gameObject);
    }

    
    private Outline lastPickObject;
    
    private void PickItem()
    {
        if ((int) PhotonNetwork.LocalPlayer.CustomProperties["team"] == (int) Laucher.Team.Thief)
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
                        PhotonNetwork.Destroy(target.GetComponentInParent<Item>().gameObject);
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
