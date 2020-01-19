using Managers;
using TMPro;
using UnityEngine;
using Utils;

namespace UI
{
    public class GameEndScreen : MonoBehaviour
    {
        private const string gameOverText = "GAME OVER";
        private const string youWinText = "YOU WIN";
        private static readonly int Show = Animator.StringToHash("Show");

        [SerializeField] private CanvasGroup gameEndCanvasGroup;
        [SerializeField] private TextMeshProUGUI gameEndText;
        [SerializeField] private GameObject newHighScoreObject;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Animator youDiedAnimator;

        public void ShowGameEndScreen(int score, bool gameOver, bool newHighScore)
        {
            gameObject.SetActive(true);
            scoreText.text = "Score: " + score;
            newHighScoreObject.SetActive(newHighScore);

            if (gameOver)
            {
                GameManager.instance.AudioManager.Play(Constants.AudioKeys.gameOver, AudioManager.AudioGroup.Sfx, false,
                    Vector3.zero);
                SetGameEndCanvasState(false);
                gameEndText.text = gameOverText;

                youDiedAnimator.SetTrigger(Show);
            }
            else
            {
                GameManager.instance.AudioManager.Play(Constants.AudioKeys.gameComplete, AudioManager.AudioGroup.Sfx, false,
                    Vector3.zero);
                gameEndText.text = youWinText;
                SetGameEndCanvasState(true);
            }
        }

        public void OnYouDiedAnimationEnded()
        {
            SetGameEndCanvasState(true);
        }

        private void SetGameEndCanvasState(bool enable)
        {
            gameEndCanvasGroup.interactable = enable;
            gameEndCanvasGroup.alpha = enable ? 1 : 0;
        }

        public void OnPlayClicked()
        {
            GameManager.instance.StartGame();
        }

        public void OnMenuClicked()
        {
            GameManager.instance.LoadMainMenu();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}