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
/// This class handles the configuration of a sound at runtime. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: [Describe what this class does].
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: [Where it should live in the Scene, Project, Assets'].
/// </summary>
#endregion

using System.Collections;
using UnityEngine;


public class PooledAudioSource : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private AudioSource audioSource;
    #endregion


    #region Internal
    private PoolManager poolManager;
    private GameObject prefabReference;
    private Coroutine releaseRoutine;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    #endregion

    
    #region Methods
    /// <summary>
    /// Configures the AudioSource and schedules its return to the PoolManager upon comletion.
    /// </summary>
    public void Play(SoundData data, PoolManager pool, GameObject prefab)
    {
        poolManager = pool;
        prefabReference = prefab;

        AudioClip clip = data.GetRandomClip();
        if (clip == null)
        {
            Recycle();
            return;
        }

        audioSource.clip = clip;
        audioSource.volume = data.volume;
        audioSource.pitch = Random.Range(data.minPitch, data.maxPitch);
        audioSource.spatialBlend = data.spatialBlend;
        audioSource.minDistance = data.minDistance;
        audioSource.maxDistance = data.maxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        audioSource.Play();

        if (releaseRoutine != null) StopCoroutine(releaseRoutine);
        releaseRoutine = StartCoroutine(AutoReleaseRoutine(clip.length / Mathf.Abs(audioSource.pitch)));
    }
    private IEnumerator AutoReleaseRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Recycle();
    }
    private void Recycle()
    {
        releaseRoutine = null;
        if (poolManager != null && prefabReference != null)
        {
            poolManager.Release(prefabReference, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
}