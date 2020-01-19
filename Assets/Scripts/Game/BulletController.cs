using Managers;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Behaviour for bullets
    /// </summary>
    public class BulletController : Projectile
    {
        #region Unity Functions
        
        private void Awake()
        {
            mActive = false;
        }
        
        private void Update()
        {
            if (mActive)
            {
                if (Type == ProjectileType.Homing && Mode == ProjectileMode.Friendly)
                {
                    if (mTarget == null || !mTarget.activeSelf) mTarget = GetClosestTarget();

                    if (mTarget != null)
                    {
                        var pos = transform.position;
                        var closest = Utils.Utils.GetClosestPosition(pos, mTarget.transform.position);

                        var dir = closest - pos;
                        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        var rot = Quaternion.Euler(new Vector3(0, 0, angle));

                        var lerpedRottion = Quaternion.Lerp(transform.rotation, rot,
                            Time.deltaTime * GameManager.instance.Config.homingRotationSpeed).eulerAngles;
                        transform.rotation = Quaternion.Euler(0, 0, lerpedRottion.z);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }

                transform.position += Time.deltaTime * GameManager.instance.Config.projectileSpeed * transform.right;
            }
        }

        private void OnDisable()
        {
            mActive = false;
            CancelInvoke("DestroyProjectile");
        }

        #endregion
        
        /// <summary>
        /// Initialise the bullet
        /// </summary>
        /// <param name="mode">Mode of the bullet</param>
        /// <param name="type">Type of the bullet</param>
        public override void Init(ProjectileMode mode, ProjectileType type)
        {
            base.Init(mode, type);
            mActive = true;

            Invoke("DestroyProjectile", GameManager.instance.Config.projectileLife);
            mTarget = GetClosestTarget();

            mGraphics.color = mode == ProjectileMode.Enemy
                ? GameManager.instance.Config.enemyProjectileColor
                : GameManager.instance.Config.playerProjectileColor;
        }

        private void DestroyProjectile()
        {
            GameManager.Instance.PoolManager.ReturnObject(this);
        }

        private GameObject GetClosestTarget()
        {
            var pos = transform.position;
            GameObject result = null;
            float minDistance = float.MaxValue;
            var asteroids = GameManager.instance.PoolManager.GetActiveObjects<Asteroid>();
            if (asteroids.Count > 0)
            {
                for (var i = 0; i < asteroids.Count; i++)
                {
                    var closestPoint = Utils.Utils.GetClosestPosition(pos, asteroids[i].transform.position);
                    var dist = Vector3.Distance(closestPoint, pos);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        result = asteroids[i].gameObject;
                    }
                }
            }

            var enemies = GameManager.instance.PoolManager.GetActiveObjects<FlyingSaucer>();
            if (enemies.Count > 0)
            {
                for (var i = 0; i < enemies.Count; i++)
                {
                    var closestPoint = Utils.Utils.GetClosestPosition(pos, enemies[i].transform.position);
                    var dist = Vector3.Distance(closestPoint, pos);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        result = enemies[i].gameObject;
                    }
                }
            }

            return result;
        }

        private bool mActive;
        private GameObject mTarget;

        [SerializeField] private SpriteRenderer mGraphics;
    }
}