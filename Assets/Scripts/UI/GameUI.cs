using System;
using Game;
using Managers;
using TMPro;
using UnityEngine;
using Utils;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Assert(heartContainer != null);
            Debug.Assert(levelText != null);
            Debug.Assert(scoreText != null);
        }

        /// <summary>
        /// Show the game UI
        /// </summary>
        /// <param name="levelManager">Level manager</param>
        public void Show(LevelManager levelManager)
        {
            gameObject.SetActive(true);
            this.mLevelManager = levelManager;

            levelManager.OnHealthChanged += OnHealthChanged;
            levelManager.OnScoreChanged += OnScoreChanged;
            levelManager.OnLevelStarted += OnLevelStarted;

            UpdateHearths(levelManager.Health);
            UpdateScore(levelManager.Score);
            UpdateLevelNumber();
        }
        
        /// <summary>
        /// Hide the game UI
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Event handler for the settings button
        /// </summary>
        public void OnSettingButtonClicked()
        {
            GameManager.Instance.UIManager.OpenSetting();
        }
        
        /// <summary>
        /// Event handler for the main menu button
        /// </summary>
        public void OnMainMenuClicked()
        {
            GameManager.Instance.LoadMainMenu();
        }

        private void OnLevelStarted(object sender, EventArgs eventArgs)
        {
            UpdateLevelNumber();
        }

        private void OnScoreChanged(object sender, ValueArgs<int> valueArgs)
        {
            UpdateScore(valueArgs.Value);
        }

        private void OnHealthChanged(object sender, ValueArgs<int> valueArgs)
        {
            UpdateHearths(valueArgs.Value);
        }

        private void UpdateLevelNumber()
        {
            levelText.text = "Level: " + GameManager.Instance.CurrentLevelNumber;
        }

        private void UpdateScore(int score)
        {
            scoreText.text = "Score: " + mLevelManager.Score;
        }

        private void UpdateHearths(int health)
        {
            var hearts = heartContainer.childCount;
            if (health < hearts)
            {
                for (var i = hearts - 1; i > health - 1; i--)
                {
                    var child = heartContainer.GetChild(i).gameObject;
                    GameManager.Instance.PoolManager.ReturnObject(Constants.PoolKeys.cHeart, child);
                }
            }
            else if (health > hearts)
            {
                var heartObjects =
                    GameManager.Instance.PoolManager.RetrieveObjects<GameObject>(Constants.PoolKeys.cHeart, health - hearts);
                for (var i = 0; i < health - hearts; i++)
                {
                    heartObjects[i].transform.SetParent(heartContainer);
                    heartObjects[i].SetActive(true);
                }
            }
        }
        
        private LevelManager mLevelManager;
        
        [SerializeField] private Transform heartContainer;
        
        [SerializeField] private TextMeshProUGUI levelText;
        
        [SerializeField] private TextMeshProUGUI scoreText;

    }
}