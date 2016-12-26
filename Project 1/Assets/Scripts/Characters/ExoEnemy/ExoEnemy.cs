using UnityEngine;

public class ExoEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject m_exoskeleton;
    [SerializeField]
    private bool m_useExo = true;

    private ExoEnemyAnimation m_anim;
    private Health m_health;
    private CapsuleCollider m_collider;

    private void Start()
    {
        m_anim = GetComponent<ExoEnemyAnimation>();
        m_health = GetComponent<Health>();
        m_collider = GetComponent<CapsuleCollider>();

        if (!m_useExo)
        {
            Destroy(m_exoskeleton);
        }

        m_health.OnDie += OnDie;
    }

    private void OnDestroy()
    {
        m_health.OnDie -= OnDie;
    }

    private void OnDie()
    {
        m_anim.OnDie();
        m_collider.enabled = false;
    }

    private void Update()
    {
        if (m_health.IsAlive)
        {
            m_anim.Animate();
        }
	}
}
