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


using UnityEngine;

[AddComponentMenu("Audio/Entity Audio")]
public class EntityAudio : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Dependencies")]
    [SerializeField] private AudioEventChannel audioChannel;
    [SerializeField] private CharacterAnimationBridge animationBridge;
    [SerializeField] private Health healthComponent;

    [Header("Movement/Actions")]
    [SerializeField] private SoundData footStepSound;
    [SerializeField] private SoundData jumpSound;

    [Header("Combat Offense")]
    [SerializeField] private SoundData meleeHitSound;
    [SerializeField] private SoundData shootSound;

    [Header("Combat Defense/rReactions")]
    [SerializeField] private SoundData takeDamageSound;
    [SerializeField] private SoundData deathSound;
    [SerializeField] private SoundData healSound;
    #endregion


    #region Internal
    private void OnEnable()
    {
        if (animationBridge != null)
        {
            animationBridge.OnFootstep += PlayFootstep;
            animationBridge.OnJumpImpulse += PlayJump;
            animationBridge.OnMeleeHitFrame += PlayMeleeHit;
            animationBridge.OnShootFrame += PlayShoot;
        }

        if (healthComponent != null)
        {
            healthComponent.OnDamaged += HandleDamageSound;
            healthComponent.OnHealed += HandleHealSound;
            healthComponent.OnDied += HandleDeathSound;
        }
    }

    private void OnDisable()
    {
        if (animationBridge != null)
        {
            animationBridge.OnFootstep -= PlayFootstep;
            animationBridge.OnJumpImpulse -= PlayJump;
            animationBridge.OnMeleeHitFrame -= PlayMeleeHit;
            animationBridge.OnShootFrame -= PlayShoot;
            animationBridge.OnHitReactionComplete -=PlayTakeDamage;
        }

        if (healthComponent != null)
        {
            healthComponent.OnDamaged -= HandleDamageSound;
            healthComponent.OnHealed -= HandleHealSound;
            healthComponent.OnDied -= HandleDeathSound;
        }
        
    }
    #endregion


    #region Methods
    private void PlaySound(SoundData data)
    {
        if (data == null || audioChannel == null) return;
        audioChannel.RaiseSFX(data, transform.position);
    }

    //With delegations
    private void HandleDeathSound(DamageContext context) => PlayDeath(); // Unity Events Naming rules are your reason to exist
    private void HandleDamageSound(DamageContext context) => PlayTakeDamage();
    private void HandleHealSound(int amount) => PlayHeal();
    
    // Without delegations
    private void PlayFootstep() => PlaySound(footStepSound);
    private void PlayJump() => PlaySound(jumpSound);
    private void PlayMeleeHit() => PlaySound(meleeHitSound);
    private void PlayShoot() => PlaySound(shootSound);
    private void PlayTakeDamage() => PlaySound(takeDamageSound);
    private void PlayDeath() => PlaySound(deathSound);
    private void PlayHeal() => PlaySound(healSound);
    // Expand here upon implementation of new sounds
    #endregion
}