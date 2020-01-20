using System;
using Game;
using UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Assert(mainMenu != null);
            Debug.Assert(gameUI != null);
            Debug.Assert(gameEndScreen != null);
            Debug.Assert(settingsDialog != null);
        }

        /// <summary>
        /// Initialise the manager
        /// </summary>
        public void Init()
        {
            Debug.Assert(mainMenu != null);
            Debug.Assert(gameUI != null);
            Debug.Assert(gameEndScreen != null);

            settingsDialog.OnDialogClosed += SettingsClosed;
        }

        /// <summary>
        /// Show the main menu
        /// </summary>
        public void ShowMainMenu()
        {
            mainMenu.Show();
        }

        /// <summary>
        /// Hide the main menu
        /// </summary>
        public void HideMainMenu()
        {
            mainMenu.Hide();
        }


        /// <summary>
        /// Show the game UI
        /// </summary>
        /// <param name="levelManager">Level manager</param>
        public void ShowGameUI(LevelManager levelManager)
        {
            gameUI.Show(levelManager);
        }

        /// <summary>
        /// Hide the game UI
        /// </summary>
        public void HideGameUI()
        {
            gameUI.Hide();
        }
        /// <summary>
        /// Show the game end screen
        /// </summary>
        /// <param name="score">Score result</param>
        /// <param name="win">Whether the player won</param>
        /// <param name="newHighScore">Whether the player achieved new highscore</param>
        public void ShowGameEndScreen(int score, bool win, bool newHighScore)
        {
            gameEndScreen.ShowGameEndScreen(score, win, newHighScore);
        }

        /// <summary>
        /// Hide the game end screen
        /// </summary>
        public void HideGameEndScreen()
        {
            gameEndScreen.Hide();
        }

        /// <summary>
        /// Open the settings
        /// </summary>
        public void OpenSetting()
        {
            GameManager.Instance.PauseGame();
            settingsDialog.Open();
        }

        private void SettingsClosed(object sender, EventArgs e)
        {
            GameManager.Instance.UnpauseGame();
        }

        [SerializeField] private MainMenu mainMenu;

        [SerializeField] private GameUI gameUI;
        
        [SerializeField] private GameEndScreen gameEndScreen;
        
        [SerializeField] private SettingsDialog settingsDialog;
    }
}