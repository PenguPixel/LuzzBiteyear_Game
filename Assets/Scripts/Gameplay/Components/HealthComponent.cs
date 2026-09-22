#region Project Details
using System.Data.Common;
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-08
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
/// This class handles the health of the entitity it is attached to. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Holds Data to the health of an entity.
/// Coordination: Reacts to appropriate events by Damaging or healing the player.
/// Deployment: Attach a health Component to an entity.
/// </summary>
#endregion


using System;
using UnityEngine;
using UnityEngine.Events;
[AddComponentMenu("Resources/Health Resource")]
public class Health : MonoBehaviour, IDamageable, IHealable
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Health Data")]
    [SerializeField] private IntReference currentHealth;
    [SerializeField] private IntReference maxHealth;

    [Header("Global Events")]
    [SerializeField] private GameEvent onHealthChanged;
    [SerializeField] private GameEvent onDied;

    [Header("Local Events")]
    [SerializeField] private UnityEvent<int> onThisHealthChanged;
    [SerializeField] private UnityEvent onDiedLocal;
    [SerializeField] private CharacterAnimationBridge animationBridge;

    [Header("Audio")]
    [SerializeField] private AudioEventChannel audioChannel;
    [SerializeField] private SoundData hitSO;
    [SerializeField] private SoundData healSO;
    #endregion


    #region Internal
    public delegate void DeathHandler(DamageContext context);
    public event DeathHandler OnDied;
    public event Action<DamageContext> OnDamaged;
    public event Action<int> OnHealed;
    public int CurrentHealth => currentHealth.Value;
    public int MaxHealth => maxHealth.Value;


    private void Start()
    {
        if (currentHealth != null)
            onThisHealthChanged?.Invoke(currentHealth.Value);
    }
    private void OnEnable()
    {
        if (currentHealth != null && maxHealth != null)
        {
            currentHealth.Value = maxHealth.Value;
        }
        
        if (animationBridge != null)
            animationBridge.OnDeathComplete += HandleDeathAnimation;
    }
    private void OnDisable()
    {
        if (animationBridge != null)
            animationBridge.OnDeathComplete -= HandleDeathAnimation;
    }
    #endregion


    #region Methods
    public void TakeDamage(DamageContext ctx)
    {
        if (currentHealth.Value <= 0) return;
        currentHealth.Value -= ctx.Amount;

        OnDamaged?.Invoke(ctx);

        if (animationBridge != null)
            animationBridge.TriggerHit();

        if (onHealthChanged != null)
            onHealthChanged.Raise();
        onThisHealthChanged?.Invoke(currentHealth.Value);

        if (audioChannel != null && hitSO != null)
            audioChannel.RaiseSFX(hitSO, transform.position);

        if (currentHealth.Value <= 0)
        {
            Die(ctx);
        }
    }

    public void Heal(int amount, GameObject healSource = null)
    {
        if (currentHealth.Value >= maxHealth.Value) return;
        currentHealth.Value = Math.Clamp(currentHealth.Value + amount, 0, maxHealth.Value);

        OnHealed?.Invoke(amount);
        if (onHealthChanged != null)
            onHealthChanged.Raise();
        onThisHealthChanged?.Invoke(currentHealth.Value);
        if (audioChannel != null && healSO != null)
            audioChannel.RaiseSFX(healSO, transform.position);
    }
    public void Die(DamageContext ctx = default)
    {
        // Heals the Player, if the attack was a bite. Technically works for enemies too if the player dies, but since that leads to GameOver anyway it is neglible.
        if (ctx.Type == DamageType.Melee && ctx.Source != null)
        {
            if (ctx.Source.TryGetComponent<Health>(out var attackerHealth))
                attackerHealth.Heal(1);
        }

        onDiedLocal?.Invoke();
        OnDied?.Invoke(ctx);

        if (animationBridge != null)
            animationBridge.SetDeath(true);

    }
    private void HandleDeathAnimation()
    {
        if (onDied != null)
            onDied.Raise();
    }
    #endregion
}