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
        }
        
    }
}