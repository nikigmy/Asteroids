using Managers;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(UnityEngine.AudioSource))]
public class AudioSource : MonoBehaviour
{
    private void Awake()
    {
        mSource = GetComponent<UnityEngine.AudioSource>();
    }
    
    private void OnDisable()
    {
        CancelInvoke("ReturnObjectToPool");
    }

    /// <summary>
    /// Play an audio clip on this audio source
    /// </summary>
    /// <param name="group">Audio group on which the clip should be played</param>
    /// <param name="clip">Audio clip to be played</param>
    /// <param name="looping">Whether the clip should be looping</param>
    public void Play(AudioMixerGroup group, AudioClip clip, bool looping)
    {
        mSource.outputAudioMixerGroup = group;
        mSource.clip = clip;
        mSource.loop = looping;
        mSource.Play();
        if (!looping) Invoke("ReturnObjectToPool", clip.length);
    }
    
    /// <summary>
    /// Stop playing audio
    /// </summary>
    public void Stop()
    {
        mSource.Stop();
    }

    private void ReturnObjectToPool()
    {
        GameManager.Instance.PoolManager.ReturnObject(this);
    }
    
    private UnityEngine.AudioSource mSource;
}