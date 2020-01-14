using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour
{
    public int Level
    {
        get { return level;} }
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
}
