using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public Config Config;
	private string selectedLevelName;
	private Dictionary<string, LevelData> levels;
	public static GameManager instance;
	[SerializeField] private InputManager inputManager;
	public InputManager InputManager
	{
		get { return inputManager; }
	}
	[SerializeField] private MainMenu mainMenu;
	[SerializeField] private LevelManager levelManager;


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
			levels = Resources.LoadAll(Defines.ResourcePaths.levelsFolder, typeof(LevelData)).ToDictionary(x => x.name, x => x as LevelData);
			Debug.Assert(levels.Count > 0);
		}
		else
		{
			Debug.LogError("Multiple game managers in scene");
			gameObject.SetActive(false);
		}
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == Defines.SceneNames.mainMenu)
		{
			inputManager = GameObject.FindObjectOfType<InputManager>();
			mainMenu = FindObjectOfType<MainMenu>();
			mainMenu.GenerateDropdown(levels.Values.ToArray());

			if (inputManager != null)
			{
				inputManager.Init(Config.ControlScheme);
			}
			
			Debug.Assert(inputManager != null);
			Debug.Assert(mainMenu != null);
		}
		else if(scene.name == Defines.SceneNames.game)
		{
			if (inputManager == null)
			{
				inputManager = GameObject.FindObjectOfType<InputManager>();
				if (inputManager != null)
				{
					inputManager.Init(Config.ControlScheme);
				}
			}
			levelManager = FindObjectOfType<LevelManager>();
			levelManager.Init(levels[selectedLevelName]);
			
			Debug.Assert(inputManager != null);
			Debug.Assert(levelManager != null);
		}
	}

	public void LoadLevel(string levelName)
	{
		selectedLevelName = levelName;
		SceneManager.LoadScene(Defines.SceneNames.game);
		
	}
}
