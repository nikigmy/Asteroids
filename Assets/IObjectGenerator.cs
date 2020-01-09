using UnityEngine;

public interface IObjectGenerator
{
    GameObject GenerateObject(Transform parent = null);
}