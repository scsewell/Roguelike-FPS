using UnityEngine;

public class ExoEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject m_exoskeleton;
    [SerializeField]
    private bool m_useExo = true;

    private ExoEnemyAI m_ai;
    private ExoEnemyAnimation m_anim;
    private CharacterMovement m_movement;
    private Health m_health;
    private Interactable m_interact;
    private CharacterController m_collider;
    
    private MoveInputs m_lastInputs;

    private void Start()
    {
        m_ai = GetComponent<ExoEnemyAI>();
        m_anim = GetComponent<ExoEnemyAnimation>();
        m_movement = GetComponent<CharacterMovement>();
        m_health = GetComponent<Health>();
        m_interact = GetComponent<Interactable>();
        m_collider = GetComponent<CharacterController>();

        if (!m_useExo)
        {
            Destroy(m_exoskeleton);
        }

        m_health.OnDie += OnDie;
        m_interact.InteractStart += m_anim.GrabBody;
        m_interact.InteractEnd += m_anim.ReleaseBody;

        m_interact.enabled = false;
    }

    private void OnDestroy()
    {
        m_health.OnDie -= OnDie;
        m_interact.InteractStart -= m_anim.GrabBody;
        m_interact.InteractEnd -= m_anim.ReleaseBody;
    }

    private void OnDie()
    {
        m_collider.enabled = false;
        m_ai.enabled = false;
        foreach (HitboxCollider hitbox in GetComponentsInChildren<HitboxCollider>())
        {
            hitbox.enabled = false;
        }
        m_interact.enabled = true;
        m_anim.OnDie();

        EnemyManager.Instance.RemoveExoEnemy(this);
    }

    private void Update()
    {
        m_anim.Animate(m_lastInputs);
    }

    private void FixedUpdate()
    {
        if (m_health.IsAlive)
        {
            m_lastInputs = m_ai.DecideActions();
            m_movement.UpdateMovement(m_lastInputs);
        }
    }

    public void EndInteract()
    {
        m_interact.EndInteraction();
    }
}
