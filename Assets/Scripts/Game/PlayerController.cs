﻿using System;
using System.Collections;
using System.Linq;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.PlayerLoop;
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

        protected override void Awake()
        {
            base.Awake();
            mWrapAround = GetComponent<WrapAroundObject>();
            mEffectHandler = GetComponent<EffectHandler>();

            mWrapAround.OnBeginTeleport += BeginTeleport;
            mEffectHandler.OnEffectsUpdated += EffectsUpdated;
            
            Debug.Assert(engineParticle != null);   
            Debug.Assert(gunpoint != null);   
            Debug.Assert(shieldSpriteRenderer != null);   
        }

        protected virtual void Start()
        {
            var inputController = GameManager.Instance.InputManager.GetInputController();
            if (GameManager.Instance.Config.ControlScheme == Declarations.ControlScheme.Desktop)
                inputController.GetDirectionalInput().OnDirectinalInput += ProcessDesktopDirectionalInput;
            else
                inputController.GetDirectionalInput().OnDirectinalInput += ProcessMobileDirectionalInput;
            inputController.GetFireInput().OnButtonInput += OnFireInput;
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

            mEffectHandler.AddEffect(GameManager.Instance.Config.playerStartEffect);
        }
        
        /// <summary>
        /// Shoots a bullet
        /// </summary>
        protected override void Shoot()
        {
            base.Shoot();
            mLastBulletTime = Time.realtimeSinceStartup;
            var bulletObject = GameManager.Instance.PoolManager.RetrieveObject<BulletController>();
            var bulletTransform = bulletObject.transform;

            bulletTransform.position = gunpoint.position;
            bulletTransform.rotation = gunpoint.rotation;

            bulletObject.Init(ProjectileMode.Friendly,
                mEffectHandler.GetCurrentEffects().Contains(Declarations.EffectType.HomingAmmo)
                    ? ProjectileType.Homing
                    : ProjectileType.Normal);
            bulletObject.gameObject.SetActive(true);
        }

        #region Input
        
        private void OnFireInput(object sender, EventArgs e)
        {
            if (GameManager.Instance.GamePaused || !mActive) return;

            if (HasCooledDown()) Shoot();
        }

        private void ProcessDesktopDirectionalInput(object sender, ValueArgs<Vector2> args)
        {
            if (GameManager.Instance.GamePaused || !mActive) return;

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
            if (GameManager.Instance.GamePaused || !mActive) return;

            var thisTransform = transform;
            var input = new Vector2(args.Value.x, Mathf.Clamp(args.Value.y, 0, 1));
            var dir = input - Vector2.zero;

            var targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            var accelerationPower = Mathf.Sqrt(input.y * input.y + input.x * input.x);
            var speed = mShipData.Acceleration * accelerationPower;
            if (mEffectHandler.GetCurrentEffects().Contains(Declarations.EffectType.Speed)) speed *= 1.3f;

            mVelocity += Time.deltaTime * speed * thisTransform.right;

            thisTransform.rotation =
                Quaternion.Lerp(thisTransform.rotation, Quaternion.Euler(new Vector3(0, 0, targetAngle)),
                    mShipData.AngularVelocity * Time.deltaTime);
        }
        
        #endregion

        #region EventHandlers

        private void BeginTeleport(object sender, EventArgs eventArgs)
        {
            DisableTrail();
        }

        private void EffectsUpdated(object sender, ValueArgs<Declarations.EffectType[]> e)
        {
            shieldSpriteRenderer.enabled = e.Value.Contains(Declarations.EffectType.Shield);

            var targetColor = e.Value.Contains(Declarations.EffectType.Speed)
                ? GameManager.Instance.Config.boostedEngineColor
                : GameManager.Instance.Config.engineColor;

            var engineColor = engineParticle.colorOverLifetime;
            engineColor.color = targetColor;
        }
        
        #endregion
        
        private void DisableTrail(bool enableAfterwards = true)
        {
            engineParticle.Pause();

            if (enableAfterwards) StartCoroutine(EnableTrail());
        }
        
        private IEnumerator EnableTrail()
        {
            yield return new WaitForEndOfFrame();
            engineParticle.Play();
        }
        
        private EffectHandler mEffectHandler;

        private WrapAroundObject mWrapAround;

        [SerializeField] private ParticleSystem engineParticle;
        
        [SerializeField] private Transform gunpoint;

        [SerializeField] private SpriteRenderer shieldSpriteRenderer;
    }
}