using Game;
using Iterfaces;
using Managers;
using UnityEngine;

namespace ObjectGenerators
{
    /// <summary>
    /// Generator for game objects
    /// </summary>
    public sealed class ObjectGenerator : IObjectGenerator<GameObject>
    {
        private readonly GameObject templateObject;

        /// <param name="prefabPath">Path of the object to generate</param>
        public ObjectGenerator(string prefabPath)
        {
            templateObject = Utils.Utils.LoadObject(prefabPath);
        }

        public GameObject GenerateObject(Transform parent = null)
        {
            GameObject obj;
            if (parent != null)
                obj = Object.Instantiate(templateObject, parent);
            else
                obj = Object.Instantiate(templateObject);

            var collisionReporter = obj.GetComponent<CollisionReporter>();
            if (collisionReporter != null)
                GameManager.Instance.LevelManager.CollisionManager.RegisterCollisionReporter(collisionReporter);

            return obj;
        }

        public GameObject[] GenerateObjects(int count, Transform parent = null)
        {
            var result = new GameObject[count];
            for (var i = 0; i < count; i++)
            {
                GameObject obj;
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