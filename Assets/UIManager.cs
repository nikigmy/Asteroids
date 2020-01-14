using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private MainMenu mainMenu;
    private GameUI gameUI;

    public void Init(Scene scene)
    {
        if (scene.name == Defines.SceneNames.mainMenu)
        {
            mainMenu = Utils.FindOrLoad<MainMenu>(Defines.ResourcePaths.mainMenuPrefabPath);
            
            if (gameUI != null)
            {
                gameUI.Disable();
            }
        }
        else if (scene.name == Defines.SceneNames.game)
        {
            gameUI = Utils.FindOrLoad<GameUI>(Defines.ResourcePaths.gameUIPrefabPath);

            if (mainMenu != null)
            {
                mainMenu.Disable();
            }
        }
    }

    public void LoadGameUI(LevelManager levelManager)
    {
        if (mainMenu != null)
        {
            mainMenu.Disable();
        }
        gameUI.Init(levelManager);
    }

    public void LoadMainMenu(LevelData[] levels)
    {
        if (gameUI != null)
        {
            gameUI.Disable();
        }
        mainMenu.Init(levels);
    }
}
