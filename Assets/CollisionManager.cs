using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollisionManager : MonoBehaviour
{
    
    public event Defines.OnPlayerHit OnPlayerHit;
    public event Defines.OnAsteroidDestroyed OnAsteroidDestroyed;
    public event Defines.OnEnemyHit OnEnemyHit;

    private List<Tuple<Collider2D, Collider2D>> handledCollisions;
    Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>> collisionEnterHandlers;
    Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>> collisionExitHandlers;
    
    private void Awake()
    {
        collisionEnterHandlers = new Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>>();
        collisionExitHandlers = new Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>>();
        handledCollisions = new List<Tuple<Collider2D, Collider2D>>(10);
        
        collisionEnterHandlers.Add(GenerateDictionaryKey(Defines.Tags.asteroid, Defines.Tags.projectile), AsteroidDestroyed );
        collisionEnterHandlers.Add(GenerateDictionaryKey(Defines.Tags.asteroid, Defines.Tags.player), AsteroidPlayerHit );
        collisionEnterHandlers.Add(GenerateDictionaryKey(Defines.Tags.asteroid, Defines.Tags.enemy), AsteroidEnemyHit );
        collisionEnterHandlers.Add(GenerateDictionaryKey(Defines.Tags.projectile, Defines.Tags.player), BulletPlayerHit );
        collisionEnterHandlers.Add(GenerateDictionaryKey(Defines.Tags.projectile, Defines.Tags.enemy), BulletEnemyHit );
        collisionEnterHandlers.Add(GenerateDictionaryKey(Defines.Tags.enemy, Defines.Tags.player), EnemyPlayerHit );
        collisionEnterHandlers.Add(GenerateDictionaryKey(Defines.Tags.player, Defines.Tags.pickup), OnPickup);
    }

    private void OnPickup(Tuple<Collider2D, Collider2D> obj)
    {
//        throw new NotImplementedException();
    }

    private void EnemyPlayerHit(Tuple<Collider2D, Collider2D> obj)
    {
        OnPlayerHit();
    }

    private void BulletEnemyHit(Tuple<Collider2D, Collider2D> obj)
    {
//        throw new NotImplementedException();
    }

    private void BulletPlayerHit(Tuple<Collider2D, Collider2D> obj)
    {
//        throw new NotImplementedException();
    }

    private void AsteroidEnemyHit(Tuple<Collider2D, Collider2D> obj)
    {
//        throw new NotImplementedException();
    }

    private void AsteroidPlayerHit(Tuple<Collider2D, Collider2D> obj)
    {
        OnPlayerHit();
    }

    private void AsteroidDestroyed(Tuple<Collider2D, Collider2D> collision)
    {
        Asteroid asteroid;
        BulletController bullet;
        if (collision.Item1.gameObject.CompareTag(Defines.Tags.asteroid))
        {
            asteroid = collision.Item1.GetComponent<Asteroid>();
            bullet = collision.Item2.GetComponent<BulletController>();
        }
        else
        {
            asteroid = collision.Item2.GetComponent<Asteroid>();
            bullet = collision.Item1.GetComponent<BulletController>();
        }
        OnAsteroidDestroyed?.Invoke(asteroid, bullet);
    }

    private void FixedUpdate()
    {
        handledCollisions.Clear();
    }

    public void RegisterCollisionReporter(CollisionReporter reporter)
    {
        if (reporter != null)
        {
            reporter.CallOnEnter(CollisionEnter);
            reporter.CallOnExit(CollisionExit);
        }
    }

    private void CollisionEnter(object sender, ValueArgs<Tuple<Collider2D, Collider2D>> collision)
    {
        var collisionKey = GenerateCollisionKey(collision.Value.Item1, collision.Value.Item2);
        if (!handledCollisions.Contains(collisionKey))
        {
            var collisionPair = GenerateDictionaryKey(collision.Value.Item1.gameObject.tag,
                collision.Value.Item2.gameObject.tag);
            if (collisionEnterHandlers.ContainsKey(collisionPair))
            {
                collisionEnterHandlers[collisionPair].Invoke(collision.Value);
            }

            handledCollisions.Add(collisionKey);
        }
    }

    private void CollisionExit(object sender, ValueArgs<Tuple<Collider2D, Collider2D>> collision)
    {
        var collisionKey = GenerateCollisionKey(collision.Value.Item1, collision.Value.Item2);
        if (!handledCollisions.Contains(collisionKey))
        {
            var collisionPair = GenerateDictionaryKey(collision.Value.Item1.gameObject.tag,
                collision.Value.Item2.gameObject.tag);
            if (collisionExitHandlers.ContainsKey(collisionPair))
            {
                collisionExitHandlers[collisionPair].Invoke(collision.Value);
            }

            handledCollisions.Add(collision.Value);
        }
    }

    private Tuple<Collider2D, Collider2D> GenerateCollisionKey(Collider2D firstCollider, Collider2D secondCollider)
    {
        if (firstCollider.GetHashCode() > secondCollider.GetHashCode())
        {
            return Tuple.Create(firstCollider, secondCollider);
        }
        else
        {
            return Tuple.Create(secondCollider, firstCollider);
        }
    }

    private Tuple<string, string> GenerateDictionaryKey(string firstTag, string secondTag)
    {
        Tuple<string, string> result;
        var compareResult = string.Compare(firstTag, secondTag, StringComparison.Ordinal);
        if (compareResult > 0)
        {
            result = Tuple.Create(firstTag, secondTag);
        }
        else
        {
            result = Tuple.Create(secondTag, firstTag);
        }

        return result;
    }
}
