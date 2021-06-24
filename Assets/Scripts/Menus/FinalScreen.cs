using System.Collections;
using Photon.Pun;
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
        
        public void SetUp(string message, bool victory, bool isOwner)
        {
            title.text = victory ? "Victory" : "Defeat";
            _victory = victory;
            this.message.text = message;
            GetComponent<Image>().color = victory ? Color.green : Color.red;
            if (isOwner) waiting.gameObject.SetActive(false); else replay.gameObject.SetActive(false);
            GetComponent<Menu>().Open();
            Destroy(RoomManager.Instance.LoseScreen.gameObject);
            Destroy(pause.gameObject);
            Destroy(tab.gameObject);
            Destroy(RoomManager.Instance.infoText);
            Inventory.Instance.Close();
            if (PlayerController.myController != null)
            {
                Transform controllerTransform = PlayerController.myController.cameraHolder.transform;
                Transform cameraTransform = RoomManager.Instance.spectatorCamera.transform;
                cameraTransform.position = controllerTransform.position;
                cameraTransform.rotation = controllerTransform.rotation;
                Destroy(PlayerController.myController.gameObject);
            }
            RoomManager.Instance.spectatorCamera.gameObject.AddComponent<AudioListener>();
            Camera.SetupCurrent(RoomManager.Instance.spectatorCamera);
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
            
            DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence
            {
                largeImageKey = "icon", largeImageText = Application.version, details = "In Waiting Room", 
                state = PhotonNetwork.CurrentRoom.Name + " server", 
                partySize = PhotonNetwork.CurrentRoom.PlayerCount,
                partyMax = PhotonNetwork.CurrentRoom.MaxPlayers
            };
            DiscordRpc.UpdatePresence(presence);
        }

        public void Relaunch()
        {
            RoomManager.Instance.photonView.RPC("Replay", RpcTarget.All);
        }

    }
}