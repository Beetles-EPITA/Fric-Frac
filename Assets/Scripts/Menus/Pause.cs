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
            gameObject.SetActive(false);
        }

        public void setPause(bool pause)
        {
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
    }
}
