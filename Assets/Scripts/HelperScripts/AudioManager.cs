using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Cable Scanning")]
    public AudioClip scanningSound;
    [Range(0f, 1f)] public float scanningVolume = 0.7f;

    public AudioClip vulnerabilityFoundSound;
    [Range(0f, 1f)] public float vulnerabilityVolume = 0.5f;

    public AudioClip vulnerabilityDestroyedSound;
    [Range(0f, 1f)] public float destroyedVolume = 0.5f;

    [Header("Device Scanning")]
    public AudioClip deviceScanningSound;
    [Range(0f, 1f)] public float deviceScanningVolume = 0.5f;

    private AudioSource loopingSource;
    private AudioSource oneShotSource;

    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        loopingSource = gameObject.AddComponent<AudioSource>();
        oneShotSource = gameObject.AddComponent<AudioSource>();

        loopingSource.spatialBlend = 0.5f;
        oneShotSource.spatialBlend = 0.5f;
    }

    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        if (clip == null) return;

        if (loop)
        {
            loopingSource.clip = clip;
            loopingSource.loop = true;
            loopingSource.volume = volume;
            loopingSource.pitch = pitch;
            loopingSource.Play();
        }
        else
        {
            oneShotSource.pitch = pitch;
            oneShotSource.PlayOneShot(clip, volume);
        }
    }

    public void UpdateLoopingSound(float volume, float pitch)
    {
        if (loopingSource.isPlaying)
        {
            loopingSource.volume = volume;
            loopingSource.pitch = pitch;
        }
    }

    public void StopLoopingSound()
    {
        if (loopingSource.isPlaying)
        {
            loopingSource.Stop();
        }
    }

    public bool IsLoopingSoundPlaying()
    {
        return loopingSource.isPlaying;
    }

    public AudioClip GetLoopingClip()
    {
        return loopingSource.clip;
    }

    public float GetLoopingVolume()
    {
        return loopingSource.volume;
    }

    public float GetLoopingPitch()
    {
        return loopingSource.pitch;
    }

    public void PlayCableScanningSound()
    {
        PlaySound(scanningSound, scanningVolume, 1f, true);
    }

    public void PlayVulnerabilityFoundSound()
    {
        PlaySound(vulnerabilityFoundSound, vulnerabilityVolume);
    }

    public void PlayVulnerabilityDestroyedSound()
    {
        PlaySound(vulnerabilityDestroyedSound, destroyedVolume);
    }
}
