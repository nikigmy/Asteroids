using Managers;
using UnityEngine;
using Utils;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        private AudioSource menuMusicSource;

        public void Show()
        {
            menuMusicSource = GameManager.instance.AudioManager.Play(Constants.AudioKeys.menuMusic,
                AudioGroup.Music, true, Vector3.zero);
            gameObject.SetActive(true);
        }

        public void StartClicked()
        {
            GameManager.instance.StartGame();
        }

        public void Hide()
        {
            GameManager.instance.PoolManager.ReturnObject(menuMusicSource);
            gameObject.SetActive(false);
        }
    }
}