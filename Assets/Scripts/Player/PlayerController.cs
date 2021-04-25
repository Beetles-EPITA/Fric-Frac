using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float mouseSensitivity, sprintSpeed, walkSpeed, jumpFoce, smoothTime;

    private Rigidbody _rigidbody;

    private float _verticalLookRotation;
    private bool _grounded;
    private Vector3 _smoothMoveVelocity;
    private Vector3 _moveAmount;

    private PhotonView _photonView;
    
    //Sound:
    [SerializeField] private AudioSource _audioSource;
    private soundState audioState = soundState.standBy;
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

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;
        Look();
        Move();
        Jump();
        SoundManager();
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (RoomManager.Instance != null) Destroy(RoomManager.Instance.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        
        if (!_photonView.IsMine)
        {
            Destroy(cameraHolder);
            Destroy(_rigidbody);
        }
    }


    /**
     * Move
     */
    private void Move()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        _moveAmount = Vector3.SmoothDamp(_moveAmount,
            moveDirection * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref _smoothMoveVelocity,
            smoothTime);

        anim.SetFloat("Speed", Math.Max(Math.Abs(_moveAmount.x),(Math.Abs(_moveAmount.z))));
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
        if (Input.GetKeyDown(KeyCode.Space) && _grounded)
        {
            _rigidbody.AddForce(transform.up * jumpFoce);
            anim.SetTrigger(jumpHash);
        }
        audioState = soundState.jump;
    }

    private void SoundManager()
    {
        soundState oldSoundState = audioState;
        if (anim.speed == 0)
        {
            audioState = soundState.standBy;
        }
        if (anim.speed != 0 && anim.speed < 3.1)
        {
            audioState = soundState.walk;
        }
        if (anim.speed >= 3.1)
        {
            audioState = soundState.run;
        }

        if (oldSoundState != audioState)
        {
            _audioSource.Stop();
            switch (audioState)
            {
                case soundState.standBy:
                    //play stanby
                    break;
                case soundState.walk:
                    //play walk
                    break;
                case soundState.run:
                    //play run
                    break;
                case soundState.jump:
                    //play jump
                    break;
                default:
                    throw new Exception("sound manager goes brrr");
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
        _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
    }

}
