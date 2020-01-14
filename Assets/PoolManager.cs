using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

public class PoolManager : MonoBehaviour
{
	[SerializeField] private Dictionary<string, object> objectGenerators;
	private Dictionary<string, List<object>> poolObjects;

	private void Awake()
	{
		objectGenerators = new Dictionary<string, object>();
		poolObjects = new Dictionary<string, List<object>>();
	}
	
	public void RegisterGenerator<T>(string key, IObjectGenerator<T> generator)
	{
		if (!objectGenerators.ContainsKey(key))
		{
			objectGenerators.Add(key, generator);
		}
		else
		{
			Debug.LogError(string.Format("Generator for key {0} already registered", key));
		}
	}

	public void RegisterGenerator<T> (IObjectGenerator<T> generator)
	{
		var type = typeof(T).FullName;
		if (!objectGenerators.ContainsKey(type))
		{
			objectGenerators.Add(type, generator);
		}
		else
		{
			Debug.LogError(string.Format("Generator for key {0} already registered", type));
		}
	}

	public T[] PopulatePool<T>(int count)
	{
		var type = typeof(T).FullName;
		return PopulatePool<T>(type, count);
	}
	
	public T[] PopulatePool<T>(string key, int count)
	{
		T[] generatedObjects = null;
		IEnumerable<object> objects = null;
		if (objectGenerators.ContainsKey(key))
		{
			generatedObjects = GenerateObjects<T>(key, count, transform);
			objects = generatedObjects.Select(x => (object) x);
			if (!poolObjects.ContainsKey(key))
			{
				poolObjects.Add(key, new List<object>(objects){});
			}
			else
			{
				poolObjects[key].AddRange(objects);
			}
		}
		else
		{
			Debug.LogError("No object generator for pool object: " + key);
		}

		return generatedObjects;
	}

	public void PopulatePoolToTarget<T>(int target)
	{
		var type = typeof(T).FullName;
		PopulatePoolToTarget<T>(type, target);
	}
	
	public void PopulatePoolToTarget<T>(string key, int target)
	{
		if (objectGenerators.ContainsKey(key))
		{
			int objectsToAdd;
			if (poolObjects.ContainsKey(key))
			{
				objectsToAdd = target - poolObjects[key].Count;
			}
			else
			{
				poolObjects.Add(key, new List<object>(target));
				objectsToAdd = target;
			}

			if (objectsToAdd <= 0) return;
			poolObjects[key].AddRange(GenerateObjects<T>(key, objectsToAdd, transform).Select(x => (object)x).ToList());
		}
	}

	public void ReturnObject<T>(T poolObject) 
	{
		var key = typeof(T).FullName;
		ReturnObject(key, poolObject);
	}
	
	public void ReturnObject(string key, object poolObject) 
	{
		if (poolObject.GetType().IsSubclassOf(typeof(MonoBehaviour)))
		{
			((MonoBehaviour) poolObject).gameObject.SetActive(false);
			((MonoBehaviour) poolObject).transform.parent = transform;
		}
		else
		{
			((GameObject) poolObject).SetActive(false);
			((GameObject) poolObject).transform.parent = transform;
		}
		
		if (poolObjects.ContainsKey(key))
		{
			poolObjects[key].Add(poolObject);
		}
		else
		{
			Debug.LogError("Object not from this pool manager");
			poolObjects.Add(key, new List<object>() {poolObject});
		}
	}

	public T RetrieveObject<T>()
	{
		var type = typeof(T).FullName;
		return RetrieveObject<T>(type);
	}
	
	public T RetrieveObject<T>(string key)
	{
		object result = null;
		T resultObject = default;
		if (poolObjects.ContainsKey(key) && poolObjects[key].Count > 0)
		{
			result = poolObjects[key][poolObjects[key].Count - 1];
			poolObjects[key].Remove(result);
		}
		else
		{
			if (objectGenerators.ContainsKey(key))
			{
				result = (objectGenerators[key] as IObjectGenerator<T>).GenerateObject();
			}
			else
			{
				Debug.LogError(string.Format("Object generator for key: {0} does not exist", key));
			}
		}

		if (result != null)
		{
			if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
			{
				((MonoBehaviour) result).transform.parent = null;
			}
			else
			{
				((GameObject) result).transform.parent = null;
			}

			resultObject = (T) result;
		}

		return resultObject;
	}

	public T[] RetrieveObjects<T>(int count)
	{
		var type = typeof(T).FullName;
		return RetrieveObjects<T>(type, count);
	}
	
	public T[] RetrieveObjects<T>(string key, int count)
	{
		bool isMonobehaviour = typeof(T).IsSubclassOf(typeof(MonoBehaviour));
		T[] resultObjects = null;
		if (poolObjects.ContainsKey(key))
		{
			var objsToTake = Math.Min(count, poolObjects[key].Count);
			resultObjects = new T[count];
			if (objsToTake > 0)
			{
				for (int i = 0; i < objsToTake; i++)
				{
					var result = poolObjects[key][i];
					if (isMonobehaviour)
					{
						((MonoBehaviour) result).transform.parent = null;
					}
					else
					{
						((GameObject) result).transform.parent = null;
					}

					resultObjects[i] = (T) result;
				}

				poolObjects[key].RemoveRange(0, objsToTake);
			}

			if (objsToTake < count)
			{
				var generatedObjects = GenerateObjects<T>(key, count - objsToTake);
				for (int i = 0; i < count - objsToTake; i++)
				{
					resultObjects[objsToTake + i] = generatedObjects[i];
				}
			}
		}
		else
		{
			if (objectGenerators.ContainsKey(key))
			{
				resultObjects = GenerateObjects<T>(key ,count).ToArray();
			}
			else
			{
				Debug.LogError(string.Format("Object generator for key: {0} does not exist", key));
			}
		}
		

		return resultObjects;
	}

	private T[] GenerateObjects<T>(string key, int count, Transform parent = null)
	{
		var generatedObjects = (objectGenerators[key] as IObjectGenerator<T>).GenerateObjects(count,parent);

		bool isMonobehaviour = typeof(T).IsSubclassOf(typeof(MonoBehaviour));
		for (int i = 0; i < count; i++)
		{
			if (isMonobehaviour)
			{
				((MonoBehaviour) (object)generatedObjects[i]).gameObject.SetActive(false);
			}
			else
			{
				((GameObject) (object)generatedObjects[i]).SetActive(false);
			}
		}
		
		return generatedObjects;
	}
}
