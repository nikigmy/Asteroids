using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
	private PoolManager poolManager;
	[SerializeField] private LevelFactory levelFactory;
	[SerializeField] private BoundarySetter boundarySetter;
	[SerializeField] private CameraSetter cameraSetter;
	public Vector2 FieldPosition { get; private set; }
	public Vector2 FieldSize { get; private set; }
	
	
	public PoolManager PoolManager
	{
		get { return poolManager; }
	}

	public void Init(LevelData level)
	{
		boundarySetter = FindObjectOfType<BoundarySetter>();
		cameraSetter = FindObjectOfType<CameraSetter>();
		levelFactory = FindObjectOfType<LevelFactory>();
			
		poolManager = levelFactory.CreatePoolManager();

		var cam = Camera.main;
		cam.transform.position = Vector3.back * 100;
		FieldPosition = Vector2.zero;

		var aspectRatio = (float) Screen.width / Screen.height;
		var fieldHeight = cam.orthographicSize * 2;
		FieldSize = new Vector2(fieldHeight * aspectRatio, fieldHeight);

		boundarySetter.SetBoundaries(Vector2.zero, FieldSize);
		cameraSetter.PositionCameras(Vector3.back * 100, FieldSize);

		levelFactory.CreateLevel(level, poolManager, OnAsteroidDestroyed);
	}

	private UnityEvent Event;
	public void OnAsteroidDestroyed(GameObject asteroid, int level, Vector3 hitDir)
	{
		Debug.Log(asteroid.GetInstanceID());
		GameManager.instance.LevelManager.PoolManager.ReturnObject(Defines.PoolKeys.asteroid, asteroid);

		if (level > 1)
		{
			GameManager.instance.StartCoroutine(SplitAsteroid(level, asteroid.transform.position, hitDir));
		}
	}
	//
	IEnumerator SplitAsteroid( int level, Vector3 spawnPos, Vector3 hitDir)
	{
		foreach (var rend in FindObjectsOfType<Asteroid>())
		{
			var lineRenderer = rend.GetComponent<LineRenderer>();	
			lineRenderer.startColor = Color.white;
			lineRenderer.endColor = Color.white;
		}
		transform.forward = Vector3.back;
		yield return new WaitForSeconds(0.03f);

		var nextLevel = level - 1;
		var asteroids = GameManager.instance.LevelManager.PoolManager.RetrieveObjects(Defines.PoolKeys.asteroid, 2);
		var rot = Quaternion.Euler(new Vector3(0, 0, (Mathf.Atan(hitDir.y / hitDir.x) * Mathf.Rad2Deg) + 90));
		
		foreach (var asteroidObject in asteroids)
		{
			asteroidObject.transform.rotation = rot;
			
//			var dir = new Vector3(Utils.GenerateRandomValue(GameManager.instance.Config.asteroidMinSpeed, GameManager.instance.Config.asteroidMaxSpeed), 
//				Utils.GenerateRandomValue(GameManager.instance.Config.asteroidMinSpeed, GameManager.instance.Config.asteroidMaxSpeed));
			var asteroid = asteroidObject.GetComponent<Asteroid>();
			
			asteroid.SetData(nextLevel, UnityEngine.Random.Range(GameManager.instance.Config.asteroidMinSpeed, GameManager.instance.Config.asteroidMaxSpeed));
			asteroid.OnAsteroidDestroyed += OnAsteroidDestroyed;
			asteroidObject.transform.position = spawnPos;
			asteroidObject.SetActive(true);
			asteroidObject.GetComponent<LineRenderer>().endColor =
				asteroidObject.GetComponent<LineRenderer>().startColor = Color.red;
		}
		
		Debug.Log(asteroids[0].name + "      " + asteroids[1].name);
		asteroids[0].transform.Rotate(Vector3.forward, 180);
	}
}
