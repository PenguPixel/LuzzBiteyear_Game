#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-22
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


using System.Collections;
using UnityEngine;

public enum SoundPlayMode
{
    RandomInterval,
    LoopedDuration
}
public class RandomSoundPlayer : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Dependencies")]
    [SerializeField] private AudioEventChannel audioChannel;
    [SerializeField] private SoundData soundData;

    [Header("Playmode Configuration")]
    [SerializeField] private SoundPlayMode playMode = SoundPlayMode.RandomInterval;
    [SerializeField] private bool autoStart = true;

    [Header("Interval/Cooldown Setup")]
    [SerializeField] private float minCooldown = 5f;
    [SerializeField] private float maxCooldown = 10f;

    [Header("Loop Duration Setup")]
    [SerializeField] private float minPlayDuration = 10f;
    [SerializeField] private float maxPlayDuration = 30f;
    #endregion


    #region Internal
    private Coroutine playbackRoutine;
    private bool isRunning;

    private void OnEnable()
    {
        if (autoStart) StartPlayer();
    }
    private void OnDisable()
    {
        StopPlayer();
    }
    #endregion


    #region Methods
    public void StartPlayer()
    {
        StopPlayer();
        isRunning = true;
        playbackRoutine = playMode switch
        {
            SoundPlayMode.RandomInterval => StartCoroutine(RandomIntervalRoutine()),
            SoundPlayMode.LoopedDuration => StartCoroutine(LoopedDurationRoutine()),
            _ => throw new System.NotImplementedException(),
        };
    }
    public void StopPlayer()
    {
        isRunning = false;
        if (playbackRoutine != null)
        {
            StopCoroutine(playbackRoutine);
            playbackRoutine = null;
        }
    }

    private IEnumerator RandomIntervalRoutine()
    {
        yield return null;
        while (isRunning)
        {
            float waitTime = Random.Range(minCooldown, maxCooldown);

            yield return new WaitForSeconds(waitTime);
            if (!isRunning) yield break;
            if (audioChannel != null && soundData != null)
            {
                audioChannel.RaiseSFX(soundData, transform.position);
            }
        }
    }
    private IEnumerator LoopedDurationRoutine()
    {
        yield return null;
        while (isRunning)
        {
            if (audioChannel != null && soundData != null)
            {
                audioChannel.RaiseSFX(soundData, transform.position);
            }

            float activeDuration = Random.Range(minPlayDuration, maxPlayDuration);
            yield return new WaitForSeconds(activeDuration);

            if (!isRunning) yield break;

            float cooldown = Random.Range(minCooldown, maxCooldown);
            yield return new WaitForSeconds(cooldown);
        }
    }
    #endregion
    [ContextMenu("Test Sound Trigger")]
private void TestSoundTrigger()
{
    if (audioChannel != null && soundData != null)
    {
        Debug.Log($"[Test] Manually triggering SFX from {gameObject.name}");
        audioChannel.RaiseSFX(soundData, transform.position);
    }
    else
    {
        Debug.LogWarning("[Test] Missing AudioChannel or SoundData reference!");
    }
}

[ContextMenu("Force Start Player")]
public void ForceStartPlayer()
{
    StartPlayer();
}
}