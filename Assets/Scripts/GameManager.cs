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
			mainMenu = FindObjectOfType<MainMenu>();
			mainMenu.GenerateDropdown(levels.Values.ToArray());
		}
		else if(scene.name == Defines.SceneNames.game)
		{
			
			levelManager = FindObjectOfType<LevelManager>();
			levelManager.Init(levels[selectedLevelName]);
		}
	}

	public void LoadLevel(string levelName)
	{
		selectedLevelName = levelName;
		SceneManager.LoadScene(Defines.SceneNames.game);
		
	}
}
