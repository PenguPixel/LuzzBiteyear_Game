#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-18
*/
#endregion


#region Basic Instruction
/*
Structure the class into regions as appropriate for their use case.
The regions should separate what is viewed or used in an inspector, class intern relevant fields, Public Getters if necessary, 
Use top comments above methods to describe them and explain their parameters.
TODO comments above a method or codeblock
Use side comments in line to describe lines that obfuscate their function as explanation
*/
#endregion


#region Development remarks
/// <remarks>
/// <para>
/// This class handles [Core Responsibility]. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: [Describe what this class does].
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: [Where it should live in the Scene, Project, Assets'].
/// </summary>
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[AddComponentMenu("Audio/AudioManager")]
public class AudioManager : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Dependencies")]
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private AudioEventChannel audioChannel;
    [SerializeField] private AudioMixer mainMixer;

    [Header("SFX Pooling")]
    [SerializeField] private GameObject sfxSourcePrefab;
    [SerializeField] private int prewarmCount = 10;

    [Header("Music Setup")]
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    #endregion


    #region Internal
    private Func<Vector3, Quaternion, GameObject> sfxSpawnDelegate;
    private AudioSource activeMusicSource;
    private Coroutine musicFadeRoutine;

    private void Awake()
    {
        if (musicSourceA != null) musicSourceA.playOnAwake = false;
        if (musicSourceB != null) musicSourceB.playOnAwake = false;
    }

    private void Start()
    {
        if (poolManager != null && sfxSourcePrefab != null)
        {
            poolManager.Prewarm(sfxSourcePrefab, prewarmCount);
            sfxSpawnDelegate = poolManager.GetSpawnDelegate(sfxSourcePrefab);
        }
    }

    private void OnEnable()
    {
        if (audioChannel != null)
        {
            audioChannel.OnMusicRequested += PlayMusic;
            audioChannel.OnSFXRequested += PlaySFX;
        }
    }

    private void OnDisable()
    {
        if (audioChannel != null)
        {
            audioChannel.OnSFXRequested -= PlaySFX;
            audioChannel.OnMusicRequested -= PlayMusic;
        }
    }
    #endregion


    #region Volume Settings Logic
    public void SetMasterVolume(float linearVolume)
    {
        float db = linearVolume > 0.0001f ? Mathf.Log10(linearVolume) * 20f : -80f;
        mainMixer.SetFloat("MasterVolume", db);
    }

    public void SetMusicVolume(float linearVolume)
    {
        float db = linearVolume > 0.0001f ? Mathf.Log10(linearVolume) * 20 : -80;
        mainMixer.SetFloat("MusicVolume", db);
    }

    public void SetSFXVolume(float linearVolume)
    {
        float db = linearVolume > 0.0001f ? Mathf.Log10(linearVolume) * 20 : -80;
        mainMixer.SetFloat("SFXVolume", db);
    }
    #endregion


    #region AudioExecution Logic
    private void PlaySFX(SoundData data, Vector3 position)
    {
        if (data == null || sfxSpawnDelegate == null) return;

        GameObject sourceObj = sfxSpawnDelegate.Invoke(position, Quaternion.identity);
        if (sourceObj != null && sourceObj.TryGetComponent<PooledAudioSource>(out var pooledSource))
        {
            pooledSource.Play(data, poolManager, sfxSourcePrefab);
        }
    }

    private void PlayMusic(SoundData data, float fadeDuration)
    {
        if (data == null) return;
        AudioClip clip = data.GetRandomClip();
        if (clip == null) return;

        if (musicFadeRoutine != null) StopCoroutine(musicFadeRoutine);

        AudioSource nextSource = (activeMusicSource == musicSourceA) ? musicSourceB : musicSourceA;
        nextSource.clip = clip;
        nextSource.loop = true;
        nextSource.spatialBlend = 0f;
        nextSource.Play();

        musicFadeRoutine = StartCoroutine(CrossFadeMusic(nextSource, data.volume, fadeDuration));
    }

    private IEnumerator CrossFadeMusic(AudioSource newSource, float targetVolume, float duration)
    {
        float timer = 0f;
        AudioSource oldSource = activeMusicSource;
        activeMusicSource = newSource;

        float startOldVolume = oldSource != null ? oldSource.volume : 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            if (oldSource != null) oldSource.volume = Mathf.Lerp(startOldVolume, 0f, t);
            newSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        if (oldSource != null)
        {
            oldSource.Stop();
            oldSource.volume = startOldVolume;
        }
    }
    #endregion
}