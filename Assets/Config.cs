using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/Config", order = 1)]
public class Config : ScriptableObject
{
[Header("Game settings")]
public int startHealth;

[Header("Asteroids", order = 1)] 
	public int asteroidStartLevel = 3;
	public float asteroidMinSpeed = 1;
	public float asteroidMaxSpeed = 3;
	public float[] scaleLevels = new float[3]
	{
		0.3f,
		0.6f,
		1,
	};

	[Header("Asteroid Shape Generation", order = 2)]
	public float minRadius = 0.5f;
	public float maxRadius = 1;
	public int minPoints = 6;
	public int maxPoints = 8;
	public float minAngleRandomisation = 5; 
	public float maxAngleRandomisation = 10;
	
	[Header("PoolSettings")]
	public int shipPool = 1;
	public int flyingSaucerPool = 1;
	public int minAsteroidPool = 15;
	public int bulletPool = 20;
	public int optimalAsteroidPoolPercentage = 70;

	[Header("Controls")] public Defines.ControlScheme ControlScheme;
}
