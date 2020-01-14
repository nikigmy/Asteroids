using UnityEngine;

public interface IObjectGenerator<T>
{
    T GenerateObject(Transform parent = null);

    T[] GenerateObjects(int count, Transform parent = null);
}