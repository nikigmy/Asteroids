using Managers;
using ScriptableObjects;
using UnityEngine;
using Utils;

namespace Game
{
    /// <summary>
    /// Base controller for ships
    /// </summary>
    public abstract class ShipController : MonoBehaviour
    {
        protected virtual void Awake()
        {
            mActive = false;
            mLastBulletTime = Time.realtimeSinceStartup;
        }

        protected virtual void Update()
        {
            if (mShipData != null) ApplyMovement();
        }
        
        protected virtual void OnDisable()
        {
            mActive = false;
        }

        /// <summary>
        /// Initialise the ship
        /// </summary>
        /// <param name="data">Data of the ship</param>
        public virtual void Init(ShipData data)
        {
            mAudioManager = GameManager.Instance.AudioManager;
            mShipData = data;
            mActive = true;
        }

        /// <summary>
        /// Resets the ship
        /// </summary>
        public virtual void ResetShip()
        {
            mVelocity = Vector3.zero;
        }

        /// <summary>
        /// Apply mVelocity and Damping to the ship transform
        /// </summary>
        protected virtual void ApplyMovement()
        {
            transform.position += mVelocity * Time.deltaTime;
            mVelocity *= Mathf.Pow(mShipData.Damping, Time.deltaTime);
        }

        /// <summary>
        /// Checks if the firing has cooled down
        /// </summary>
        /// <returns>Whether the firing has cooled down</returns>
        protected virtual bool HasCooledDown()
        {
            return Time.realtimeSinceStartup - mLastBulletTime >= mShipData.FireRate;
        }

        /// <summary>
        /// Plays shooting sound
        /// </summary>
        protected virtual void Shoot()
        {
            mAudioManager.Play(Constants.AudioKeys.cLazerBlast, AudioGroup.Sfx, false,
                transform.position);
        }

        private AudioManager mAudioManager;
        
        protected bool mActive;
        
        protected float mLastBulletTime;

        protected ShipData mShipData;

        protected Vector3 mVelocity;
    }
}