using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class ShipController : MonoBehaviour
    {
        private readonly Vector3 shipInitialRotation = new Vector3(0, 0, 90);
        [SerializeField] private Transform gunpoint;

        private WrapAroundObject wrapAround;
        private ParticleSystem particleSystem;
        private void Awake()
        {
            particleSystem = GetComponentInChildren<ParticleSystem>();
            wrapAround = GetComponent<WrapAroundObject>();
            float currentTime = Time.realtimeSinceStartup;
            mLastBulletTime = currentTime;
            wrapAround.OnBeginTeleport += BeginTeleport;
        
            var inputController = GameManager.instance.InputManager.GetInputController();
            inputController.GetDirectionalInput().OnDirectinalInput += ProcessDirectinalInput;
            inputController.GetFireInput().OnButtonInput += Shoot;
        }

        private void BeginTeleport()
        {
            DisableTrail(true);
        }

        private void DisableTrail(bool enableAfterwards)
        {
            particleSystem.Pause();

            if (enableAfterwards)
            {
                StartCoroutine(EnableTrail());
            }
        }

        public void ResetShip()
        {
            gameObject.SetActive(true);
            DisableTrail(true);
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.Euler(shipInitialRotation);
            mVelocity = Vector3.zero;
        }


        private IEnumerator EnableTrail()
        {
            yield return new WaitForEndOfFrame();
            particleSystem.Play();
        }

        private void Update()
        {
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            transform.position += mVelocity * Time.deltaTime;
            mVelocity *= Mathf.Pow(DAMPING, Time.deltaTime);
        }

        private void ProcessDirectinalInput(Vector2 input)
        {
            input.y = Mathf.Clamp(input.y, 0, 1);
            mVelocity += Time.deltaTime * ACCELERATION * input.y * transform.right;
            transform.Rotate(new Vector3(0.0f, 0.0f, ANGULAR_VELOCITY * Time.deltaTime * input.x * -1));
        }

        private void Shoot()
        {
            if (HasCooledDown(mLastBulletTime))
            {
                mLastBulletTime = Time.realtimeSinceStartup;
                var bulletObject = GameManager.instance.LevelManager.PoolManager.RetrieveObject<BulletController>();
                var bulletTransform = bulletObject.transform;
                
                bulletTransform.position = gunpoint.position;
                bulletTransform.rotation = gunpoint.rotation;
                
                bulletObject.Init(Defines.ProjectileMode.Friendly);
                bulletObject.gameObject.SetActive(true);
            }
        }

        private bool HasCooledDown(float time)
        {
            return (Time.realtimeSinceStartup - time >= 0.3f);
        }

        private Vector3 mVelocity;
        private float mLastBulletTime;

        private const float ACCELERATION = 10.0f;
        private const float DAMPING = 0.4f;
        private const float ANGULAR_VELOCITY = 80.0f;
    }
}
