using System;
using Game;
using UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameEndScreen gameEndScreen;
        [SerializeField] private GameUI gameUI;
        [SerializeField] private MainMenu mainMenu;

        [SerializeField] private SettingsDialog settingsDialog;

        public void Init()
        {
            Debug.Assert(mainMenu != null);
            Debug.Assert(gameUI != null);
            Debug.Assert(gameEndScreen != null);

            settingsDialog.OnDialogClosed += SettingClosed;
        }

        private void SettingClosed(object sender, EventArgs e)
        {
            GameManager.instance.UnpauseGame();
        }

        public void ShowGameUI(LevelManager levelManager)
        {
            gameUI.Show(levelManager);
        }

        public void HideGameUI()
        {
            gameUI.Hide();
        }

        public void ShowMainMenu()
        {
            mainMenu.Show();
        }

        public void HideMainMenu()
        {
            mainMenu.Hide();
        }

        public void ShowGameEndScreen(int score, bool gameOver, bool newHighScore)
        {
            gameEndScreen.ShowGameEndScreen(score, gameOver, newHighScore);
        }

        public void HideGameEndScreen()
        {
            gameEndScreen.Hide();
        }

        public void OpenSetting()
        {
            GameManager.instance.PauseGame();
            settingsDialog.Open();
        }
    }
}