using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour
{
    public event Defines.OnAsteroidDestroyedDelegate OnAsteroidDestroyed;
    private int level;
    [SerializeField]
    private float speed;

    public void SetData(int level, float speed)
    {
        this.speed = speed;
        this.level = level;

        transform.localScale = Vector3.one * GameManager.instance.Config.scaleLevels[level - 1];
    }

    private void Update()
    {
        transform.position += Time.deltaTime * speed * transform.right;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            if (OnAsteroidDestroyed != null) OnAsteroidDestroyed(gameObject, level, other.transform.position - transform.position);
        }
    }
}
