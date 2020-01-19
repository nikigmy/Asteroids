using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Utils;

namespace Managers
{
    /// <summary>
    /// Handles all collisions of CollisionReporters and forwards them to subscribed event handlers
    /// </summary>
    public class CollisionManager : MonoBehaviour
    {
        private void Awake()
        {
            mCollisionEnterHandlers = new Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>>();
            mCollisionStayHandlers = new Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>>();
            mCollisionExitHandlers = new Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>>();
            mHandledCollisions = new List<Tuple<Collider2D, Collider2D>>(10);
        }

        private void FixedUpdate()
        {
            mHandledCollisions.Clear();
        }

        /// <summary>
        /// Register a collision reporter
        /// </summary>
        /// <param name="reporter">The reporter to register</param>
        public void RegisterCollisionReporter(CollisionReporter reporter)
        {
            if (reporter != null) reporter.CallOnEnter(CollisionEnter).CallOnStay(CollisionStay).CallOnExit(CollisionExit);
        }

        /// <summary>
        /// Subscribe to collision enter
        /// </summary>
        /// <param name="firstTag">First tag</param>
        /// <param name="secondTag">First tag</param>
        /// <param name="handler">Collision handler</param>
        public void SubscribeOnEnter(string firstTag, string secondTag, Action<Tuple<Collider2D, Collider2D>> handler)
        {
            mCollisionEnterHandlers.Add(GenerateDictionaryKey(firstTag, secondTag), handler);
        }
        
        /// <summary>
        /// Subscribe to collision stay
        /// </summary>
        /// <param name="firstTag">First tag</param>
        /// <param name="secondTag">First tag</param>
        /// <param name="handler">Collision handler</param>
        public void SubscribeOnStay(string firstTag, string secondTag, Action<Tuple<Collider2D, Collider2D>> handler)
        {
            mCollisionStayHandlers.Add(GenerateDictionaryKey(firstTag, secondTag), handler);
        }
        
        /// <summary>
        /// Subscribe to collision exit
        /// </summary>
        /// <param name="firstTag">First tag</param>
        /// <param name="secondTag">First tag</param>
        /// <param name="handler">Collision handler</param>
        public void SubscribeOnExit(string firstTag, string secondTag, Action<Tuple<Collider2D, Collider2D>> handler)
        {
            mCollisionExitHandlers.Add(GenerateDictionaryKey(firstTag, secondTag), handler);
        }

        private void CollisionEnter(object sender, ValueArgs<Tuple<Collider2D, Collider2D>> collision)
        {
            var collisionKey = GenerateCollisionKey(collision.Value.Item1, collision.Value.Item2);
            if (!mHandledCollisions.Contains(collisionKey))
            {
                var collisionPair = GenerateDictionaryKey(collision.Value.Item1.gameObject.tag,
                    collision.Value.Item2.gameObject.tag);
                if (mCollisionEnterHandlers.ContainsKey(collisionPair))
                    mCollisionEnterHandlers[collisionPair].Invoke(collision.Value);

                mHandledCollisions.Add(collisionKey);
            }
        }

        private void CollisionStay(object sender, ValueArgs<Tuple<Collider2D, Collider2D>> collision)
        {
            var collisionKey = GenerateCollisionKey(collision.Value.Item1, collision.Value.Item2);
            if (!mHandledCollisions.Contains(collisionKey))
            {
                var collisionPair = GenerateDictionaryKey(collision.Value.Item1.gameObject.tag,
                    collision.Value.Item2.gameObject.tag);
                if (mCollisionStayHandlers.ContainsKey(collisionPair))
                    mCollisionStayHandlers[collisionPair].Invoke(collision.Value);

                mHandledCollisions.Add(collisionKey);
            }
        }

        private void CollisionExit(object sender, ValueArgs<Tuple<Collider2D, Collider2D>> collision)
        {
            var collisionKey = GenerateCollisionKey(collision.Value.Item1, collision.Value.Item2);
            if (!mHandledCollisions.Contains(collisionKey))
            {
                var collisionPair = GenerateDictionaryKey(collision.Value.Item1.gameObject.tag,
                    collision.Value.Item2.gameObject.tag);
                if (mCollisionExitHandlers.ContainsKey(collisionPair))
                    mCollisionExitHandlers[collisionPair].Invoke(collision.Value);

                mHandledCollisions.Add(collision.Value);
            }
        }

        #region KeyGenerators
        
        /// <summary>
        /// Generates a key from the two colliders
        /// </summary>
        /// <param name="firstCollider">First collider</param>
        /// <param name="secondCollider">Second collider</param>
        /// <returns>Collision key</returns>
        private Tuple<Collider2D, Collider2D> GenerateCollisionKey(Collider2D firstCollider, Collider2D secondCollider)
        {
            if (firstCollider.GetHashCode() > secondCollider.GetHashCode())
                return Tuple.Create(firstCollider, secondCollider);
            return Tuple.Create(secondCollider, firstCollider);
        }

        /// <summary>
        /// Generates a key from the two tags
        /// </summary>
        /// <param name="firstTag">First tag</param>
        /// <param name="secondTag">Second gag</param>
        /// <returns>Dictionary key</returns>
        private Tuple<string, string> GenerateDictionaryKey(string firstTag, string secondTag)
        {
            Tuple<string, string> result;
            var compareResult = string.Compare(firstTag, secondTag, StringComparison.Ordinal);
            if (compareResult > 0)
                result = Tuple.Create(firstTag, secondTag);
            else
                result = Tuple.Create(secondTag, firstTag);

            return result;
        }
        
        #endregion

        private Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>> mCollisionEnterHandlers;
        
        private Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>> mCollisionExitHandlers;
        
        private Dictionary<Tuple<string, string>, Action<Tuple<Collider2D, Collider2D>>> mCollisionStayHandlers;
        
        private List<Tuple<Collider2D, Collider2D>> mHandledCollisions;
    }
}