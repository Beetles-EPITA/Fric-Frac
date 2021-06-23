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
        public void SetUp(string message, bool victory, bool isOwner)
        {
            title.text = victory ? "Victory" : "Defeat";
            this.message.text = message;
            GetComponent<Image>().color = victory ? Color.green : Color.red;
            if (isOwner) waiting.gameObject.SetActive(false); else replay.gameObject.SetActive(false);
            GetComponent<Menu>().Open();
            Destroy(RoomManager.Instance.LoseScreen.gameObject);
            Destroy(pause.gameObject);
            Destroy(tab.gameObject);
            Inventory.Instance.Close();
            if (PlayerController.myController != null)
            {
                Transform controllerTransform = PlayerController.myController.cameraHolder.transform;
                Transform cameraTransform = RoomManager.Instance.spectatorCamera.transform;
                cameraTransform.position = controllerTransform.position;
                cameraTransform.rotation = controllerTransform.rotation;
                Destroy(PlayerController.myController.gameObject);
            }
                
            
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