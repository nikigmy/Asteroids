using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
	public event Defines.OnScoreChanged ScoreChanged; 
	public event Defines.OnHealthChanged HealthChanged; 
	public event Defines.LevelCompletedDelegate OnLevelComplete;
	public event Defines.GameOverDelegate OnGameOver;
	private List<Asteroid> activeAsteroids;
	private ShipController shipController;
	private int score;
	private int lives;
	private LevelData level;
	
	private PoolManager poolManager;
	[SerializeField] private CollisionManager collisionManager;

	[SerializeField] private LevelFactory levelFactory;
	[SerializeField] private BoundarySetter boundarySetter;
	[SerializeField] private CameraSetter cameraSetter;
	public Vector2 FieldPosition { get; private set; }
	public Vector2 FieldSize { get; private set; }
	public CollisionManager CollisionManager
	{
		get { return collisionManager;}
		protected set { collisionManager = value; }
	}
	
	public PoolManager PoolManager
	{
		get { return poolManager; }
		protected set { poolManager = value; }
	}

	public int Score
	{
		get { return score; }
	}
	public int Health
	{
		get { return lives; }
	}
	public LevelData Level
	{
		get { return level; }
	}

	public void Init(LevelData level)
	{
		this.level = level;
		lives = GameManager.instance.Config.startHealth;
		activeAsteroids = new List<Asteroid>();
		boundarySetter = FindObjectOfType<BoundarySetter>();
		cameraSetter = FindObjectOfType<CameraSetter>();
		levelFactory = FindObjectOfType<LevelFactory>();
		collisionManager = FindObjectOfType<CollisionManager>();
			
		PoolManager = levelFactory.CreatePoolManager();

		var cam = Camera.main;
		cam.transform.position = Vector3.back * 100;
		FieldPosition = Vector2.zero;

		var aspectRatio = (float) Screen.width / Screen.height;
		var fieldHeight = cam.orthographicSize * 2;
		FieldSize = new Vector2(fieldHeight * aspectRatio, fieldHeight);

		boundarySetter.SetBoundaries(Vector2.zero, FieldSize);
		cameraSetter.PositionCameras(Vector3.back * 100, FieldSize);

		var createdLevelElements = levelFactory.CreateLevel(level, PoolManager);

		shipController = createdLevelElements.Item1;
		activeAsteroids.AddRange(createdLevelElements.Item2);
		
		collisionManager.OnAsteroidDestroyed += OnAsteroidDestroyed;
		collisionManager.OnPlayerHit += TakeDamage;
//		collisionManager.OnEnemyHit += Defines.OnEnemyHit;
	}

	private UnityEvent Event;

	public void OnAsteroidDestroyed(Asteroid asteroid, Projectile projectile)
	{
		int[] asteroidRewards = new[] {1000, 500, 100};
		activeAsteroids.Remove(asteroid);

		var hitDir = projectile.transform.position - asteroid.transform.position;

		if (asteroid.Level > 1)
		{
			SplitAsteroid(asteroid.Level, asteroid.transform.position, hitDir);
//				GameManager.instance.StartCoroutine(SplitAsteroid(asteroid.Level, asteroid.transform.position, hitDir));
		}
		else if (!activeAsteroids.Any())
		{
			OnLevelComplete?.Invoke(lives);
		}

		if (projectile.Mode == Defines.ProjectileMode.Friendly)
		{
			if (Random.Range(0, 20) == 0)
			{
				Defines.PoolKey key = Defines.PoolKey.ShieldPickup;
				var pickupIndex = Random.Range(0, 1);
				switch (pickupIndex)
				{
					case 0:
						key = Defines.PoolKey.ShieldPickup;
						break;
					case 1:
						key = Defines.PoolKey.HomingAmmoPickup;
						break;
					case 2:
						key = Defines.PoolKey.SpeedBoostPickup;
						break;
				}

				Debug.LogError("Fix this hardcode");
				var pickup = GameManager.instance.LevelManager.PoolManager.RetrieveObject<Pickup>(key.ToString());
				pickup.transform.position = asteroid.transform.position;
			}

			AddScore(asteroidRewards[asteroid.Level - 1]);
		}
		GameManager.instance.LevelManager.PoolManager.ReturnObject(projectile);
		GameManager.instance.LevelManager.PoolManager.ReturnObject(asteroid);
	}

	private void AddScore(int addedScore)
	{
		score += addedScore;
		ScoreChanged?.Invoke(score);
	}

	private void TakeDamage()
	{
		lives--;
		HealthChanged?.Invoke(lives);
		if (lives > 0)
		{
			shipController.ResetShip();
		}
		else
		{
			PoolManager.ReturnObject(shipController);
			OnGameOver?.Invoke(score);
		}
	}

	//
	void SplitAsteroid( int level, Vector3 spawnPos, Vector3 hitDir)
	{
		foreach (var rend in FindObjectsOfType<Asteroid>())
		{
			var lineRenderer = rend.GetComponent<LineRenderer>();	
			lineRenderer.startColor = Color.white;
			lineRenderer.endColor = Color.white;
		}
		transform.forward = Vector3.back;
//		yield return new WaitForSeconds(0.03f);

		var nextLevel = level - 1;
		var asteroids = GameManager.instance.LevelManager.PoolManager.RetrieveObjects<Asteroid>( 2);
		var rot = Quaternion.Euler(new Vector3(0, 0, (Mathf.Atan(hitDir.y / hitDir.x) * Mathf.Rad2Deg) + 90));
		
		foreach (var asteroidObject in asteroids)
		{
			asteroidObject.transform.rotation = rot;
			
			var asteroid = asteroidObject.GetComponent<Asteroid>();
			
			asteroid.SetData(nextLevel, UnityEngine.Random.Range(GameManager.instance.Config.asteroidMinSpeed, GameManager.instance.Config.asteroidMaxSpeed));
			asteroidObject.transform.position = spawnPos;
			asteroidObject.gameObject.SetActive(true);
			asteroidObject.GetComponent<LineRenderer>().endColor =
				asteroidObject.GetComponent<LineRenderer>().startColor = Color.red;
			activeAsteroids.Add(asteroidObject);
		}
		
		asteroids[0].transform.Rotate(Vector3.forward, 180);
	}
}
