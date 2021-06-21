using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class LoseScreenMenu : MonoBehaviour
    {

        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text message;
        [SerializeField] private Button backButton;
        
        
        public void SetUp(string title, string message, bool endGame)
        {
            gameObject.SetActive(true);
            this.title.text = title;
            this.message.text = message;
            backButton.gameObject.SetActive(!endGame);
            Pause.isPause = true;
            Cursor.visible = true;
            RoomManager.Instance.crosshair.gameObject.SetActive(false);
        }

        public void Exit()
        {
            GetComponent<Menu>().Close();
            Transform transform = RoomManager.Instance.spectatorCamera.transform;
            transform.position = new Vector3(5, 51, -333);
            transform.rotation = Quaternion.Euler(new Vector3(31,58,0));
            Pause.isPause = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }
}