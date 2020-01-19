using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using Utils;

namespace Game
{
    /// <summary>
    /// Controller for player ship
    /// </summary>
    [RequireComponent(typeof(EffectHandler), typeof(WrapAroundObject))]
    public class PlayerController : ShipController
    {
        private readonly Vector3 shipInitialRotation = new Vector3(0, 0, 90);
        
        protected virtual void Start()
        {
            mWrapAround = GetComponent<WrapAroundObject>();
            mEffectHandler = GetComponent<EffectHandler>();

            mWrapAround.OnBeginTeleport += BeginTeleport;

            var inputController = GameManager.instance.InputManager.GetInputController();
            if (GameManager.instance.Config.ControlScheme == Declarations.ControlScheme.Desktop)
                inputController.GetDirectionalInput().OnDirectinalInput += ProcessDesktopDirectionalInput;
            else
                inputController.GetDirectionalInput().OnDirectinalInput += ProcessMobileDirectionalInput;
            inputController.GetFireInput().OnButtonInput += OnFireInput;

            mEffectHandler.OnEffectsUpdated += EffectsUpdated;
        }

        /// <summary>
        /// Resets the ship
        /// </summary>
        public override void ResetShip()
        {
            base.ResetShip();

            gameObject.SetActive(true);
            DisableTrail();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.Euler(shipInitialRotation);

            mEffectHandler.AddEffect(GameManager.instance.Config.playerStartEffect);
        }

        /// <summary>
        /// Shoots a bullet
        /// </summary>
        protected override void Shoot()
        {
            base.Shoot();
            mLastBulletTime = Time.realtimeSinceStartup;
            var bulletObject = GameManager.instance.PoolManager.RetrieveObject<BulletController>();
            var bulletTransform = bulletObject.transform;

            bulletTransform.position = mGunpoint.position;
            bulletTransform.rotation = mGunpoint.rotation;

            bulletObject.Init(ProjectileMode.Friendly,
                mEffectHandler.GetCurrentEffects().Contains(Declarations.EffectType.HomingAmmo)
                    ? ProjectileType.Homing
                    : ProjectileType.Normal);
            bulletObject.gameObject.SetActive(true);
        }

        #region Input
        
        private void OnFireInput(object sender, EventArgs e)
        {
            if (GameManager.instance.GamePaused || !mActive) return;

            if (HasCooledDown()) Shoot();
        }

        private void ProcessDesktopDirectionalInput(object sender, ValueArgs<Vector2> args)
        {
            if (GameManager.instance.GamePaused || !mActive) return;

            var input = new Vector2(args.Value.x, Mathf.Clamp(args.Value.y, 0, 1));
            var speed = mShipData.Acceleration;
            
            if (mEffectHandler.GetCurrentEffects().Any(e => e == Declarations.EffectType.Speed))
            {
                speed *= 1.3f;
            }
            
            mVelocity += Time.deltaTime * speed * input.y * transform.right;

            transform.Rotate(new Vector3(0.0f, 0.0f, mShipData.AngularVelocity * Time.deltaTime * input.x * -1));
        }

        private void ProcessMobileDirectionalInput(object sender, ValueArgs<Vector2> args)
        {
            if (GameManager.instance.GamePaused || !mActive) return;

            var thisTransform = transform;
            var input = new Vector2(args.Value.x, Mathf.Clamp(args.Value.y, 0, 1));
            var dir = input - Vector2.zero;

            var targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            var accelerationPower = Mathf.Sqrt(input.y * input.y + input.x * input.x);
            var speed = mShipData.Acceleration * accelerationPower;
            if (mEffectHandler.GetCurrentEffects().Contains(Declarations.EffectType.Speed)) speed *= 1.3f;

            mVelocity += Time.deltaTime * speed * thisTransform.right;

            thisTransform.rotation =
                Quaternion.Lerp(thisTransform.rotation, Quaternion.Euler(new Vector3(0, 0, targetAngle)), mShipData.AngularVelocity * Time.deltaTime)
        }
        
        #endregion

        #region EventHandlers

        private void BeginTeleport(object sender, EventArgs eventArgs)
        {
            DisableTrail();
        }

        private void EffectsUpdated(object sender, ValueArgs<Declarations.EffectType[]> e)
        {
            mShieldSprite.enabled = e.Value.Contains(Declarations.EffectType.Shield);

            var targetColor = e.Value.Contains(Declarations.EffectType.Speed)
                ? GameManager.instance.Config.boostedEngineColor
                : GameManager.instance.Config.engineColor;

            var engineColor = mEngineParticle.colorOverLifetime;
            engineColor.color = targetColor;
        }
        
        #endregion
        
        private void DisableTrail(bool enableAfterwards = true)
        {
            mEngineParticle.Pause();

            if (enableAfterwards) StartCoroutine(EnableTrail());
        }
        
        private IEnumerator EnableTrail()
        {
            yield return new WaitForEndOfFrame();
            mEngineParticle.Play();
        }
        
        private EffectHandler mEffectHandler;

        private WrapAroundObject mWrapAround;

        [SerializeField] private ParticleSystem mEngineParticle;
        
        [SerializeField] private Transform mGunpoint;

        [SerializeField] private SpriteRenderer mShieldSprite;
    }
}