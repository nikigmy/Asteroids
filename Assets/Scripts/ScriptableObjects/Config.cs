using UnityEngine;
using Utils;

namespace ScriptableObjects
{
    /// <summary>
    /// Holds a configuration of the game
    /// </summary>
    [CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/Config", order = 1)]
    public class Config : ScriptableObject
    {
        [Header("LevelSettings", order = 1)] 
        public float levelBoundaryThickness = 10;
        
        public float cameraZOffset = -10;
        
        [Header("Player", order = 2)] 
        public int playerStartHealth = 3;
        
        public ShipData playerData;

        public ParticleSystem.MinMaxGradient engineColor;
        
        public ParticleSystem.MinMaxGradient boostedEngineColor;

        public Declarations.EffectDuration playerStartEffect;
        
        [Header("Enemy", order = 3)] 
        public ShipData flyingSaucerData;
        
        public float saucerShotRandomisation = 15;

        public int flyingSaucerReward = 2000;

        [Header("Asteroids", order = 4)] 
        public int asteroidStartLevel = 3;

        public Declarations.AsteroidLevel[] asteroidLevels;

        [Header("Asteroid Shape Generation", order = 5)]
        public int minAsteroidPoints = 10;
        
        public int maxAsteroidPoints = 15;
        
        public float minAsteroidRadius = 1.3f;
        
        public float maxAsteroidRadius = 2.3f;
        
        [Header("Pickups", order = 6)] 
        public Declarations.EffectDuration[] pickupDurations;

        public int pickupDropPercentage = 5;
        
        public int speedBoostPercentage = 30;

        [Header("Projectiles", order = 7)] 
        public float projectileLife = 2.5f;

        public float projectileSpeed = 300.0f;

        public int homingRotationSpeed = 17;

        public Color playerProjectileColor;
        
        public Color enemyProjectileColor;
        
        [Header("PoolSettings", order = 8)] 
        public int minAsteroidPool = 15;

        public int shipPool = 1;

        public int audioSourcePool = 10;

        public int bulletPool = 20;

        public int optimalAsteroidPoolPercentage = 70;

        [Header("Controls", order = 9)] 
        public Declarations.ControlScheme ControlScheme; //This can be set in the initialisation depending on the platform. Left here for testing purposes
    }
}