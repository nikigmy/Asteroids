using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObjectGenerator : IObjectGenerator
{
	private string prefabPath;
	GameObject templateObject;

	public BasicObjectGenerator(string prefabPath)
	{
		this.prefabPath = prefabPath;
		templateObject = Resources.Load(this.prefabPath) as GameObject;
	}

	public virtual GameObject GenerateObject(Transform parent = null)
	{
		if (parent != null)
		{
			return GameObject.Instantiate(templateObject, parent);
		}
		else
		{
			return GameObject.Instantiate(templateObject);
		}
	}
}
