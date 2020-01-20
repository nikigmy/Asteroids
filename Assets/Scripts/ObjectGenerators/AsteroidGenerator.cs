using Game;
using Managers;
using UnityEngine;

namespace ObjectGenerators
{
    /// <summary>
    /// Generator for asteroids
    /// </summary>
    public class AsteroidGenerator : GenericObjectGenerator<Asteroid>
    {
        
        /// <param name="prefabPath">Path of the object to generate</param>
        public AsteroidGenerator(string prefabPath) : base(prefabPath)
        {
        }

        public override Asteroid GenerateObject(Transform parent = null)
        {
            var asteroid = base.GenerateObject(parent);

            var lineRenderer = asteroid.GetComponent<LineRenderer>();
            var polygonCollider = asteroid.GetComponent<PolygonCollider2D>();
            var collisionReporter = asteroid.GetComponent<CollisionReporter>();
            GameManager.Instance.LevelManager.CollisionManager.RegisterCollisionReporter(collisionReporter);

            var pointsCount = Random.Range(GameManager.Instance.Config.minAsteroidPoints,
                GameManager.Instance.Config.maxAsteroidPoints);
            
            var colliderPositions = new Vector2[pointsCount];
            var positions = new Vector3[pointsCount + 1];

            var step = 360 / (pointsCount + 1);
            var angle = 0f;

            lineRenderer.positionCount = pointsCount + 1;

            for (var i = 0; i < pointsCount; i++)
            {
                var currAngle = angle * Mathf.Deg2Rad;
                var r = Random.Range(GameManager.Instance.Config.minAsteroidRadius,
                    GameManager.Instance.Config.maxAsteroidRadius);

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
            for (var i = 0; i < count; i++) result[i] = GenerateObject(parent);

            return result;
        }
    }
}