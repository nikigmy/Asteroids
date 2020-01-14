using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public Config Config;
	private int currentLevelIndex;
	private LevelData[] levels;
	public static GameManager instance;
	private InputManager inputManager;
	private UIManager uiManager;
	public InputManager InputManager
	{
		get { return inputManager; }
	}
	private LevelManager levelManager;


	public LevelManager LevelManager
	{
		get { return levelManager; }
	}

	private void Awake()
	{
		SceneManager.sceneLoaded += SceneLoaded;
		if (instance == null)
		{
			instance = this;
			levels = Resources.LoadAll(Defines.ResourcePaths.levelsFolder, typeof(LevelData)).Select(x => x as LevelData).ToArray();
		}
		else
		{
			gameObject.SetActive(false);
			Destroy(gameObject);
		}
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SceneManager.SetActiveScene(scene);
		
//		uiManager = new UIManager(scene);
		uiManager = Utils.FindOrCreate<UIManager>();
		uiManager.Init(scene);
		
		inputManager = Utils.FindOrCreate<InputManager>(new Type[]{typeof(global::DontDestroyOnLoad)});
			
		inputManager.Init(Config.ControlScheme);
		
		if (scene.name == Defines.SceneNames.mainMenu)
		{
			uiManager.LoadMainMenu(levels);
		}
		else if(scene.name == Defines.SceneNames.game)
		{
			if (levelManager == null)
			{
				levelManager = Utils.FindOrCreate<LevelManager>();
				levelManager.OnGameOver += OnGameOver;
				levelManager.OnLevelComplete += OnLevelCompleted;
			}

			levelManager.Init(levels[currentLevelIndex]);
			uiManager.LoadGameUI(levelManager);
		}
	}

	private void OnLevelCompleted(int livesleft)
	{
		LoadLevel(currentLevelIndex + 1);
	}

	private void OnGameOver(int score)
	{
		LoadMainMenu();
	}

	public void LoadLevel(int levelIndex)
	{
		currentLevelIndex  = levelIndex;
		var scene = SceneManager.GetSceneByName(Defines.SceneNames.game);
		if (!scene.isLoaded)
		{
			SceneManager.LoadScene(Defines.SceneNames.game, LoadSceneMode.Additive);
		}
		else
		{
			levelManager.Init(levels[levelIndex]);
			uiManager.LoadGameUI(levelManager);
		}
	}

	public void LoadMainMenu()
	{
		var scene = SceneManager.GetSceneByName(Defines.SceneNames.mainMenu);
		if (scene.isLoaded)
		{
			uiManager.LoadMainMenu(levels);
		}
		else
		{
			SceneManager.LoadScene(Defines.SceneNames.mainMenu, LoadSceneMode.Additive);
		}
	}

	public void StartGame()
	{
		LoadLevel(0);
	}
}
