using System;
using UnityEngine;

namespace Game
{
    public class BulletController : Projectile
    {
        public float projectileLife = 2;
        public float ProjectileSpeed = 1.0f;
        private bool active;

        private void Awake()
        {
            active = false;
        }

        public void Init(Defines.ProjectileMode mode)
        {
            this.mode = mode;
            active = true;
            Invoke("DestroyProjectile", projectileLife);
        }

        private void DestroyProjectile()
        {
            GameManager.instance.LevelManager.PoolManager.ReturnObject(this);
        }

        private void OnDisable()
        {
            active = false;
            CancelInvoke("DestroyProjectile");
        }

        private void Update()
        {
            if (active)
            {
                transform.position += Time.deltaTime * ProjectileSpeed * transform.right;
            }
        }
    }
}
