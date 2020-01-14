using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : GenericObjectGenerator<Asteroid>
{
	public AsteroidGenerator(string prefabPath) : base(prefabPath)
	{
	}

	public override Asteroid GenerateObject(Transform parent = null)
	{
		Asteroid asteroid = base.GenerateObject(parent);
		
		var lineRenderer = asteroid.GetComponent<LineRenderer>();
		var polygonCollider = asteroid.GetComponent<PolygonCollider2D>();
		var collisionReporter = asteroid.GetComponent<CollisionReporter>();
		GameManager.instance.LevelManager.CollisionManager.RegisterCollisionReporter(collisionReporter);
		
		var pointsCount = Random.Range(GameManager.instance.Config.minPoints, GameManager.instance.Config.maxPoints);
		var step = 360 / (pointsCount + 1);
		
		Vector2[] colliderPositions = new Vector2[pointsCount];
		Vector3[] positions = new Vector3[pointsCount + 1];
		var angle = 0f;

		lineRenderer.positionCount = pointsCount + 1;

		for (int i = 0; i < pointsCount; i++)
		{
			var currAngle = (angle + Random.Range(GameManager.instance.Config.minAngleRandomisation, GameManager.instance.Config.maxAngleRandomisation)) * Mathf.Deg2Rad;
			var r = Random.Range(GameManager.instance.Config.minRadius, GameManager.instance.Config.maxRadius);

			var x = r * Mathf.Cos(currAngle);
			var y = r * Mathf.Sin(currAngle);
			colliderPositions[i] = new Vector2(x, y);
			positions[i] = new Vector3(x, y, 0);
			angle += step;
		}

		positions[pointsCount] = positions[0];

		polygonCollider.points = colliderPositions;
		lineRenderer.SetPositions(positions);

		return asteroid;
	}

	public override Asteroid[] GenerateObjects(int count, Transform parent = null)
	{
		var result = new Asteroid[count];
		for (int i = 0; i < count; i++)
		{
			result[i] = GenerateObject(parent);
		}

		return result;
	}
}