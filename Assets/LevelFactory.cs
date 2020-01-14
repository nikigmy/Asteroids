using System;
using System.Collections;
using System.Collections.Generic;
using Game;
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

		poolManager.RegisterGenerator(new GenericObjectGenerator<ShipController>(Defines.ResourcePaths.shipPrefabPath));
		
//		poolManager.RegisterGenerator(new ObjectGenerator<FlyingSaucer>(Defines.ResourcePaths.flyingSaucerPrefabPath));
//		poolManager.PopulatePool(Defines.PoolKey.FlyingSaucer, GameManager.instance.Config.flyingSaucerPool);
		
		poolManager.RegisterGenerator(Defines.PoolKey.ShieldPickup.ToString(), new GenericObjectGenerator<Pickup>(Defines.ResourcePaths.shieldPickup));
		poolManager.RegisterGenerator(Defines.PoolKey.HomingAmmoPickup.ToString(), new GenericObjectGenerator<Pickup>(Defines.ResourcePaths.homingAmmoPickup));
		poolManager.RegisterGenerator(Defines.PoolKey.SpeedBoostPickup.ToString(), new GenericObjectGenerator<Pickup>(Defines.ResourcePaths.speedBoostPickup));
		poolManager.RegisterGenerator(new GenericObjectGenerator<BulletController>(Defines.ResourcePaths.bulletPrefabPath));
		poolManager.PopulatePool<BulletController>(GameManager.instance.Config.bulletPool);
		
		poolManager.RegisterGenerator(new AsteroidGenerator(Defines.ResourcePaths.asteroidPrefabPath));
		poolManager.PopulatePool<Asteroid>(GameManager.instance.Config.minAsteroidPool);

		return poolManager;
	}

	public Tuple<ShipController, Asteroid[]> CreateLevel(LevelData level, PoolManager poolManager)
	{
		var targetAsteroidPool = (level.AsteroidCount * 4) * (GameManager.instance.Config.optimalAsteroidPoolPercentage / 100);
		var targetPickupPool = targetAsteroidPool / 20;
		
		poolManager.PopulatePoolToTarget<Asteroid>(targetAsteroidPool);
		poolManager.PopulatePool<Pickup>(Defines.PoolKey.ShieldPickup.ToString(), targetPickupPool);
		poolManager.PopulatePool<Pickup>(Defines.PoolKey.HomingAmmoPickup.ToString(), targetPickupPool);
		poolManager.PopulatePool<Pickup>(Defines.PoolKey.SpeedBoostPickup.ToString(), targetPickupPool);
		
		
		var asteroidsToPlace = poolManager.RetrieveObjects<Asteroid>(level.AsteroidCount);

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

			asteroid.gameObject.SetActive(true);
		}

		var ship = poolManager.RetrieveObject<ShipController>();
		ship.ResetShip();
		
		return Tuple.Create(ship, asteroidsToPlace);
	}
}
