using UnityEngine;

namespace Iterfaces
{
    /// <summary>
    /// Interface for object generators
    /// </summary>
    /// <typeparam name="T">Type of object to generate</typeparam>
    public interface IObjectGenerator<T>
    {
        /// <summary>
        /// Generate a single object
        /// </summary>
        /// <param name="parent">Parent of the object</param>
        /// <returns>The generated object</returns>
        T GenerateObject(Transform parent = null);

        /// <summary>
        /// Generated the specified amount of objects
        /// </summary>
        /// <param name="count">Count of objects to generate</param>
        /// <param name="parent">Parent of the objects</param>
        /// <returns>The generated objects</returns>
        T[] GenerateObjects(int count, Transform parent = null);
    }
}