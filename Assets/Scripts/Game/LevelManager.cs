using System;
using System.Collections;
using System.Linq;
using Managers;
using ScriptableObjects;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Game
{
    /// <summary>
    /// This manager handles the gameplay
    /// </summary>
    [RequireComponent(typeof(CollisionManager))]
    public class LevelManager : MonoBehaviour
    {
        private const float cPlayerSpawnRadius = 2f;
        public event EventHandler<ValueArgs<int>> OnScoreChanged;
        public event EventHandler<ValueArgs<int>> OnHealthChanged;
        public event EventHandler<ValueArgs<(int health, int score)>> OnLevelComplete;
        public event EventHandler<EventArgs> OnLevelStarted;
        public event EventHandler<EventArgs> OnGameOver;
        
        public Vector2 FieldPosition { get; private set; }
        public Vector2 FieldSize { get; private set; }

        public CollisionManager CollisionManager { get; private set; }

        public int Score { get; private set; }

        public int Health { get; private set; }

        public ShipController Player { get; private set; }

        private void Awake()
        {
            CollisionManager = GetComponent<CollisionManager>();
            
            Debug.Assert(wrapAroundCameras != null);
            Debug.Assert(boundaryColliders != null);
            Debug.Assert(wrapAroundCameras.Length == 8);
            Debug.Assert(boundaryColliders.Length == 4);
        }

        #region Public

        /// <summary>
        /// Initialises the level manager
        /// </summary>
        /// <param name="poolManager">Pool manager</param>
        /// <param name="audioManager">Audio manager</param>
        /// <param name="configuration">Configuration</param>
        public void Init(PoolManager poolManager, AudioManager audioManager, Config configuration)
        {
            mPoolManager = poolManager;
            mAudioManager = audioManager;
            FieldPosition = Vector2.zero;
            mConfiguration = configuration;

            var aspectRatio = (float) Screen.width / Screen.height;
            var fieldHeight = Camera.main.orthographicSize * 2;
            FieldSize = new Vector2(fieldHeight * aspectRatio, fieldHeight);

            PositionBoundaries();
            PositionCameras();

            CollisionManager.SubscribeOnEnter(Constants.Tags.cAsteroid, Constants.Tags.cProjectile, AsteroidDestroyed);
            CollisionManager.SubscribeOnEnter(Constants.Tags.cAsteroid, Constants.Tags.cPlayer, AsteroidPlayerHit);
            CollisionManager.SubscribeOnEnter(Constants.Tags.cProjectile, Constants.Tags.cPlayer, BulletPlayerHit);
            CollisionManager.SubscribeOnEnter(Constants.Tags.cProjectile, Constants.Tags.cEnemy, BulletEnemyHit);
            CollisionManager.SubscribeOnEnter(Constants.Tags.cEnemy, Constants.Tags.cPlayer, EnemyPlayerHit);
            CollisionManager.SubscribeOnEnter(Constants.Tags.cPlayer, Constants.Tags.cPickup, PlayerPickupHit);

            CollisionManager.SubscribeOnStay(Constants.Tags.cEnemy, Constants.Tags.cPlayer, EnemyPlayerHit);
            CollisionManager.SubscribeOnStay(Constants.Tags.cProjectile, Constants.Tags.cPlayer, BulletPlayerHit);
            CollisionManager.SubscribeOnStay(Constants.Tags.cAsteroid, Constants.Tags.cPlayer, AsteroidPlayerHit);
        }

        /// <summary>
        /// Loads a level with default score and start health
        /// </summary>
        /// <param name="level">Level to load</param>
        public void LoadLevel(LevelData level)
        {
            mCurrentLevel = level;

            Score = 0;
            Health = mConfiguration.playerStartHealth;

            CreateLevel(level);
        }

        /// <summary>
        /// Loads a level
        /// </summary>
        /// <param name="level">Level to load</param>
        /// <param name="startHealth">Start health</param>
        /// <param name="startScore">Start score</param>
        public void LoadLevel(LevelData level, int startHealth, int startScore)
        {
            mCurrentLevel = level;

            Score = startScore;
            Health = startHealth;

            CreateLevel(level);
        }

        /// <summary>
        /// Starts the currently loaded level
        /// </summary>
        /// <param name="playerData">Player ship data</param>
        /// <param name="saucerData">Saucer ship data</param>
        public void StartLevel(ShipData playerData, ShipData saucerData)
        {
            var ship = mPoolManager.RetrieveObject<PlayerController>();

            ship.Init(playerData);
            ship.ResetShip();

            Player = ship;

            mStartLevelTime = Time.time;

            mFlyingSaucerData = saucerData;
            if (mCurrentLevel.FlyingSaucerTimings != null && mCurrentLevel.FlyingSaucerTimings.Length > 0)
                StartCoroutine("SpawnFlyingSaucerRoutine");

            mGameMusicSource = mAudioManager.Play(Constants.AudioKeys.cGameMusic,
                AudioGroup.Music, true,
                Vector3.zero);
            this.Raise(OnLevelStarted, EventArgs.Empty);
        }
        
        /// <summary>
        /// Clears the loaded level
        /// </summary>
        public void Clear()
        {
            mPoolManager.ReturnObject(mGameMusicSource);
            StopCoroutine("SpawnFlyingSaucerRoutine");
            mPoolManager.ReturnAllObjects(new[] {typeof(AudioSource).FullName, Constants.PoolKeys.cHeart});
        }

        #endregion

        #region CollisionHandlers
        
        private void AsteroidDestroyed(Tuple<Collider2D, Collider2D> collision)
        {
            Asteroid asteroid;
            BulletController bullet;
            if (collision.Item1.gameObject.CompareTag(Constants.Tags.cAsteroid))
            {
                asteroid = collision.Item1.GetComponent<Asteroid>();
                bullet = collision.Item2.GetComponent<BulletController>();
            }
            else
            {
                asteroid = collision.Item2.GetComponent<Asteroid>();
                bullet = collision.Item1.GetComponent<BulletController>();
            }

            OnAsteroidDestroyed(asteroid, bullet);
        }
        
        private void AsteroidPlayerHit(Tuple<Collider2D, Collider2D> obj)
        {
            TakeDamage();
        }

        private void BulletPlayerHit(Tuple<Collider2D, Collider2D> collision)
        {
            BulletController bullet;
            if (collision.Item1.gameObject.CompareTag(Constants.Tags.cProjectile))
                bullet = collision.Item1.GetComponent<BulletController>();
            else
                bullet = collision.Item2.GetComponent<BulletController>();

            if (bullet.Mode == ProjectileMode.Enemy)
            {
                mPoolManager.ReturnObject(bullet);
                TakeDamage();
            }
        }

        private void BulletEnemyHit(Tuple<Collider2D, Collider2D> collision)
        {
            FlyingSaucer enemy;
            BulletController bullet;
            if (collision.Item1.gameObject.CompareTag(Constants.Tags.cEnemy))
            {
                enemy = collision.Item1.GetComponent<FlyingSaucer>();
                bullet = collision.Item2.GetComponent<BulletController>();
            }
            else
            {
                enemy = collision.Item2.GetComponent<FlyingSaucer>();
                bullet = collision.Item1.GetComponent<BulletController>();
            }

            if (bullet.Mode == ProjectileMode.Friendly)
            {
                AddScore(mConfiguration.flyingSaucerReward);

                mPoolManager.ReturnObject(enemy);
                mPoolManager.ReturnObject(bullet);

                mAudioManager.Play(Constants.AudioKeys.cExplosion, AudioGroup.Sfx,
                    false,
                    enemy.transform.position);
            }
        }

        private void EnemyPlayerHit(Tuple<Collider2D, Collider2D> obj)
        {
            TakeDamage();
        }
        
        private void PlayerPickupHit(Tuple<Collider2D, Collider2D> collision)
        {
            ShipController player;
            Pickup pickUp;
            if (collision.Item1.gameObject.CompareTag(Constants.Tags.cPlayer))
            {
                player = collision.Item1.GetComponent<ShipController>();
                pickUp = collision.Item2.GetComponent<Pickup>();
            }
            else
            {
                player = collision.Item2.GetComponent<ShipController>();
                pickUp = collision.Item1.GetComponent<Pickup>();
            }

            OnPickup(player, pickUp);
        }

        #endregion
        
        #region Private

        private void OnPickup(ShipController player, Pickup pickup)
        {
            mAudioManager.Play(Constants.AudioKeys.cPickup, AudioGroup.Sfx, false,
                pickup.transform.position);

            var key = pickup.Effect == Declarations.EffectType.Shield ? Constants.PoolKeys.cShieldPickup :
                pickup.Effect == Declarations.EffectType.Speed ? Constants.PoolKeys.cSpeedBoostPickup :
                Constants.PoolKeys.cHomingAmmoPickup;
            player.GetComponent<EffectHandler>().AddEffect(pickup.Effect,
                mConfiguration.pickupDurations.First(x => x.effectType == pickup.Effect)
                    .duration);

            mPoolManager.ReturnObject(key, pickup);
        }

        private void CreateLevel(LevelData level)
        {
            var targetAsteroidPool =
                level.AsteroidCount * 4 * (mConfiguration.optimalAsteroidPoolPercentage / 100);
            var targetPickupPool = (targetAsteroidPool * mConfiguration.pickupDropPercentage) / 100;

            mPoolManager.PopulatePoolToTarget<Asteroid>(targetAsteroidPool);
            mPoolManager.PopulatePool<Pickup>(Constants.PoolKeys.cShieldPickup, targetPickupPool);
            mPoolManager.PopulatePool<Pickup>(Constants.PoolKeys.cHomingAmmoPickup, targetPickupPool);
            mPoolManager.PopulatePool<Pickup>(Constants.PoolKeys.cSpeedBoostPickup, targetPickupPool);

            if (level.FlyingSaucerTimings != null && level.FlyingSaucerTimings.Length > 0)
                mPoolManager.PopulatePoolToTarget<FlyingSaucer>(level.FlyingSaucerTimings.Length);

            var asteroidsToPlace = mPoolManager.RetrieveObjects<Asteroid>(level.AsteroidCount);

            var asteroidLevelData = mConfiguration.asteroidLevels.First(x =>
                x.asteroidLevel == mConfiguration.asteroidStartLevel);
            for (var i = 0; i < asteroidsToPlace.Length; i++)
            {
                var asteroid = asteroidsToPlace[i];

                var posX = Utils.Utils.GenerateRandomValue(cPlayerSpawnRadius, FieldSize.x / 2f);
                var posY = Utils.Utils.GenerateRandomValue(cPlayerSpawnRadius, FieldSize.y / 2f);
                asteroid.transform.position = new Vector3(posX, posY, 0);

                var asteroidRotation = new Vector3(0,
                    0, Utils.Utils.GenerateRandomValue(0, 180));
                asteroid.transform.rotation = Quaternion.Euler(asteroidRotation);

                var speed = Random.Range(asteroidLevelData.maxSpeed, asteroidLevelData.maxSpeed);
                asteroid.GetComponent<Asteroid>().Init(mConfiguration.asteroidStartLevel, speed, asteroidLevelData.scale);

                asteroid.gameObject.SetActive(true);
            }
        }

        private IEnumerator SpawnFlyingSaucerRoutine()
        {
            for (var i = 0; i < mCurrentLevel.FlyingSaucerTimings.Length; i++)
            {
                var nextTiming = mStartLevelTime + mCurrentLevel.FlyingSaucerTimings[i];
                yield return new WaitForSeconds(nextTiming - Time.time);

                SpawnFlyingSaucer();
            }
        }

        private void SpawnFlyingSaucer()
        {
            var enemy = mPoolManager.RetrieveObject<FlyingSaucer>();

            enemy.Init(mFlyingSaucerData);

            var posX = Utils.Utils.GenerateRandomValue(0, FieldSize.x / 2f);
            var posY = Utils.Utils.GenerateRandomValue(0, FieldSize.y / 2f);
            enemy.transform.position = new Vector3(posX, posY, 0);

            var movementVectorX = Utils.Utils.GenerateRandomValue(0, 1);
            var movementVectorY = Utils.Utils.GenerateRandomValue(0, 1 - Mathf.Abs(movementVectorX));
            enemy.SetMovementVector(new Vector3(movementVectorX, movementVectorY, 0));
            enemy.transform.rotation = Quaternion.Euler(Vector3.zero);

            enemy.gameObject.SetActive(true);
        }

        private void OnAsteroidDestroyed(Asteroid asteroid, Projectile projectile)
        {
            mAudioManager.Play(Constants.AudioKeys.cExplosion, AudioGroup.Sfx, false,
                asteroid.transform.position);

            if (projectile.Mode == ProjectileMode.Friendly) AddScore(mConfiguration.asteroidLevels.First(x => x.asteroidLevel == asteroid.Level).reward);

            var hitDir = projectile.transform.position - asteroid.transform.position;

            if (asteroid.Level > 1) SplitAsteroid(asteroid.Level, asteroid.transform.position, hitDir);

            if (Random.Range(0, 100 / mConfiguration.pickupDropPercentage) == 0)
            {
                var key = Constants.PoolKeys.cShieldPickup;
                var pickupIndex = Random.Range(0, 3);
                switch (pickupIndex)
                {
                    case 0:
                        key = Constants.PoolKeys.cShieldPickup;
                        break;
                    case 1:
                        key = Constants.PoolKeys.cHomingAmmoPickup;
                        break;
                    case 2:
                        key = Constants.PoolKeys.cSpeedBoostPickup;
                        break;
                }

                var pickup = mPoolManager.RetrieveObject<Pickup>(key);
                pickup.transform.position = asteroid.transform.position;
            }

            mPoolManager.ReturnObject(projectile as BulletController);
            mPoolManager.ReturnObject(asteroid);

            if (asteroid.Level == 1 && mPoolManager.GetActiveObjects<Asteroid>().Count == 0)
            {
                this.Raise(OnLevelComplete, new ValueArgs<(int health, int score)>((Health, Score)));
            }
        }

        private void AddScore(int addedScore)
        {
            Score += addedScore;
            this.Raise(OnScoreChanged, new ValueArgs<int>(Score));
        }

        private void TakeDamage()
        {
            if (!Player.GetComponent<EffectHandler>().IsEffectActive(Declarations.EffectType.Shield))
            {
                Health--;
                this.Raise(OnHealthChanged, new ValueArgs<int>(Health));
                if (Health > 0)
                {
                    mAudioManager.Play(Constants.AudioKeys.cExplosion, AudioGroup.Sfx,
                        false, Player.transform.position);
                    Player.ResetShip();
                }
                else
                {
                    Player.GetComponent<EffectHandler>().ClearEffects();
                    this.Raise(OnGameOver, new ValueArgs<int>(Score));
                }
            }
        }

        private void SplitAsteroid(int level, Vector3 spawnPos, Vector3 hitDir)
        {
            var nextLevel = level - 1;
            var asteroids = mPoolManager.RetrieveObjects<Asteroid>(2);
            var angle = Mathf.Atan2(hitDir.y, hitDir.x) * Mathf.Rad2Deg;
            var asteroidLevelData = mConfiguration.asteroidLevels.First(x => x.asteroidLevel == nextLevel);

            foreach (var asteroidObject in asteroids)
            {
                var asteroid = asteroidObject.GetComponent<Asteroid>();

                asteroid.Init(nextLevel, Random.Range(asteroidLevelData.minSpeed, asteroidLevelData.maxSpeed), asteroidLevelData.scale);
                asteroidObject.transform.position = spawnPos;
                asteroidObject.gameObject.SetActive(true);
            }

            asteroids[0].transform.rotation = Quaternion.Euler(Vector3.forward * (angle + 90));
            asteroids[1].transform.rotation = Quaternion.Euler(Vector3.forward * (angle - 90));
        }


        private void PositionCameras()
        {
            var fieldPos = Vector2.zero;
            var cameSize = Camera.main.orthographicSize;
            //left
            wrapAroundCameras[0].transform.position = new Vector3(fieldPos.x - FieldSize.x, fieldPos.y, mConfiguration.cameraZOffset);
            //leftUp
            wrapAroundCameras[1].transform.position =
                new Vector3(fieldPos.x - FieldSize.x, fieldPos.y + FieldSize.y, mConfiguration.cameraZOffset);
            //up
            wrapAroundCameras[2].transform.position = new Vector3(fieldPos.x, fieldPos.y + FieldSize.y, mConfiguration.cameraZOffset);
            //rightUp
            wrapAroundCameras[3].transform.position =
                new Vector3(fieldPos.x + FieldSize.x, fieldPos.y + FieldSize.y, mConfiguration.cameraZOffset);
            //right
            wrapAroundCameras[4].transform.position = new Vector3(fieldPos.x + FieldSize.x, fieldPos.y, mConfiguration.cameraZOffset);
            //rightBot
            wrapAroundCameras[5].transform.position =
                new Vector3(fieldPos.x + FieldSize.x, fieldPos.y - FieldSize.y, mConfiguration.cameraZOffset);
            //bot
            wrapAroundCameras[6].transform.position = new Vector3(fieldPos.x, fieldPos.y - FieldSize.y, mConfiguration.cameraZOffset);
            //leftBot
            wrapAroundCameras[7].transform.position =
                new Vector3(fieldPos.x - FieldSize.x, fieldPos.y - FieldSize.y, mConfiguration.cameraZOffset);

            for (var i = 0; i < wrapAroundCameras.Length; i++) wrapAroundCameras[i].orthographicSize = cameSize;
        }

        private void PositionBoundaries()
        {
            var fieldPos = Vector2.zero;
            var boundaryThickness = mConfiguration.levelBoundaryThickness;
            var leftBoundaryPos = new Vector3(fieldPos.x - FieldSize.x / 2 - boundaryThickness / 2, fieldPos.y);
            var rightBoundaryPos = new Vector3(fieldPos.x + FieldSize.x / 2 + boundaryThickness / 2, fieldPos.y);
            var topBoundaryPos = new Vector3(fieldPos.x, fieldPos.y + FieldSize.y / 2 + boundaryThickness / 2);
            var botBoundaryPos = new Vector3(fieldPos.x, fieldPos.y - FieldSize.y / 2 - boundaryThickness / 2);

            var verticalBoundarySize = new Vector2(boundaryThickness, FieldSize.y + boundaryThickness * 2);
            var horizontalBoundarySize = new Vector2(FieldSize.x, boundaryThickness);


            boundaryColliders[0].transform.position = leftBoundaryPos;
            boundaryColliders[0].size = verticalBoundarySize;

            boundaryColliders[1].transform.position = rightBoundaryPos;
            boundaryColliders[1].size = verticalBoundarySize;

            boundaryColliders[2].transform.position = topBoundaryPos;
            boundaryColliders[2].size = horizontalBoundarySize;

            boundaryColliders[3].transform.position = botBoundaryPos;
            boundaryColliders[3].size = horizontalBoundarySize;
        }
        #endregion

        private LevelData mCurrentLevel;

        private ShipData mFlyingSaucerData;

        private AudioSource mGameMusicSource;

        private PoolManager mPoolManager;

        private AudioManager mAudioManager;

        private float mStartLevelTime;
        
        [SerializeField] private Camera[] wrapAroundCameras;
        
        [SerializeField] private BoxCollider2D[] boundaryColliders;

        private Config mConfiguration;
    }
}