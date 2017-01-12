using UnityEngine;
using System;

public class ExoEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject m_exoskeleton;
    [SerializeField]
    private bool m_useExo = true;

    private ExoEnemyAI m_ai;
    private ExoEnemyAnimation m_anim;
    private Health m_health;
    private Interactable m_interact;
    private CharacterController m_collider;

    private Action<Interactable> m_endInteract;

    private void Start()
    {
        m_ai = GetComponent<ExoEnemyAI>();
        m_anim = GetComponent<ExoEnemyAnimation>();
        m_health = GetComponent<Health>();
        m_interact = GetComponent<Interactable>();
        m_collider = GetComponent<CharacterController>();

        if (!m_useExo)
        {
            Destroy(m_exoskeleton);
        }

        m_health.OnDie += OnDie;
        m_interact.InteractStart += OnInteractStart;
        m_interact.InteractEnd += OnInteractEnd;

        m_interact.enabled = false;
    }

    private void OnDestroy()
    {
        m_health.OnDie -= OnDie;
        m_interact.InteractStart -= OnInteractStart;
        m_interact.InteractEnd -= OnInteractEnd;
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

        EnemyManager.RemoveExoEnemy(transform);
    }

    private void OnInteractStart(Transform interacted, Vector3 interactPoint, Action<Interactable> endInteract)
    {
        m_endInteract = endInteract;
        m_anim.GrabBody(interacted, interactPoint);
    }

    private void OnInteractEnd()
    {
        m_anim.ReleaseBody();
    }

    private void Update()
    {
        m_anim.Animate();
    }

    private void FixedUpdate()
    {
        if (m_health.IsAlive)
        {
            m_ai.DecideActions();
        }
    }

    public void EndInteract()
    {
        if (m_endInteract != null)
        {
            m_endInteract(m_interact);
            m_endInteract = null;
        }
    }
}
