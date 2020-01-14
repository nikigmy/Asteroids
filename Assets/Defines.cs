using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defines
{
	public enum EffectType
	{
		Speed,
		Shield,
		HomingAmmo
	}

	public delegate void OnScoreChanged(int score);
	

	public delegate void OnHealthChanged(int health);
	public delegate void OnPlayerHit();
	public delegate void OnAsteroidDestroyed(Asteroid asteroid, Projectile projectile);
	public delegate void OnEnemyHit();

	public delegate void OnHit();
	public delegate void DirectionalInputDelegate(Vector2 inputVector);
	public delegate void ButtonInputDelegate();
	public delegate void LevelCompletedDelegate(int livesLeft);
	public delegate void GameOverDelegate(int score);
	public delegate void OnDestroyedDelegate(object[] args);
	public delegate void OnAsteroidDestroyedDelegate(GameObject target, int level, Vector3 hitDir);
//	public delegate void OnDestroyDelegate();
	public class ResourcePaths
	{
		public static string uiHearthPrefabPath = "Prefabs/UI/Heart";
		public const string gameUIPrefabPath = "Prefabs/UI/GameUI";
		public const string mainMenuPrefabPath = "Prefabs/UI/MainMenu";
		public const string asteroidPrefabPath = "Prefabs/Gameplay/Asteroid";
		public const string bulletPrefabPath = "Prefabs/Gameplay/Bullet";
		public const string flyingSaucerPrefabPath = "Prefabs/Gameplay/FlyingSaucer";
		public const string shipPrefabPath = "Prefabs/Gameplay/Ship";
		public static string shieldPickup = "Prefabs/Gameplay/Pickups/ShieldPickup";
		public static string speedBoostPickup = "Prefabs/Gameplay/Pickups/SpeedBoostPickup";
		public static string homingAmmoPickup = "Prefabs/Gameplay/Pickups/HomingAmmoPickup";

		public const string levelsFolder = "Levels";
	}

	public class SceneNames
	{
		public const string game = "Game";
		public const string mainMenu = "MainMenu";
	}

	public enum PoolKey
	{
		Heart,
		ShieldPickup,
		SpeedBoostPickup,
		HomingAmmoPickup
	}

	public enum ControlScheme
	{
		Desktop,
		Mobile,
		MobileArcade,
	}
	
	public enum ProjectileMode
	{
		Friendly,
		Enemy
	}
	
	public class Tags
	{
		public const string player = "Player";
		public const string boundary = "Boundary";
		public const string projectile = "Projectile";
		public const string asteroid = "Asteroid";
		public const string enemy = "Enemy";
		public const string pickup = "Pickup";
	}
}
