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

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
        audioState = soundState.standBy;
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
            print(audioState.ToString());

            _audioSource.Stop();
            switch (audioState)
            {
                case soundState.standBy:
                    //_audioSource.clip = standByClip;
                    _audioSource.Play();
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
        _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
    }

}
