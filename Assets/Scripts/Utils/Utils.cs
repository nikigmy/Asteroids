using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        /// <summary>
        /// Returns a random value
        /// </summary>
        /// <param name="min">Min</param>
        /// <param name="max">Max</param>
        /// <returns>Returns a random value between -max and -min or min and max</returns>
        public static float GenerateRandomValue(float min, float max)
        {
            var val = Random.Range(min, max);
            FlipDirectionOnRandom(ref val);

            return val;
        }

        /// <summary>
        /// Inverts the sigh of value on random
        /// </summary>
        /// <param name="value">Value</param>
        public static void FlipDirectionOnRandom(ref float value)
        {
            if (Random.value > 0.5f) value *= -1;
        }

        /// <summary>
        /// Loads a game object
        /// </summary>
        /// <param name="prefabPath">Prefab path of the object</param>
        /// <returns>Loaded object</returns>
        public static GameObject LoadObject(string prefabPath)
        {
            return Resources.Load(prefabPath) as GameObject;
        }

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <param name="prefabPath">Prefab path of the object</param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>Loaded object</returns>
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

        /// <summary>
        /// Tries to get the value associated to passed key otherwise returns the default value
        /// </summary>
        /// <param name="dict">Dictionary</param>
        /// <param name="key">Dictionary key</param>
        /// <param name="defaultValue">Default value</param>
        /// <typeparam name="TKey">Dictionary key type</typeparam>
        /// <typeparam name="TValue">Dictionary value type</typeparam>
        /// <returns>Either the associated value or the default value if not found</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key,
            TValue defaultValue)
        {
            TValue value;
            if (dict.TryGetValue(key, out value)) return value;

            return defaultValue;
        }


        /// <summary>
        /// Tries to get the value associated to passed key
        /// </summary>
        /// <param name="dict">Dictionary</param>
        /// <param name="key">Dictionary key</param>
        /// <typeparam name="TKey">Dictionary key type</typeparam>
        /// <typeparam name="TValue">Dictionary value type</typeparam>
        /// <returns>The value of the passed key</returns>
        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            dict.TryGetValue(key, out value);
            return value;
        }
        
        
        /// <summary>
        /// Gets the closest position to target taking into account the wrap around field
        /// </summary>
        /// <param name="from">Start point</param>
        /// <param name="to">Target position</param>
        /// <param name="fieldSize">Field size</param>
        /// <returns>Closest position to target</returns>
        public static Vector3 GetClosestPosition(Vector3 from, Vector3 to, Vector3 fieldSize)
        {
            var minDistance = float.MaxValue;
            var result = to;

            var positions = new[]
            {
                new Vector3(to.x, to.y),
                new Vector3(to.x + fieldSize.x, to.y, 0),
                new Vector3(to.x - fieldSize.x, to.y, 0),
                new Vector3(to.x, to.y + fieldSize.y, 0),
                new Vector3(to.x, to.y - fieldSize.y, 0)
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