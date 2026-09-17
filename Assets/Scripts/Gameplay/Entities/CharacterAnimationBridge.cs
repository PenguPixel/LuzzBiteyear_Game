#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-17
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
/// This class handles animation contol of entitites. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Knows about the states an entity can represent and feeds the Animation Controller Master with the respective animations.
/// Coordination: Whenever a script fires an animation event on an entity, it sets the appropriate animation.
/// Deployment: Lives as component on an entity.
/// </summary>
#endregion


using UnityEngine;

[AddComponentMenu("Entities/CharacterAnimationBridge")]
public class CharacterAnimationBridge : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private Animator animator;
    
    #endregion


    #region Internal
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DJumpHash = Animator.StringToHash("DoubleJump");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int IsDeath = Animator.StringToHash("isDead");
    #endregion


    #region Methods
    public void UpdateLocomotion(float currentSpeed) => animator.SetFloat(SpeedHash, currentSpeed);
    public void TriggerAttack() => animator.SetTrigger(AttackHash);
    public void TriggerShoot() => animator.SetTrigger(ShootHash);
    public void TriggerHit() => animator.SetTrigger(HitHash);
    public void TriggerJump() => animator.SetTrigger(JumpHash);
    public void TriggerDoubleJump() => animator.SetTrigger(DJumpHash);
    public void SetGrounded(bool isGrounded) => animator.SetBool(IsGroundedHash, isGrounded);
    public void SetDeath(bool isDead) => animator.SetBool(IsDeath, isDead);
    #endregion
}