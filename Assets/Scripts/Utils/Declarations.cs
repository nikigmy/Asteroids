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
        public class AsteroidLevel
        {
            public int asteroidLevel;
            public float minSpeed;
            public float maxSpeed;
            public float scale;
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
            public ControlScheme controlScheme;
            public GameObject inputControllerPrefab;
        }

        [Serializable]
        public class AudioData
        {
            public string key;
            public AudioClip[] clips;
        }
    }
}