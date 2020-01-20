using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsDialog : Dialog
    {
        private void Awake()
        {
            Debug.Assert(musicSlider);
            Debug.Assert(musicSlider);
        }

        public override void Open()
        {
            mAudioManager = GameManager.Instance.AudioManager;

            musicSlider.value = mAudioManager.GetVolume(AudioGroup.Music);
            sfxSlider.value = mAudioManager.GetVolume(AudioGroup.Sfx);

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Event handler for music slider
        /// </summary>
        /// <param name="value">Slider value</param>
        public void OnMusicVolumeChanged(float value)
        {
            mAudioManager.SetVolume(AudioGroup.Music, value);
        }
        
        /// <summary>
        /// Event handler for sfx slider
        /// </summary>
        /// <param name="value">Slider value</param>
        public void OnSfxVolumeChanged(float value)
        {
            mAudioManager.SetVolume(AudioGroup.Sfx, value);
        }

        private AudioManager mAudioManager;
        
        [SerializeField] private Slider musicSlider;
        
        [SerializeField] private Slider sfxSlider;
    }
}