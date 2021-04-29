using UnityEngine;

namespace Menus
{
    public class VolumeSlider : MonoBehaviour
    {
        public string path;
        public void SetVolume(float volume)
        {
            SettingsMenu.SetVolume(volume, path);
        }
    }
}