using UnityEngine;

namespace Menus
{
    public class VolumeSlider : MonoBehaviour
    {
        public string path;
        public void SetVolume(float volume)
        {
            SettingsMenu.Instance.SetVolume(volume, path);
        }
    }
}