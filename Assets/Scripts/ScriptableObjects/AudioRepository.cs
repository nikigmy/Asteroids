using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace ScriptableObjects
{
    /// <summary>
    /// Holder for audio dictionary
    /// </summary>
    [CreateAssetMenu(fileName = "AudioRepository", menuName = "ScriptableObjects/AudioRepository", order = 4)]
    public class AudioRepository : ScriptableObject
    {
        public List<Declarations.AudioData> AudioDatas;
    }
}