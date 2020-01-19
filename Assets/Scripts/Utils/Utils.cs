using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        public static float GenerateRandomValue(float min, float max)
        {
            var val = Random.Range(min, max);
            FlipDirectiononRandom(ref val);

            return val;
        }

        private static void FlipDirectiononRandom(ref float value)
        {
            if (Random.value > 0.5f) value *= -1;
        }

        public static GameObject LoadObject(string prefabPath)
        {
            return Resources.Load(prefabPath) as GameObject;
        }

        public static T LoadObject<T>(string prefabPath) where T : Component
        {
            T result = null;
            var loadedObject = Resources.Load(prefabPath);
            if (loadedObject != null)
            {
                if (typeof(T) != typeof(GameObject))
                    result = (loadedObject as GameObject).GetComponent<T>();
                else
                    result = loadedObject as T;
            }
            else
            {
                Debug.LogError("Cant load prefab: " + prefabPath);
            }

            return result;
        }

        /// Tries to get the value associated to passed key otherwise returns
        /// the default value
        /// 
        /// @param key
        /// Key to retrieve
        /// @param defaultValue
        /// The default value in case the get fails
        /// 
        /// @return Either the associated value or the default value if not found
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key,
            TValue defaultValue)
        {
            TValue value;
            if (dict.TryGetValue(key, out value)) return value;

            return defaultValue;
        }


        /// Tries to get the value associated to passed key
        /// 
        /// @param key
        /// Key to retrieve
        /// 
        /// @return The value of the passed key
        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            dict.TryGetValue(key, out value);
            return value;
        }

        public static float NormaliseAngle(float angle)
        {
            while (angle < -180) angle += 360;

            while (angle > 180) angle -= 360;

            return angle;
        }

        public static Vector3 GetClosestPosition(Vector3 from, Vector3 to)
        {
            var minDistance = float.MaxValue;
            var result = to;

            var positions = new[]
            {
                new Vector3(to.x, to.y),
                new Vector3(to.x + GameManager.instance.LevelManager.FieldSize.x, to.y, 0),
                new Vector3(to.x - GameManager.instance.LevelManager.FieldSize.x, to.y, 0),
                new Vector3(to.x, to.y + GameManager.instance.LevelManager.FieldSize.y, 0),
                new Vector3(to.x, to.y - GameManager.instance.LevelManager.FieldSize.y, 0)
            };

            for (var i = 0; i < positions.Length; i++)
            {
                var dist = Vector3.Distance(from, positions[i]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    result = positions[i];
                }
            }

            return result;
        }
    }
}