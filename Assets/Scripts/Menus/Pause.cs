using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus
{
    public class Pause : MonoBehaviour
    {
        
        [SerializeField] private Menu settingsMenu;
        
        public static bool isPause;

        public static Pause Instance;
        
        private void Awake()
        {
            isPause = false;
            Instance = this;
        }

        private void Start()
        {
            if(GetComponent<FinalScreen>() == null)
                gameObject.SetActive(false);
        }

        public void setPause(bool pause)
        {
            if (GetComponent<FinalScreen>() != null)
                return;
            isPause = pause;
            if (!isPause)
            {
                gameObject.SetActive(false);
                settingsMenu.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        
        public void Exit()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public static void Open()
        {
            Instance.GetComponent<Menu>().Open();
        }
    }
}
