using UnityEngine;
using System;

public class ExoEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject m_exoskeleton;
    [SerializeField]
    private bool m_useExo = true;

    private ExoEnemyAnimation m_anim;
    private Health m_health;
    private Interactable m_interact;
    private CapsuleCollider m_collider;

    private Action<Interactable> m_endInteract;

    private void Start()
    {
        m_anim = GetComponent<ExoEnemyAnimation>();
        m_health = GetComponent<Health>();
        m_interact = GetComponent<Interactable>();
        m_collider = GetComponent<CapsuleCollider>();

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
        foreach (HitboxCollider hitbox in GetComponentsInChildren<HitboxCollider>())
        {
            hitbox.enabled = false;
        }
        m_interact.enabled = true;
        m_anim.OnDie();
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

    public void EndInteract()
    {
        if (m_endInteract != null)
        {
            m_endInteract(m_interact);
            m_endInteract = null;
        }
    }
}
