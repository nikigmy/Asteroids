using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defines {
	
	public delegate void DirectionalInputDelegate(Vector2 inputVector);
	public delegate void ButtonInputDelegate();
	public delegate void OnDestroyedDelegate(object[] args);
	public delegate void OnAsteroidDestroyedDelegate(GameObject target, int level, Vector3 hitDir);
//	public delegate void OnDestroyDelegate();
	public class ResourcePaths
	{
		public const string asteroidPrefabPath = "Prefabs/Gameplay/Asteroid";
		public const string bulletPrefabPath = "Prefabs/Gameplay/Bullet";
		public const string shipPrefabPath = "Prefabs/Gameplay/Ship";
		public const string levelsFolder = "Levels";
	}

	public class SceneNames
	{
		public const string game = "Game";
		public const string mainMenu = "MainMenu";
	}

	public enum PoolKey
	{
		Bullet,
		Asteroid
	}

	public enum ControlScheme
	{
		Desktop,
		Mobile,
		MobileArcade,
	}
}
