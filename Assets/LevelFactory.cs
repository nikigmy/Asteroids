using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Random = UnityEngine.Random;

public class LevelFactory : MonoBehaviour
{

	void Start()
	{
	}

	public PoolManager CreatePoolManager()
	{
		var obj = new GameObject("PoolManager");
		var poolManager = obj.AddComponent<PoolManager>();
		
		poolManager.RegisterGenerator(Defines.PoolKeys.bullet, new BasicObjectGenerator(Defines.ResourcePaths.bulletPrefabPath));
		poolManager.PopulatePool(Defines.PoolKeys.bullet, GameManager.instance.Config.bulletPool);
		
		poolManager.RegisterGenerator(Defines.PoolKeys.asteroid, new AsteroidGenerator());
		poolManager.PopulatePool(Defines.PoolKeys.asteroid, GameManager.instance.Config.minAsteroidPool);
		
		return poolManager;
	}

//	private void GenerateObjects(, int count)
//	{
//		
//	}

	public void CreateLevel(LevelData level, PoolManager poolManager, Defines.OnAsteroidDestroyedDelegate onAsteroidDestroyed)
	{
		var targetPool = (level.AsteroidCount * 4) * (GameManager.instance.Config.optimalAsteroidPoolPercentage / 100);
		poolManager.PopulatePoolToTarget(Defines.PoolKeys.asteroid, targetPool);

		var asteroidsToPlace = poolManager.RetrieveObjects(Defines.PoolKeys.asteroid, level.AsteroidCount);

		for (int i = 0; i < asteroidsToPlace.Length; i++)
		{
			var asteroid = asteroidsToPlace[i];
			
			var posX = Utils.GenerateRandomValue(1.5f, GameManager.instance.LevelManager.FieldSize.x / 2f);
			var posY = Utils.GenerateRandomValue(1.5f, GameManager.instance.LevelManager.FieldSize.y / 2f);
			asteroid.transform.position = new Vector3(posX, posY, 0);
			
			var asteroidRotation = new Vector3(0,
				0, Utils.GenerateRandomValue(0, 180));
			asteroid.transform.rotation = Quaternion.Euler(asteroidRotation);
			
			var speed = Random.Range(GameManager.instance.Config.asteroidMinSpeed,
				GameManager.instance.Config.asteroidMaxSpeed);
			asteroid.GetComponent<Asteroid>().SetData(GameManager.instance.Config.asteroidStartLevel, speed);
			
			asteroid.GetComponent<Asteroid>().OnAsteroidDestroyed += onAsteroidDestroyed;

			asteroid.name = i.ToString();
			asteroid.SetActive(true);
		}
	}
}
