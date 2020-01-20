using Managers;
using UnityEngine;
using Utils;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        /// <summary>
        /// Show the main menu
        /// </summary>
        public void Show()
        {
            mMenuMusicSource = GameManager.Instance.AudioManager.Play(Constants.AudioKeys.cMenuMusic,
                AudioGroup.Music, true, Vector3.zero);
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Hide the main menu
        /// </summary>
        public void Hide()
        {
            GameManager.Instance.PoolManager.ReturnObject(mMenuMusicSource);
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Event handler for the start button
        /// </summary>
        public void OnStartClicked()
        {
            GameManager.Instance.StartGame();
        }
        
        /// <summary>
        /// Event handler for the exit button
        /// </summary>
        public void OnExitClicked()
        {
            Application.Quit();
        }
        
        private AudioSource mMenuMusicSource;
    }
}