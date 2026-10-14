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
/// This class handles the passiing of information between Entitites and AudioManager. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Acts as the Event BUS for Audio.
/// Coordination: .
/// Deployment: Lives as an asset. Wire this to AudioManager, EntityAudios or other similar components
/// </summary>
#endregion


using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Audio/Audio Event Channel")]
public class AudioEventChannel : ScriptableObject
{
    public UnityAction<SoundData, Vector3> OnSFXRequested;
    public UnityAction<SoundData, float> OnMusicRequested;

    public void RaiseSFX(SoundData data, Vector3 position)
    {
        OnSFXRequested?.Invoke(data, position);
    }
    public void RaiseMusic(SoundData data, float fadeDuration = 1f)
    {
        OnMusicRequested?.Invoke(data, fadeDuration);
    }
}