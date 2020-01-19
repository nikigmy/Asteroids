using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Managers
{
    public enum AudioGroup
    {
        Music,
        Sfx
    }
    
    /// <summary>
    /// This manager handles the audio
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private const string cMusicVolume = "MusicVolume";
        private const string cSfxVolume = "SfxVolume";
        
        /// <summary>
        /// Initialise the manager
        /// </summary>
        /// <param name="poolManager">Pool manager</param>
        public void Init(PoolManager poolManager)
        {
            mAudioClipDictionary = new Dictionary<string, List<AudioClip>>();
            mPoolManager = poolManager;
            mAudioMixer.SetFloat(cMusicVolume, PlayerPrefs.GetFloat(Constants.SaveKeys.musicVolume, 0));
            mAudioMixer.SetFloat(cSfxVolume, PlayerPrefs.GetFloat(Constants.SaveKeys.sfxVolume, 0));

            for (var i = 0; i < mRepository.AudioDatas.Count; i++)
            {
                var audioData = mRepository.AudioDatas[i];
                if (!mAudioClipDictionary.ContainsKey(audioData.Key))
                {
                    mAudioClipDictionary.Add(audioData.Key, new List<AudioClip>(audioData.Clips));
                }
                else
                {
                    mAudioClipDictionary[audioData.Key].AddRange(audioData.Clips);
                    Debug.LogError("Duplicating audio key: " + audioData.Key);
                }
            }
        }

        /// <summary>
        /// Play audio clip
        /// </summary>
        /// <param name="key">Key of the audio clip</param>
        /// <param name="group">Audio group to use</param>
        /// <param name="looping">Should the clip be looping</param>
        /// <param name="position">Position of the sound</param>
        /// <returns></returns>
        public AudioSource Play(string key, AudioGroup group, bool looping, Vector3 position)
        {
            if (mAudioClipDictionary.ContainsKey(key) && mAudioClipDictionary[key].Count > 0)
            {
                var clip = mAudioClipDictionary[key][Random.Range(0, mAudioClipDictionary[key].Count)];
                var audioSource = mPoolManager.RetrieveObject<AudioSource>();

                AudioMixerGroup mixerGroup;
                if (group == AudioGroup.Music)
                    mixerGroup = mMusicGroup;
                else
                    mixerGroup = mSfxGroup;

                audioSource.gameObject.SetActive(true);
                audioSource.transform.position = position;

                audioSource.Play(mixerGroup, clip, looping);

                return audioSource;
            }

            return null;
        }
        
        /// <summary>
        /// Sets the volume of an audio group
        /// </summary>
        /// <param name="group">Audio group</param>
        /// <param name="volume">Target volume</param>
        public void SetVolume(AudioGroup group, float volume)
        {
            if (group == AudioGroup.Music)
            {
                PlayerPrefs.SetFloat(Constants.SaveKeys.musicVolume, volume);
                mAudioMixer.SetFloat(cMusicVolume, volume);
            }
            else
            {
                PlayerPrefs.SetFloat(Constants.SaveKeys.sfxVolume, volume);
                mAudioMixer.SetFloat(cSfxVolume, volume);
            }
        }

        /// <summary>
        ///Ggets the volume of and audio group 
        /// </summary>
        /// <param name="group">Audio group</param>
        /// <returns>Volume</returns>
        public float GetVolume(AudioGroup group)
        {
            float result;

            if (group == AudioGroup.Music)
                mAudioMixer.GetFloat(cMusicVolume, out result);
            else
                mAudioMixer.GetFloat(cSfxVolume, out result);

            return result;
        }

        private Dictionary<string, List<AudioClip>> mAudioClipDictionary;

        private PoolManager mPoolManager;

        [SerializeField] private AudioRepository mRepository;
        
        [SerializeField] private AudioMixer mAudioMixer;
        
        [SerializeField] private AudioMixerGroup mMusicGroup;
        
        [SerializeField] private AudioMixerGroup mSfxGroup;
    }
}