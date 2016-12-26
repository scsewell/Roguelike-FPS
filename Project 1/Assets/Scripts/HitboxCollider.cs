using UnityEngine;

public class HitboxCollider : MonoBehaviour
{
    [SerializeField]
    private Collider m_hitbox;
    [SerializeField]
    private float m_damageMultiplier = 1f;

    private Health m_damageReciever;

    private void Start()
    {
        m_damageReciever = GetComponentInParent<Health>();
    }

    public void Damage(float damage)
    {
        m_damageReciever.ApplyDamage(damage * m_damageMultiplier);
    }
}
