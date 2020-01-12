using System;
using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    public float projectileLife = 2;
    public float ProjectileSpeed = 1.0f;

    private void Awake()
    {
        Invoke("DestroyProjectile", projectileLife);
    }

    private void DestroyProjectile()
    {
        GameManager.instance.LevelManager.PoolManager.ReturnObject(Defines.PoolKey.Bullet, gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid") || other.CompareTag("EnemyBullet") || other.CompareTag("Player"))
        {
            GameManager.instance.LevelManager.PoolManager.ReturnObject(Defines.PoolKey.Bullet, gameObject);
        }
    }

    private void Update()
    {
        transform.position += Time.deltaTime * ProjectileSpeed * transform.right;
    }
}
