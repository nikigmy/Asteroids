using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ShipData", menuName = "ScriptableObjects/ShipData", order = 3)]
    public class ShipData : ScriptableObject
    {
        public float Acceleration = 12.0f;
        public float AngularVelocity = 360.0f;
        public float Damping = 0.6f;
        public float FireRate = 0.2f;
    }
}