using Game;
using Iterfaces;
using Managers;
using UnityEngine;

namespace ObjectGenerators
{
    /// <summary>
    /// Object generator for standard components
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericObjectGenerator<T> : IObjectGenerator<T> where T : Component
    {
        private readonly T templateObject;

        public GenericObjectGenerator()
        {
            var template = new GameObject(typeof(T).Name);
            template.hideFlags = HideFlags.HideInHierarchy;
            templateObject = template.AddComponent<T>();
        }

        /// <param name="prefabPath">Path of the object to generate</param>
        public GenericObjectGenerator(string prefabPath)
        {
            templateObject = Utils.Utils.LoadObject<T>(prefabPath);
        }

        public virtual T GenerateObject(Transform parent = null)
        {
            T obj;
            if (parent != null)
                obj = Object.Instantiate(templateObject, parent);
            else
                obj = Object.Instantiate(templateObject);

            var collisionReporter = obj.GetComponent<CollisionReporter>();
            if (collisionReporter != null)
                GameManager.Instance.LevelManager.CollisionManager.RegisterCollisionReporter(collisionReporter);

            return obj;
        }

        public virtual T[] GenerateObjects(int count, Transform parent = null)
        {
            var result = new T[count];
            for (var i = 0; i < count; i++)
            {
                T obj;
                if (parent != null)
                    obj = Object.Instantiate(templateObject, parent);
                else
                    obj = Object.Instantiate(templateObject);

                var collisionReporter = obj.GetComponent<CollisionReporter>();
                if (collisionReporter != null)
                    GameManager.Instance.LevelManager.CollisionManager.RegisterCollisionReporter(collisionReporter);

                result[i] = obj;
            }

            return result;
        }
    }
}