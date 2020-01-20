using System;
using UnityEngine;
using Utils;
using CollisionArgs = Utils.ValueArgs<System.Tuple<UnityEngine.Collider2D, UnityEngine.Collider2D>>;

namespace Game
{
    /// <summary>
    ///     Responsible for reporting back using events on collisions.
    /// </summary>
    public class CollisionReporter
        : MonoBehaviour
    {
        protected event EventHandler<CollisionArgs> TriggerEnter;
        protected event EventHandler<CollisionArgs> TriggerStay;
        protected event EventHandler<CollisionArgs> TriggerExit;

        private void OnTriggerEnter2D(Collider2D data)
        {
            this.Raise(TriggerEnter, new CollisionArgs(Tuple.Create(GetComponent<Collider2D>(), data)));
        }

        private void OnTriggerStay2D(Collider2D data)
        {
            this.Raise(TriggerStay, new CollisionArgs(Tuple.Create(GetComponent<Collider2D>(), data)));
        }

        private void OnTriggerExit2D(Collider2D data)
        {
            this.Raise(TriggerExit, new CollisionArgs(Tuple.Create(GetComponent<Collider2D>(), data)));
        }

        /// <summary>
        /// Subscribe to collision enter
        /// </summary>
        /// <param name="handler">Collision handler</param>
        /// <returns>this</returns>
        public CollisionReporter CallOnEnter(EventHandler<CollisionArgs> handler)
        {
            TriggerEnter += handler;
            return this;
        }

        /// <summary>
        /// Subscribe to collision stay
        /// </summary>
        /// <param name="handler">Collision handler</param>
        /// <returns>this</returns>
        public CollisionReporter CallOnStay(EventHandler<CollisionArgs> handler)
        {
            TriggerStay += handler;
            return this;
        }

        /// <summary>
        /// Subscribe to collision exit
        /// </summary>
        /// <param name="handler">Collision handler</param>
        /// <returns>this</returns>
        public CollisionReporter CallOnExit(EventHandler<CollisionArgs> handler)
        {
            TriggerExit += handler;
            return this;
        }

        /// <summary>
        /// Unubscribe to collision enter
        /// </summary>
        /// <param name="handler">Collision handler</param>
        /// <returns>this</returns>
        public CollisionReporter UnsubscribeOnEnter(EventHandler<CollisionArgs> handler)
        {
            TriggerEnter -= handler;
            return this;
        }

        /// <summary>
        /// Unsubscribe to collision stay
        /// </summary>
        /// <param name="handler">Collision handler</param>
        /// <returns>this</returns>
        public CollisionReporter UnsubscribeOnStay(EventHandler<CollisionArgs> handler)
        {
            TriggerStay -= handler;
            return this;
        }

        /// <summary>
        /// Unsubscribe to collision exit
        /// </summary>
        /// <param name="handler">Collision handler</param>
        /// <returns>this</returns>
        public CollisionReporter UnsubscribeOnExit(EventHandler<CollisionArgs> handler)
        {
            TriggerExit -= handler;
            return this;
        }
    }
}