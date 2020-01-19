using Managers;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Behaviour of asteroids
    /// </summary>
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
        public void Init(int level, float speed)
        {
            mSpeed = speed;
            Level = level;

            transform.localScale = Vector3.one * GameManager.instance.Config.asteroidScaleLevels[level - 1];
        }

        private float mSpeed;
    }
}