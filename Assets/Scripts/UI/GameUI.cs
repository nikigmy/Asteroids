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
        [SerializeField] private Transform heartContainer;
        private LevelManager levelManager;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI scoreText;

        public void Show(LevelManager levelManager)
        {
            gameObject.SetActive(true);
            this.levelManager = levelManager;

            levelManager.OnHealthChanged += OnHealthChanged;
            levelManager.OnScoreChanged += OnScoreChanged;
            levelManager.OnLevelStarted += OnLevelStarted;

            UpdateHearths(levelManager.Health);
            UpdateScore(levelManager.Score);
            UpdateLevelNumber();
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
            levelText.text = "Level: " + GameManager.instance.CurrentLevelNumber;
        }

        private void UpdateScore(int score)
        {
            scoreText.text = "Score: " + levelManager.Score;
        }

        private void UpdateHearths(int health)
        {
            var hearts = heartContainer.childCount;
            if (health < hearts)
            {
                for (var i = hearts - 1; i > health - 1; i--)
                {
                    var child = heartContainer.GetChild(i).gameObject;
                    GameManager.instance.PoolManager.ReturnObject(Constants.PoolKeys.heart, child);
                }
            }
            else if (health > hearts)
            {
                var heartObjects =
                    GameManager.instance.PoolManager.RetrieveObjects<GameObject>(Constants.PoolKeys.heart, health - hearts);
                for (var i = 0; i < health - hearts; i++)
                {
                    heartObjects[i].transform.parent = heartContainer;
                    heartObjects[i].SetActive(true);
                }
            }
        }

        public void OnSettingButtonClicked()
        {
            OpenSettings();
        }

        private void OpenSettings()
        {
            GameManager.instance.UIManager.OpenSetting();
        }


        public void OnMainMenuClicked()
        {
            GameManager.instance.LoadMainMenu();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}