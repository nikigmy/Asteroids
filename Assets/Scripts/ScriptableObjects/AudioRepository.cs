using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "AudioRepository", menuName = "ScriptableObjects/AudioRepository", order = 4)]
    public class AudioRepository : ScriptableObject
    {
        public List<Declarations.AudioData> AudioDatas;
    }
}