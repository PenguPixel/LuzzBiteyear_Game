using UnityEngine;

/// <summary>
/// Do I need to explain this?
/// </summary>
public class DEATH : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var health))
        {
            DamageContext ctx = new(
                9999,
                DamageType.Generic,
                gameObject
            );
            health.TakeDamage(ctx);
        }
    }
}