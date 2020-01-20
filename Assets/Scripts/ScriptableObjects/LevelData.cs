using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Data holder for a level
    /// </summary>
    [CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
    public class LevelData : ScriptableObject
    {
        public int AsteroidCount;
        public float[] FlyingSaucerTimings;
    }
}