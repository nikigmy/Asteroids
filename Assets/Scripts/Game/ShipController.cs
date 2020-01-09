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
    }

    private void DisableTrail()
    {
        Debug.Log("Pause");
        particleSystem.Pause();

        StartCoroutine(EnableTrail());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            transform.position = Vector3.zero;
            mVelocity = Vector3.zero;
        }
    }


    private IEnumerator EnableTrail()
    {
        yield return new WaitForEndOfFrame();
        
        Debug.Log("Play");
        particleSystem.Play();
    }

    private void Update()
    {
        ApplyMovement();
//        if (EventSystem.current.IsPointerOverGameObject())
//        {
//            return;
//        }
        ProcessContinuousInput();
        ProcessActionInput();
    }

    private void ApplyMovement()
    {
        transform.position += mVelocity * Time.deltaTime;
        mVelocity *= Mathf.Pow(DAMPING, Time.deltaTime);
    }

    private void ProcessContinuousInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            mVelocity += Time.deltaTime * ACCELERATION * transform.right;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            transform.Rotate(new Vector3(0.0f, 0.0f, ANGULAR_VELOCITY * Time.deltaTime));
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            transform.Rotate(new Vector3(0.0f, 0.0f, -ANGULAR_VELOCITY * Time.deltaTime));
    }

    private void ProcessActionInput()
    {
        if (HasCooledDown(mLastBulletTime) && Input.GetKeyDown(KeyCode.Space))
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
