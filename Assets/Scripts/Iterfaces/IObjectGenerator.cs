using UnityEngine;

namespace Iterfaces
{
    /// <summary>
    /// Interface for object generators
    /// </summary>
    /// <typeparam name="T">Type of object to generate</typeparam>
    public interface IObjectGenerator<T>
    {
        T GenerateObject(Transform parent = null);

        T[] GenerateObjects(int count, Transform parent = null);
    }
}