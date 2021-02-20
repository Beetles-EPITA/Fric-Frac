using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    
    // https://youtu.be/zPZK7C5_BQo?t=988
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance;
        private Menu[] menus;
        private Text version;
        
        
        private void Awake()
        {
            Instance = this;
            menus = GetComponentsInChildren<Menu>(true);
            version = (Text) GetComponentInChildren(typeof(Text));
        }
        
        private void Start()
        {
            version.text = "Version " + Application.version;
            
        }

        public void OpenMenu(Menu menu)
        {
            foreach (var menuList in menus)
            {
                if(menuList != menu) menuList.Close();
            }
            menu.Open();
        }

        public Menu OpenMenu(string name)
        {
            foreach (Menu menu in menus)
            {
                if (menu.name == name)
                {
                    OpenMenu(menu);
                    return menu;
                }
            }

            return null;
        }

        public void CloseMenu(Menu menu)
        {
            
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
        
        
        /**
         * Input Manager
         */
        
        public void UpperCaseInput(InputField inputField)
        {
            inputField.text = inputField.text.ToUpper();
        }

        public void SimpleCaseInput(InputField inputField)
        {
            string outputFilter = "";
            foreach (char c in inputField.text)
            {
                if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z')
                {
                    outputFilter += c;
                }
            }
            inputField.text = outputFilter;
        }

        public void UpdateDisableButton(InputField inputField)
        {
            Button button = (Button) inputField.GetComponentInChildren(typeof(Button));
            button.interactable = inputField.text != "";
        }

        
    }
}
