using Managers;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Controller for enemy flying saucer ship
    /// </summary>
    public class FlyingSaucer : ShipController
    {
        private const float cProjectileOffset = 1;

        protected override void Update()
        {
            if (GameManager.instance.GamePaused || !mActive) return;

            AddVelocity();
            base.Update();
            if (HasCooledDown()) Shoot();
        }

        /// <summary>
        /// Sets the movement vector of the saucer
        /// </summary>
        /// <param name="vector">Directional vector</param>
        public void SetMovementVector(Vector3 vector)
        {
            mMovementVector = vector;
        }

        /// <summary>
        /// Shoots a bullet at the player
        /// </summary>
        protected override void Shoot()
        {
            base.Shoot();
            mLastBulletTime = Time.realtimeSinceStartup;

            var bulletObject = GameManager.instance.PoolManager.RetrieveObject<BulletController>();
            var bulletTransform = bulletObject.transform;

            var player = GameManager.instance.LevelManager.Player;
            var position = transform.position;
            var dir = Utils.Utils.GetClosestPosition(position, player.transform.position) - position;

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + Utils.Utils.GenerateRandomValue(0, GameManager.instance.Config.saucerShotRandomisation);

            bulletTransform.position = position + dir.normalized * cProjectileOffset;
            bulletTransform.rotation = Quaternion.Euler(Vector3.forward * angle);

            bulletObject.Init(ProjectileMode.Enemy, ProjectileType.Normal);
            bulletObject.gameObject.SetActive(true);
        }

        private void AddVelocity()
        {
            mVelocity += Time.deltaTime * mShipData.Acceleration * mMovementVector;
        }
        
        private Vector3 mMovementVector;
    }
}