using System;
using UnityEngine;

namespace Utils
{
    public class Declarations
    {
        public enum ControlScheme
        {
            Desktop,
            Mobile
        }

        public enum EffectType
        {
            Speed,
            Shield,
            HomingAmmo
        }

        [Serializable]
        public class AsteroidSpeed
        {
            public int asteroidLevel;
            public float maxSpeed;
            public float minSpeed;
        }

        [Serializable]
        public class EffectDuration
        {
            public float duration;
            public EffectType effectType;
        }

        [Serializable]
        public class ControlSchemeInputPair
        {
            public ControlScheme ControlScheme;
            public GameObject InputControllerPrefab;
        }

        [Serializable]
        public class AudioData
        {
            public AudioClip[] Clips;
            public string Key;
        }
    }
}