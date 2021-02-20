using System;
using UnityEditor;
using UnityEngine;

namespace Menus
{
    
    // https://youtu.be/zPZK7C5_BQo?t=988
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance;
        [SerializeField] Menu[] menus;

        private void Awake()
        {
            Instance = this;
        }

        public void OpenMenu(Menu menu)
        {
            foreach (var menuList in menus)
            {
                if(menuList != menu) menuList.Close();
            }
            menu.Open();
        }

        public void CloseMenu(Menu menu)
        {
            
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
