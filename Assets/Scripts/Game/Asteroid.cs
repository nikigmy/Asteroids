using System.Linq;
using Managers;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Behaviour of asteroids
    /// </summary>
    [RequireComponent(typeof(LineRenderer), typeof(PolygonCollider2D), typeof(CollisionReporter))]
    public class Asteroid : MonoBehaviour
    {
        public int Level { get; private set; }
    
        private void Update()
        {
            transform.position += Time.deltaTime * mSpeed * transform.right;
        }
    
        /// <summary>
        /// Initialise the asteroid
        /// </summary>
        /// <param name="level">Level of the asteroid</param>
        /// <param name="speed">Speed of the asteroid</param>
        /// <param name="scale">Scale of the asteroid</param>
        public void Init(int level, float speed, float scale)
        {
            mSpeed = speed;
            Level = level;

            transform.localScale = Vector3.one * scale;
        }

        private float mSpeed;
    }
}