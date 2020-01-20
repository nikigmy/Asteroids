using Managers;
using TMPro;
using UnityEngine;
using Utils;
using Debug = System.Diagnostics.Debug;

namespace UI
{
    public class GameEndScreen : MonoBehaviour
    {
        private const string cGameOverText = "GAME OVER";
        private const string cYouWinText = "YOU WIN";

        private static readonly int Show = Animator.StringToHash("Show");

        private void Awake()
        {
            Debug.Assert(gameEndCanvasGroup != null);
            Debug.Assert(gameEndText != null);
            Debug.Assert(newHighScoreObject != null);
            Debug.Assert(scoreText != null);
            Debug.Assert(youDiedAnimator != null);
        }

        /// <summary>
        /// Show the game end screen
        /// </summary>
        /// <param name="score">Score result</param>
        /// <param name="win">Whether the player won</param>
        /// <param name="newHighScore">Whether the player achieved new highscore</param>
        public void ShowGameEndScreen(int score, bool win, bool newHighScore)
        {
            gameObject.SetActive(true);
            scoreText.text = "Score: " + score;
            newHighScoreObject.SetActive(newHighScore);

            var audioManager = GameManager.Instance.AudioManager;
            if (win)
            {
                
                audioManager.Play(Constants.AudioKeys.cGameComplete, AudioGroup.Sfx, false,
                    Vector3.zero);
                gameEndText.text = cYouWinText;
                SetGameEndCanvasState(true);
            }
            else
            {
                audioManager.Play(Constants.AudioKeys.cGameOver, AudioGroup.Sfx, false,
                    Vector3.zero);
                SetGameEndCanvasState(false);
                gameEndText.text = cGameOverText;

                youDiedAnimator.SetTrigger(Show);
            }
        }

        /// <summary>
        /// Hide the screen
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Animation event handler for the you died animation
        /// </summary>
        public void OnYouDiedAnimationEnded()
        {
            SetGameEndCanvasState(true);
        }
        
        /// <summary>
        /// Event handler for the play button
        /// </summary>
        public void OnPlayClicked()
        {
            GameManager.Instance.StartGame();
        }

        /// <summary>
        /// Event handler for the main menu button
        /// </summary>
        public void OnMenuClicked()
        {
            GameManager.Instance.LoadMainMenu();
        }

        private void SetGameEndCanvasState(bool enable)
        {
            gameEndCanvasGroup.interactable = enable;
            gameEndCanvasGroup.alpha = enable ? 1 : 0;
        }


        [SerializeField] private CanvasGroup gameEndCanvasGroup;
        
        [SerializeField] private TextMeshProUGUI gameEndText;
        
        [SerializeField] private GameObject newHighScoreObject;
        
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [SerializeField] private Animator youDiedAnimator;

    }
}