using System;
using System.Collections;
using System.Linq;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
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

        private void Start()
        {
            CollisionManager = GetComponent<CollisionManager>();
        }

        #region Public

        /// <summary>
        /// Initialises the level manager
        /// </summary>
        /// <param name="poolManager">Pool manager</param>
        public void Init(PoolManager poolManager)
        {
            mPoolManager = poolManager;

            FieldPosition = Vector2.zero;

            var aspectRatio = (float) Screen.width / Screen.height;
            var fieldHeight = Camera.main.orthographicSize * 2;
            FieldSize = new Vector2(fieldHeight * aspectRatio, fieldHeight);

            PositionBoundaries();
            PositionCameras();

            CollisionManager.SubscribeOnEnter(Constants.Tags.asteroid, Constants.Tags.projectile, AsteroidDestroyed);
            CollisionManager.SubscribeOnEnter(Constants.Tags.asteroid, Constants.Tags.player, AsteroidPlayerHit);
            CollisionManager.SubscribeOnEnter(Constants.Tags.projectile, Constants.Tags.player, BulletPlayerHit);
            CollisionManager.SubscribeOnEnter(Constants.Tags.projectile, Constants.Tags.enemy, BulletEnemyHit);
            CollisionManager.SubscribeOnEnter(Constants.Tags.enemy, Constants.Tags.player, EnemyPlayerHit);
            CollisionManager.SubscribeOnEnter(Constants.Tags.player, Constants.Tags.pickup, PlayerPickupHit);

            CollisionManager.SubscribeOnStay(Constants.Tags.enemy, Constants.Tags.player, EnemyPlayerHit);
            CollisionManager.SubscribeOnStay(Constants.Tags.projectile, Constants.Tags.player, BulletPlayerHit);
            CollisionManager.SubscribeOnStay(Constants.Tags.asteroid, Constants.Tags.player, AsteroidPlayerHit);
        }

        /// <summary>
        /// Loads a level with default score and start health
        /// </summary>
        /// <param name="level">Level to load</param>
        public void LoadLevel(LevelData level)
        {
            mCurrentLevel = level;

            Score = 0;
            Health = GameManager.instance.Config.playerStartHealth;

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

            mGameMusicSource = GameManager.instance.AudioManager.Play(Constants.AudioKeys.gameMusic,
                AudioGroup.Music, true,
                Vector3.zero);
            this.Raise(OnLevelStarted, EventArgs.Empty);
        }
        
        /// <summary>
        /// Clears the loaded level
        /// </summary>
        public void Clear()
        {
            StopCoroutine("SpawnFlyingSaucerRoutine");
            mPoolManager.ReturnAllObjects(new[] {typeof(AudioSource).FullName});
            mPoolManager.ReturnObject(mGameMusicSource);
        }

        #endregion

        #region CollisionHandlers
        
        private void AsteroidDestroyed(Tuple<Collider2D, Collider2D> collision)
        {
            Asteroid asteroid;
            BulletController bullet;
            if (collision.Item1.gameObject.CompareTag(Constants.Tags.asteroid))
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
            if (collision.Item1.gameObject.CompareTag(Constants.Tags.projectile))
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
            if (collision.Item1.gameObject.CompareTag(Constants.Tags.enemy))
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
                AddScore(2000);

                mPoolManager.ReturnObject(enemy);
                mPoolManager.ReturnObject(bullet);

                GameManager.instance.AudioManager.Play(Constants.AudioKeys.explosion, AudioGroup.Sfx,
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
            if (collision.Item1.gameObject.CompareTag(Constants.Tags.player))
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
            GameManager.instance.AudioManager.Play(Constants.AudioKeys.pickup, AudioGroup.Sfx, false,
                pickup.transform.position);

            var key = pickup.Effect == Declarations.EffectType.Shield ? Constants.PoolKeys.shieldPickup :
                pickup.Effect == Declarations.EffectType.Speed ? Constants.PoolKeys.speedBoostPickup :
                Constants.PoolKeys.homingAmmoPickup;
            player.GetComponent<EffectHandler>().AddEffect(pickup.Effect,
                GameManager.instance.Config.pickupDurations.FirstOrDefault(x => x.effectType == pickup.Effect)
                    .duration);

            GameManager.instance.LevelManager.mPoolManager.ReturnObject(key, pickup);
        }

        private void CreateLevel(LevelData level)
        {
            var targetAsteroidPool =
                level.AsteroidCount * 4 * (GameManager.instance.Config.optimalAsteroidPoolPercentage / 100);
            var targetPickupPool = targetAsteroidPool / 20;

            mPoolManager.PopulatePoolToTarget<Asteroid>(targetAsteroidPool);
            mPoolManager.PopulatePool<Pickup>(Constants.PoolKeys.shieldPickup, targetPickupPool);
            mPoolManager.PopulatePool<Pickup>(Constants.PoolKeys.homingAmmoPickup, targetPickupPool);
            mPoolManager.PopulatePool<Pickup>(Constants.PoolKeys.speedBoostPickup, targetPickupPool);

            if (level.FlyingSaucerTimings != null && level.FlyingSaucerTimings.Length > 0)
                mPoolManager.PopulatePoolToTarget<FlyingSaucer>(level.FlyingSaucerTimings.Length);

            var asteroidsToPlace = mPoolManager.RetrieveObjects<Asteroid>(level.AsteroidCount);

            var speeds = GameManager.instance.Config.asteroidSpeeds.First(x =>
                x.asteroidLevel == GameManager.instance.Config.asteroidStartLevel);
            for (var i = 0; i < asteroidsToPlace.Length; i++)
            {
                var asteroid = asteroidsToPlace[i];

                var posX = Utils.Utils.GenerateRandomValue(1.5f, GameManager.instance.LevelManager.FieldSize.x / 2f);
                var posY = Utils.Utils.GenerateRandomValue(1.5f, GameManager.instance.LevelManager.FieldSize.y / 2f);
                asteroid.transform.position = new Vector3(posX, posY, 0);

                var asteroidRotation = new Vector3(0,
                    0, Utils.Utils.GenerateRandomValue(0, 180));
                asteroid.transform.rotation = Quaternion.Euler(asteroidRotation);

                var speed = Random.Range(speeds.maxSpeed, speeds.maxSpeed);
                asteroid.GetComponent<Asteroid>().Init(GameManager.instance.Config.asteroidStartLevel, speed);

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

            var posX = Utils.Utils.GenerateRandomValue(0, GameManager.instance.LevelManager.FieldSize.x / 2f);
            var posY = Utils.Utils.GenerateRandomValue(0, GameManager.instance.LevelManager.FieldSize.y / 2f);
            enemy.transform.position = new Vector3(posX, posY, 0);

            var movementVectorX = Utils.Utils.GenerateRandomValue(0, 1);
            var movementVectorY = Utils.Utils.GenerateRandomValue(0, 1 - Mathf.Abs(movementVectorX));
            enemy.SetMovementVector(new Vector3(movementVectorX, movementVectorY, 0));
            enemy.transform.rotation = Quaternion.Euler(Vector3.zero);

            enemy.gameObject.SetActive(true);
        }

        private void OnAsteroidDestroyed(Asteroid asteroid, Projectile projectile)
        {
            GameManager.instance.AudioManager.Play(Constants.AudioKeys.explosion, AudioGroup.Sfx, false,
                asteroid.transform.position);

            int[] asteroidRewards = {1000, 500, 100};

            if (projectile.Mode == ProjectileMode.Friendly) AddScore(asteroidRewards[asteroid.Level - 1]);

            var hitDir = projectile.transform.position - asteroid.transform.position;

            if (asteroid.Level > 1) SplitAsteroid(asteroid.Level, asteroid.transform.position, hitDir);

            if (Random.Range(0, 20) == 0)
            {
                var key = Constants.PoolKeys.shieldPickup;
                var pickupIndex = Random.Range(0, 3);
                switch (pickupIndex)
                {
                    case 0:
                        key = Constants.PoolKeys.shieldPickup;
                        break;
                    case 1:
                        key = Constants.PoolKeys.homingAmmoPickup;
                        break;
                    case 2:
                        key = Constants.PoolKeys.speedBoostPickup;
                        break;
                }

                var pickup = GameManager.instance.LevelManager.mPoolManager.RetrieveObject<Pickup>(key);
                pickup.transform.position = asteroid.transform.position;
            }

            mPoolManager.ReturnObject(projectile as BulletController);
            mPoolManager.ReturnObject(asteroid);

            if (asteroid.Level == 1 && !mPoolManager.GetActiveObjects<Asteroid>().Any())
            {
                mPoolManager.ReturnObject(mGameMusicSource);
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
            if (!Player.GetComponent<EffectHandler>().GetCurrentEffects().Contains(Declarations.EffectType.Shield))
            {
                Health--;
                this.Raise(OnHealthChanged, new ValueArgs<int>(Health));
                if (Health > 0)
                {
                    GameManager.instance.AudioManager.Play(Constants.AudioKeys.explosion, AudioGroup.Sfx,
                        false, Player.transform.position);
                    Player.ResetShip();
                }
                else
                {
                    Player.GetComponent<EffectHandler>().ClearEffects();
                    mPoolManager.ReturnObject(mGameMusicSource);
                    this.Raise(OnGameOver, new ValueArgs<int>(Score));
                }
            }
        }

        private void SplitAsteroid(int level, Vector3 spawnPos, Vector3 hitDir)
        {
            var nextLevel = level - 1;
            var asteroids = mPoolManager.RetrieveObjects<Asteroid>(2);
            var angle = Mathf.Atan2(hitDir.y, hitDir.x) * Mathf.Rad2Deg;
            var speeds = GameManager.instance.Config.asteroidSpeeds.First(x => x.asteroidLevel == nextLevel);

            foreach (var asteroidObject in asteroids)
            {
                var asteroid = asteroidObject.GetComponent<Asteroid>();

                asteroid.Init(nextLevel, Random.Range(speeds.minSpeed, speeds.maxSpeed));
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
            var cameraOffset = GameManager.instance.Config.cameraZOffset;
            //left
            mWrapAroundCameras[0].transform.position = new Vector3(fieldPos.x - FieldSize.x, fieldPos.y, cameraOffset);
            //leftUp
            mWrapAroundCameras[1].transform.position =
                new Vector3(fieldPos.x - FieldSize.x, fieldPos.y + FieldSize.y, cameraOffset);
            //up
            mWrapAroundCameras[2].transform.position = new Vector3(fieldPos.x, fieldPos.y + FieldSize.y, cameraOffset);
            //rightUp
            mWrapAroundCameras[3].transform.position =
                new Vector3(fieldPos.x + FieldSize.x, fieldPos.y + FieldSize.y, cameraOffset);
            //right
            mWrapAroundCameras[4].transform.position = new Vector3(fieldPos.x + FieldSize.x, fieldPos.y, cameraOffset);
            //rightBot
            mWrapAroundCameras[5].transform.position =
                new Vector3(fieldPos.x + FieldSize.x, fieldPos.y - FieldSize.y, cameraOffset);
            //bot
            mWrapAroundCameras[6].transform.position = new Vector3(fieldPos.x, fieldPos.y - FieldSize.y, cameraOffset);
            //leftBot
            mWrapAroundCameras[7].transform.position =
                new Vector3(fieldPos.x - FieldSize.x, fieldPos.y - FieldSize.y, cameraOffset);

            for (var i = 0; i < mWrapAroundCameras.Length; i++) mWrapAroundCameras[i].orthographicSize = cameSize;
        }

        private void PositionBoundaries()
        {
            var fieldPos = Vector2.zero;
            var boundaryThickness = GameManager.instance.Config.levelBoundaryThickness;
            var leftBoundaryPos = new Vector3(fieldPos.x - FieldSize.x / 2 - boundaryThickness / 2, fieldPos.y);
            var rightBoundaryPos = new Vector3(fieldPos.x + FieldSize.x / 2 + boundaryThickness / 2, fieldPos.y);
            var topBoundaryPos = new Vector3(fieldPos.x, fieldPos.y + FieldSize.y / 2 + boundaryThickness / 2);
            var botBoundaryPos = new Vector3(fieldPos.x, fieldPos.y - FieldSize.y / 2 - boundaryThickness / 2);

            var verticalBoundarySize = new Vector2(boundaryThickness, FieldSize.y + boundaryThickness * 2);
            var horizontalBoundarySize = new Vector2(FieldSize.x, boundaryThickness);


            mBoundaryColliders[0].transform.position = leftBoundaryPos;
            mBoundaryColliders[0].size = verticalBoundarySize;

            mBoundaryColliders[1].transform.position = rightBoundaryPos;
            mBoundaryColliders[1].size = verticalBoundarySize;

            mBoundaryColliders[2].transform.position = topBoundaryPos;
            mBoundaryColliders[2].size = horizontalBoundarySize;

            mBoundaryColliders[3].transform.position = botBoundaryPos;
            mBoundaryColliders[3].size = horizontalBoundarySize;
        }
        #endregion

        private LevelData mCurrentLevel;

        private ShipData mFlyingSaucerData;

        private AudioSource mGameMusicSource;

        private PoolManager mPoolManager;

        private float mStartLevelTime;
        
        [SerializeField] private Camera[] mWrapAroundCameras;
        [SerializeField] private BoxCollider2D[] mBoundaryColliders;
    }
}