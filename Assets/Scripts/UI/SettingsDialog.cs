using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsDialog : Dialog
    {
        private AudioManager audioManager;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        public override void Open()
        {
            audioManager = GameManager.instance.AudioManager;

            musicSlider.value = audioManager.GetVolume(AudioManager.AudioGroup.Music);
            sfxSlider.value = audioManager.GetVolume(AudioManager.AudioGroup.Sfx);

            gameObject.SetActive(true);
        }

        public void OnMusicVolumeChanged(float value)
        {
            audioManager.SetVolume(AudioManager.AudioGroup.Music, value);
        }

        public void OnSfxVolumeChanged(float value)
        {
            audioManager.SetVolume(AudioManager.AudioGroup.Sfx, value);
        }
    }
}