using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class FinalScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text message;
        [SerializeField] private Button replay;
        [SerializeField] private Button waiting;

        [SerializeField] private Menu pause;
        [SerializeField] private Menu tab;

        [Header("Sound Win and Lose")]
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip loseClip;
        [SerializeField] private AudioClip winClip;

        private bool _victory;

        private IEnumerator PlaySound()
        {
            PlayerController playerController = PlayerController.myController;
            playerController._audioSource.Stop();
            playerController._audioSource.clip = _victory ? playerController.winSound : playerController.looseSound;
            playerController._audioSource.Play();
            print(1);
            yield return new WaitForSecondsRealtime(2);
            print(2);
        }
        
        public void SetUp(string message, bool victory, bool isOwner)
        {
            StartCoroutine(PlaySound());
            print("acces");
            title.text = victory ? "Victory" : "Defeat";
            _victory = victory;
            this.message.text = message;
            GetComponent<Image>().color = victory ? Color.green : Color.red;
            if (isOwner) waiting.gameObject.SetActive(false); else replay.gameObject.SetActive(false);
            GetComponent<Menu>().Open();
            Destroy(RoomManager.Instance.LoseScreen.gameObject);
            Destroy(pause.gameObject);
            Destroy(tab.gameObject);
            Inventory.Instance.Close();
            if(PlayerController.myController != null)
                Destroy(PlayerController.myController.gameObject);
            RoomManager.Instance.spectatorCamera.gameObject.AddComponent<AudioListener>();
            _audioSource.PlayOneShot(victory ? winClip : loseClip);
            Pause.isPause = true;
            
            RoomManager.Instance.crosshair.gameObject.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (isOwner)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
        }

        public void Relaunch()
        {
            PhotonNetwork.LoadLevel("Multiplayer");
        }

    }
}