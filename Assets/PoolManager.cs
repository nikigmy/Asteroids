using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	[SerializeField] private Dictionary<string, IObjectGenerator> objectGenerators;
	private Dictionary<string, List<GameObject>> poolObjects;

	private void Awake()
	{
		objectGenerators = new Dictionary<string, IObjectGenerator>();
		poolObjects = new Dictionary<string, List<GameObject>>();
	}

	public void RegisterGenerator(string key, IObjectGenerator generator)
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

	public void PopulatePool(string key, int count)
	{
		if (objectGenerators.ContainsKey(key))
		{
			var objects = GenerateObjects(key, count, transform);

			if (!poolObjects.ContainsKey(key))
			{
				poolObjects.Add(key, objects);
			}
			else
			{
				poolObjects[key].AddRange(objects);
			}
		}
	}

	public void PopulatePoolToTarget(string key, int target)
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
				poolObjects.Add(key, new List<GameObject>());
				objectsToAdd = target;
			}

			if (objectsToAdd <= 0) return;
			poolObjects[key].AddRange(GenerateObjects(key, objectsToAdd, transform));
		}
	}

	public void ReturnObject(string key, GameObject poolObject)
	{
		poolObject.SetActive(false);
		poolObject.transform.parent = transform;
		
		if (poolObjects.ContainsKey(key))
		{
			poolObjects[key].Add(poolObject);
		}
		else
		{
			Debug.LogError("Object not from this pool manager");
			poolObjects.Add(key, new List<GameObject>() {poolObject});
		}
	}

	public GameObject RetrieveObject(string key)
	{
		GameObject resultObject = null;
		if (poolObjects.ContainsKey(key) && poolObjects[key].Count > 0)
		{
			resultObject = poolObjects[key][poolObjects[key].Count - 1];
		}
		else
		{
			if (objectGenerators.ContainsKey(key))
			{
				resultObject = objectGenerators[key].GenerateObject();
			}
			else
			{
				Debug.LogError(string.Format("Object generator for key: {0} does not exist", key));
			}
		}

		if (resultObject != null)
		{
			resultObject.transform.parent = null;
		}

		return resultObject;
	}

	public GameObject[] RetrieveObjects(string key, int count)
	{
		GameObject[] resultObjects = null;
		if (poolObjects.ContainsKey(key))
		{
			if (poolObjects[key].Count >= count)
			{
				resultObjects = poolObjects[key].Take(count).ToArray();
				poolObjects[key].RemoveRange(0, count);
			}
			else
			{
				resultObjects = new GameObject[poolObjects[key].Count];
				for (int i = 0; i < poolObjects[key].Count; i++)
				{
					resultObjects[i] = poolObjects[key][i];
					resultObjects[i].transform.parent = null;
				}

				resultObjects = resultObjects.Concat(GenerateObjects(key, count - poolObjects[key].Count))
					.ToArray();

				poolObjects[key].Clear();
			}
		}
		else
		{
			if (objectGenerators.ContainsKey(key))
			{
				resultObjects = GenerateObjects(key, count).ToArray();
			}
			else
			{
				Debug.LogError(string.Format("Object generator for key: {0} does not exist", key));
			}
		}

		return resultObjects;
	}

	private List<GameObject> GenerateObjects(string key, int count, Transform parent = null)
	{
		var result = new List<GameObject>(count);
		for (int i = 0; i < count; i++)
		{
			var generatedObject = objectGenerators[key].GenerateObject(parent);
			generatedObject.name = i.ToString();
			result.Add(generatedObject);;
			generatedObject.SetActive(false);
		}

		return result;
	}
}
