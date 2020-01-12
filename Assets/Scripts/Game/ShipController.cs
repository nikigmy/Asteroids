using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ShipController : MonoBehaviour
{
    [SerializeField] private Transform gunpoint;
    public GameObject BulletPrefab;

    private WrapAroundObject wrapAround;
    private ParticleSystem particleSystem;
    private void Awake()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
        wrapAround = GetComponent<WrapAroundObject>();
        float currentTime = Time.realtimeSinceStartup;
        mLastBulletTime = currentTime;
        wrapAround.OnBeginTeleport += DisableTrail;
        
        var inputController = GameManager.instance.InputManager.GetInputController();
        inputController.GetDirectionalInput().OnDirectinalInput += ProcessDirectinalInput;
        inputController.GetFireInput().OnButtonInput += Shoot;
        
    }

    private void DisableTrail()
    {
        particleSystem.Pause();

        StartCoroutine(EnableTrail());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            transform.position = Vector3.zero;
            mVelocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(Vector3.forward * 90);
        }
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
    
    public void Shoot()
    {
        if (HasCooledDown(mLastBulletTime))
        {
            mLastBulletTime = Time.realtimeSinceStartup;
            SpawnProjectile(BulletPrefab);
        }
    }

    private bool HasCooledDown(float time)
    {
        return (Time.realtimeSinceStartup - time >= 0.3f);
    }

    private void SpawnProjectile(GameObject prefab)
    {
        GameObject projectile = Instantiate(prefab, gunpoint.position, gunpoint.rotation);
    }

    private Vector3 mVelocity;
    private float mLastBulletTime;

    private const float ACCELERATION = 10.0f;
    private const float DAMPING = 0.4f;
    private const float ANGULAR_VELOCITY = 80.0f;
}
