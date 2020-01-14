using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Utils
{
    public static float GenerateRandomValue(float min, float max)
    {
        var val = Random.Range(min,max);
        FlipDirectiononRandom(ref val);
        
        return val;
    }

    private static void FlipDirectiononRandom(ref float value)
    {
        if ((Random.value > 0.5f))
        {
            value *= -1;
        }
    }
    public static GameObject LoadObject(string prefabPath)
    {
        return Resources.Load(prefabPath) as GameObject;
    }

    public static T LoadObject<T>(string prefabPath) where T : MonoBehaviour
    {
        T result = null;
        var loadedObject = Resources.Load(prefabPath);
        if (loadedObject != null)
        {
            if (typeof(T) != typeof(GameObject))
            {
                result = (loadedObject as GameObject).GetComponent<T>();
            }
            else
            {
                result = loadedObject as T;
            }
        }
        else
        {
            Debug.LogError("Cant load prefab: " + prefabPath);
        }

        return result;
    }
    
    public static T FindOrLoad<T>(string prefabPath) where T : MonoBehaviour
    {
        var obj = GameObject.FindObjectOfType<T>();

        if (obj == null)
        {
            obj = GameObject.Instantiate(LoadObject<T>(prefabPath));

            Debug.LogError("Cant find: " + typeof(T).Name);
        }

        return obj;
    }
    
    public static T FindOrCreate<T>(Type[] components = null) where T : MonoBehaviour
    {
        var result = GameObject.FindObjectOfType<T>();

        if (result == null)
        {
            List<Type> componentsToAdd;
            if (components != null)
            {
                componentsToAdd = new List<Type>(components){typeof(T)};
            }
            else
            {
                componentsToAdd = new List<Type>(){typeof(T)};
            }
            
            var obj = GameObject.Instantiate( new GameObject(typeof(T).Name, componentsToAdd.ToArray()));
            
            result = obj.AddComponent<T>();

            Debug.LogError("Cant find: " + typeof(T).Name);
        }

        return result;
    }
}
