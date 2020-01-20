using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
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

        protected override void Awake()
        {
            base.Awake();
            mWrapAround = GetComponent<WrapAroundObject>();
            mEffectHandler = GetComponent<EffectHandler>();

            mWrapAround.OnBeginTeleport += BeginTeleport;
            mEffectHandler.OnEffectAdded += OnEffectAdded;
            mEffectHandler.OnEffectExpired += OnEffectExpired;
            
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
            
            mAcceleration = mShipData.Acceleration;
            shieldSpriteRenderer.enabled = false;
            var engineColor = engineParticle.colorOverLifetime;
            engineColor.color = GameManager.Instance.Config.engineColor;

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

            bulletObject.Init(ProjectileMode.Friendly, mProjectileType);
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
            
            mVelocity += Time.deltaTime * mAcceleration * input.y * transform.right;

            transform.Rotate(new Vector3(0.0f, 0.0f, mShipData.AngularVelocity * Time.deltaTime * input.x * -1));
        }

        private void ProcessMobileDirectionalInput(object sender, ValueArgs<Vector2> args)
        {
            if (GameManager.Instance.GamePaused || !mActive) return;

            var thisTransform = transform;
            var input = new Vector2(args.Value.x, args.Value.y);
            var dir = input - Vector2.zero;

            var targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            var accelerationPower = Mathf.Sqrt(input.y * input.y + input.x * input.x);
            var speed = mAcceleration * accelerationPower;

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

        private void OnEffectAdded(object sender, ValueArgs<Declarations.EffectType> effect)
        {
            switch (effect.Value)
            {
                case Declarations.EffectType.Speed:
                    mAcceleration = mShipData.Acceleration + ((mShipData.Acceleration * GameManager.Instance.Config.speedBoostPercentage) / 100);
                    var engineColor = engineParticle.colorOverLifetime;
                    engineColor.color = GameManager.Instance.Config.boostedEngineColor;
                    break;
                case Declarations.EffectType.Shield:
                    shieldSpriteRenderer.enabled = true;
                    break;
                case Declarations.EffectType.HomingAmmo:
                    mProjectileType = ProjectileType.Homing;
                    break;
                default:
                    Debug.LogError("Unknown effect type");
                    break;
            }
        }
        
        private void OnEffectExpired(object sender, ValueArgs<Declarations.EffectType> effect)
        {
            switch (effect.Value)
            {
                case Declarations.EffectType.Speed:
                    mAcceleration = mShipData.Acceleration;
                    var engineColor = engineParticle.colorOverLifetime;
                    engineColor.color = GameManager.Instance.Config.engineColor;
                    break;
                case Declarations.EffectType.Shield:
                    shieldSpriteRenderer.enabled = false;
                    break;
                case Declarations.EffectType.HomingAmmo:
                    mProjectileType = ProjectileType.Normal;
                    break;
                default:
                    Debug.LogError("Unknown effect type");
                    break;
            }
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

        private ProjectileType mProjectileType;

        private float mAcceleration;
        
        private EffectHandler mEffectHandler;

        private WrapAroundObject mWrapAround;

        [SerializeField] private ParticleSystem engineParticle;
        
        [SerializeField] private Transform gunpoint;

        [SerializeField] private SpriteRenderer shieldSpriteRenderer;
    }
}