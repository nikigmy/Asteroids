using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
    public class LevelData : ScriptableObject
    {
        public int AsteroidCount;
        public float[] FlyingSaucerTimings;
    }
}