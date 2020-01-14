using System;
using UnityEngine;

public class GenericObjectGenerator<T> : IObjectGenerator<T> where T : MonoBehaviour  
{
    private readonly T templateObject;

    public GenericObjectGenerator(string prefabPath)
    {
        templateObject = Utils.LoadObject<T>(prefabPath);
    }
    
    public virtual T GenerateObject(Transform parent = null)
    {
        T obj;
        if (parent != null)
        {
            obj = GameObject.Instantiate(templateObject, parent);
        }
        else
        {
            obj = GameObject.Instantiate(templateObject);
        }

        var collisionReporter = obj.GetComponent<CollisionReporter>();
        if (collisionReporter != null)
        {
            GameManager.instance.LevelManager.CollisionManager.RegisterCollisionReporter(collisionReporter);
        }

        return obj;
    }

    public virtual T[] GenerateObjects(int count, Transform parent = null)
    {
        var result = new T[count];
        for (int i = 0; i < count; i++)
        {
            T obj;
            if (parent != null)
            {
                obj = GameObject.Instantiate(templateObject, parent);
            }
            else
            {
                obj = GameObject.Instantiate(templateObject);
            }

            var collisionReporter = obj.GetComponent<CollisionReporter>();
            if (collisionReporter != null)
            {
                GameManager.instance.LevelManager.CollisionManager.RegisterCollisionReporter(collisionReporter);
            }

            result[i] = obj;
        }

        return result;
    }
}