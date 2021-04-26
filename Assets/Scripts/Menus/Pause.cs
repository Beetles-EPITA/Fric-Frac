using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus
{
    public class Pause : MonoBehaviour
    {

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

        private void Update()
        {
            Cursor.visible = isPause;
        }

        public void setPause(bool pause)
        {
            isPause = pause;
        }
        
        public void Exit()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
