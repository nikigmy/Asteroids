using System;
using System.Collections.Generic;
using System.Linq;
using Iterfaces;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Handles creation and distribution of objects
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        private void Awake()
        {
            mObjectGenerators = new Dictionary<string, object>();
            mPoolObjectsDictionary = new Dictionary<string, List<object>>();
            mActiveObjectsDictionary = new Dictionary<string, List<object>>();
        }

        /// <summary>
        /// Register generator for the specified pool key
        /// </summary>
        /// <param name="key">Pool key</param>
        /// <param name="generator">Generator to register</param>
        /// <typeparam name="T">Type of generator</typeparam>
        public void RegisterGenerator<T>(string key, IObjectGenerator<T> generator)
        {
            if (!mObjectGenerators.ContainsKey(key))
            {
                mObjectGenerators.Add(key, generator);
                mPoolObjectsDictionary.Add(key, new List<object>());
                mActiveObjectsDictionary.Add(key, new List<object>());
            }
            else
                Debug.LogError(string.Format("Generator for key {0} already registered", key));
        }

        /// <summary>
        /// Register generator for pool key T
        /// </summary>
        /// <param name="generator">Generator to register</param>
        /// <typeparam name="T">Type of generator</typeparam>
        public void RegisterGenerator<T>(IObjectGenerator<T> generator)
        {
            var type = typeof(T).FullName;
            if (!mObjectGenerators.ContainsKey(type))
            {
                mObjectGenerators.Add(type, generator);
                mPoolObjectsDictionary.Add(type, new List<object>());
                mActiveObjectsDictionary.Add(type, new List<object>());
            }
            else
                Debug.LogError(string.Format("Generator for key {0} already registered", type));
        }

        /// <summary>
        /// Populate pool of pool key T with the required objects
        /// </summary>
        /// <param name="count">Objects to add</param>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <returns>Generated objects</returns>
        public T[] PopulatePool<T>(int count)
        {
            var type = typeof(T).FullName;
            return PopulatePool<T>(type, count);
        }

        /// <summary>
        /// Populate pool of specified key with the required objects
        /// </summary>
        /// <param name="key">Pool key</param>
        /// <param name="count">Objects to add</param>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <returns>Generated objects</returns>
        public T[] PopulatePool<T>(string key, int count)
        {
            T[] generatedObjects = null;
            IEnumerable<object> objects = null;
            if (mObjectGenerators.ContainsKey(key))
            {
                generatedObjects = GenerateObjects<T>(key, count, transform);
                objects = generatedObjects.Select(x => (object) x);
                if (!mPoolObjectsDictionary.ContainsKey(key))
                    mPoolObjectsDictionary.Add(key, new List<object>(objects));
                else
                    mPoolObjectsDictionary[key].AddRange(objects);
            }
            else
            {
                Debug.LogError("No object generator for pool object: " + key);
            }

            return generatedObjects;
        }

        /// <summary>
        /// Populate pool of pool key T to target
        /// </summary>
        /// <param name="target">Target count</param>
        /// <typeparam name="T">Type of pool objects</typeparam>
        public void PopulatePoolToTarget<T>(int target)
        {
            var type = typeof(T).FullName;
            PopulatePoolToTarget<T>(type, target);
        }

        /// <summary>
        /// Populate pool of specified key to target
        /// </summary>
        /// <param name="key">Pool key</param>
        /// <param name="target">Target count</param>
        /// <typeparam name="T">Type of pool objects</typeparam>
        public void PopulatePoolToTarget<T>(string key, int target)
        {
            if (mObjectGenerators.ContainsKey(key) && target > 0)
            {
                int objectsToAdd;
                if (mPoolObjectsDictionary.ContainsKey(key))
                {
                    objectsToAdd = target - mPoolObjectsDictionary[key].Count;
                }
                else
                {
                    mPoolObjectsDictionary.Add(key, new List<object>(target));
                    objectsToAdd = target;
                }

                if (objectsToAdd <= 0) return;
                mPoolObjectsDictionary[key]
                    .AddRange(GenerateObjects<T>(key, objectsToAdd, transform).Select(x => (object) x).ToList());
            }
        }

        /// <summary>
        /// Returns an object to the pool with pool key T
        /// </summary>
        /// <param name="poolObject">Object to return</param>
        /// <typeparam name="T">Type of object</typeparam>
        public void ReturnObject<T>(T poolObject)
        {
            var key = typeof(T).FullName;
            ReturnObject(key, poolObject);
        }

        /// <summary>
        /// Returns an object to the pool with the specified key
        /// </summary>
        /// <param name="key">Pool key</param>
        /// <param name="obj">Object to return</param>
        public void ReturnObject(string key, object obj)
        {
            if (obj != null)
            {
                ReturnObject(obj.GetType().IsSubclassOf(typeof(Component)), obj);

                if (mPoolObjectsDictionary.ContainsKey(key))
                {
                    mPoolObjectsDictionary[key].Add(obj);
                }
                else
                {
                    if (mObjectGenerators.ContainsKey(key))
                        mPoolObjectsDictionary.Add(key, new List<object> {obj});
                    else
                        Debug.LogError("Object not from this pool manager");
                }

                if (mActiveObjectsDictionary.ContainsKey(key)) mActiveObjectsDictionary[key].Remove(obj);
            }
        }

        /// <summary>
        /// Returns an object to the pool with pool key T
        /// </summary>
        /// <param name="objects">Objects to return</param>
        /// <typeparam name="T">Type of objects</typeparam>
        public void ReturnObjects<T>(object[] objects)
        {
            var key = typeof(T).FullName;
            ReturnObjects(key, objects);
        }

        /// <summary>
        /// Returns objects to the pool with the specified key
        /// </summary>
        /// <param name="key">Pool key</param>
        /// <param name="objects">Objects to return</param>
        public void ReturnObjects(string key, object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                var hasActiveObjects = mActiveObjectsDictionary.ContainsKey(key);
                var isComponent = objects[0].GetType().IsSubclassOf(typeof(Component));
                for (var i = 0; i < objects.Length; i++)
                {
                    var poolObject = objects[i];
                    ReturnObject(isComponent, poolObject);

                    if (hasActiveObjects) mActiveObjectsDictionary[key].Remove(poolObject);
                }

                if (mPoolObjectsDictionary.ContainsKey(key))
                    mPoolObjectsDictionary[key].AddRange(objects);
                else if (mObjectGenerators.ContainsKey(key))
                    mPoolObjectsDictionary.Add(key, new List<object>(objects));
                else
                    Debug.LogError("Object not from this pool manager");
            }
        }

        /// <summary>
        /// Retrieves an objects with pool key T from the pool
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>An objects with pool key T from the pool</returns>
        public T RetrieveObject<T>()
        {
            var type = typeof(T).FullName;
            return RetrieveObject<T>(type);
        }

        /// <summary>
        /// Retrieves an objects with the specified key from the pool
        /// </summary>
        /// <param name="key">Pool key</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Required number of objects with the specified key from the pool</returns>
        public T RetrieveObject<T>(string key)
        {
            object result = null;
            T resultObject = default;
            if (mPoolObjectsDictionary.ContainsKey(key) && mPoolObjectsDictionary[key].Count > 0)
            {
                result = mPoolObjectsDictionary[key][mPoolObjectsDictionary[key].Count - 1];
                mPoolObjectsDictionary[key].Remove(result);
            }
            else
            {
                if (mObjectGenerators.ContainsKey(key))
                    result = (mObjectGenerators[key] as IObjectGenerator<T>).GenerateObject();
                else
                    Debug.LogError(string.Format("Object generator for key: {0} does not exist", key));
            }

            if (result != null)
            {
                ReleaseObject(typeof(T).IsSubclassOf(typeof(Component)), result);
                
                resultObject = (T) result;
            }

            if (mActiveObjectsDictionary.ContainsKey(key))
                mActiveObjectsDictionary[key].Add(resultObject);
            else
                mActiveObjectsDictionary.Add(key, new List<object> {resultObject});

            return resultObject;
        }

        /// <summary>
        /// Retrieves the required number of objects with pool key T from the pool
        /// </summary>
        /// <param name="count">Count of objects</param>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <returns>Required number of objects with pool key T from the pool</returns>
        public T[] RetrieveObjects<T>(int count)
        {
            var type = typeof(T).FullName;
            return RetrieveObjects<T>(type, count);
        }

        /// <summary>
        /// Retrieves the required number of objects with the specified key from the pool
        /// </summary>
        /// <param name="key">Pool key</param>
        /// <param name="count">Count of objects</param>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <returns>Required number of objects with the specified key from the pool</returns>
        public T[] RetrieveObjects<T>(string key, int count)
        {
            var isComponent = typeof(T).IsSubclassOf(typeof(Component));
            T[] resultObjects = null;
            if (mPoolObjectsDictionary.ContainsKey(key))
            {
                var objsToTake = Math.Min(count, mPoolObjectsDictionary[key].Count);
                resultObjects = new T[count];
                if (objsToTake > 0)
                {
                    for (var i = 0; i < objsToTake; i++)
                    {
                        var result = mPoolObjectsDictionary[key][i];
                        
                        ReleaseObject(isComponent, result);

                        resultObjects[i] = (T) result;
                    }

                    mPoolObjectsDictionary[key].RemoveRange(0, objsToTake);
                }

                if (objsToTake < count)
                {
                    var generatedObjects = GenerateObjects<T>(key, count - objsToTake);
                    for (var i = 0; i < count - objsToTake; i++) resultObjects[objsToTake + i] = generatedObjects[i];
                }
            }
            else
            {
                if (mObjectGenerators.ContainsKey(key))
                    resultObjects = GenerateObjects<T>(key, count);
                else
                    Debug.LogError(string.Format("Object generator for key: {0} does not exist", key));
            }

            var res = resultObjects.Select(x => (object) x);
            if (mActiveObjectsDictionary.ContainsKey(key))
                mActiveObjectsDictionary[key].AddRange(res);
            else
                mActiveObjectsDictionary.Add(key, new List<object>(res));
            return resultObjects;
        }
        
        /// <summary>
        /// Gets active objects of type T with pool key T
        /// </summary>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <returns>Active objects of type T with pool key T</returns>
        public List<T> GetActiveObjects<T>()
        {
            var type = typeof(T).FullName;
            return GetActiveObjects<T>(type);
        }
        
        /// <summary>
        /// Gets active objects of type T with the specified key
        /// </summary>
        /// <param name="key">Pool key</param>
        /// <typeparam name="T">Type of pool</typeparam>
        /// <returns>Active object of type T with the specified key</returns>
        public List<T> GetActiveObjects<T>(string key)
        {
            return mActiveObjectsDictionary[key].Select(x => (T) x).ToList();
        }
        
        /// <summary>
        /// Returns all objects back to the pool
        /// </summary>
        /// <param name="ignoredKeys">Pool keys to ignore</param>
        public void ReturnAllObjects(string[] ignoredKeys)
        {
            foreach (var activeObjectPool in mActiveObjectsDictionary.Where(x => !ignoredKeys.Contains(x.Key)))
            {
                if (activeObjectPool.Value.Count == 0) continue;
                ReturnObjects(activeObjectPool.Key, activeObjectPool.Value.ToArray());
                mActiveObjectsDictionary[activeObjectPool.Key].Clear();
            }
        }
        
        private T[] GenerateObjects<T>(string key, int count, Transform parent = null)
        {
            var generatedObjects = (mObjectGenerators[key] as IObjectGenerator<T>).GenerateObjects(count, parent);

            var isComponent = typeof(T).IsSubclassOf(typeof(Component));
            for (var i = 0; i < count; i++)
                DeactivateObject(isComponent, generatedObjects[i]);

            return generatedObjects;
        }

        //These methods are used so both MonoBehaviors and GameObjects with no scripts attached can be passed to the pool
        #region Activation And Deactivation Of Objects
        private void ReturnObject(bool isComponent, object poolObject)
        {
            if (isComponent)
            {
                ((Component) poolObject).transform.SetParent(transform);
            }
            else
            {
                ((GameObject) poolObject).transform.SetParent(transform);
            }
            DeactivateObject(isComponent, poolObject);
        }

        private void ReleaseObject(bool isComponent, object poolObject)
        {
            if (isComponent)
            {
                ((Component) poolObject).transform.SetParent(null);
            }
            else
            {
                ((GameObject) poolObject).transform.SetParent(null);
            }
        }

        private void  DeactivateObject(bool isComponent, object poolObject)
        {
            if (isComponent)
            {
                ((Component) poolObject).gameObject.SetActive(false);
            }
            else
            {
                ((GameObject) poolObject).SetActive(false);
            }
        }
        #endregion
        
        private Dictionary<string, List<object>> mActiveObjectsDictionary;
        private Dictionary<string, List<object>> mPoolObjectsDictionary;
        private Dictionary<string, object> mObjectGenerators;
    }
}