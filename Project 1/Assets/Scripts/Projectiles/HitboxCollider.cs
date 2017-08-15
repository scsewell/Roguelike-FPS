using UnityEngine;

public class HitboxCollider : MonoBehaviour
{
    [SerializeField]
    private Collider m_hitbox;
    [SerializeField]
    private float m_damageMultiplier = 1f;

    [SerializeField]
    private bool m_bleed = false;
    public bool Bleed
    {
        get { return m_bleed; }
    }

    private Health m_damageReciever;

    private void Awake()
    {
        m_damageReciever = GetComponentInParent<Health>();
    }

    public void Damage(float damage)
    {
        if (enabled)
        {
            m_damageReciever.ApplyDamage(damage * m_damageMultiplier);
        }
    }

    private void OnEnable()
    {
        m_hitbox.enabled = true;
    }

    private void OnDisable()
    {
        m_hitbox.enabled = false;
    }
}
