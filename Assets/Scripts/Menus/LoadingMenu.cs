using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class LoadingMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform loadingIcon;
        [SerializeField] private TextMeshProUGUI loadingText;

        [SerializeField] private float timeStep;
        [SerializeField] private float timeMessageStep;
        [SerializeField] private float oneStepAngle;
        [SerializeField] private string[] messages;
        
        private float _startTime;
        private float _messageTime;

        void Start()
        {
            _startTime = Time.time;
            _messageTime = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                if (Time.time - _startTime >= timeStep)
                {
                    Vector3 iconAngle = loadingIcon.localEulerAngles;
                    iconAngle.z -= oneStepAngle;

                    loadingIcon.localEulerAngles = iconAngle;

                    _startTime = Time.time;
                }

                if (Time.time - _messageTime >= timeMessageStep)
                {
                    int random = new System.Random().Next(messages.Length);

                    loadingText.text = messages[random];
                    
                    _messageTime = Time.time;
                }
            }
            else
            {
                _messageTime = 0;
            }
        }
    }
}
