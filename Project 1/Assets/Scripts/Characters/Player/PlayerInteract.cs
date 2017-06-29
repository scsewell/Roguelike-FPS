using UnityEngine;
using System;
using Framework.Interpolation;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask m_interactLayers;
	[SerializeField] private float m_interactDistance = 0.65f;
    [SerializeField] private LayerMask m_grabBlockLayers;

    private CharacterController m_collider;
    private Interactable m_lastHovered;

    private Transform m_grabPos;
    public Rigidbody GrabTarget
    {
        get { return m_grabPos.GetComponent<Rigidbody>(); }
    }

    public event Action InteractOnce;
    public event Action InteractStart;
    public event Action InteractEnd;

    private Interactable m_currentInteraction;
    public bool IsInteracting
    {
        get { return m_currentInteraction != null; }
    }

    public bool IsCarryingHeavy
    {
        get { return m_currentInteraction != null && m_currentInteraction.Heavy; }
    }

    private void Awake()
    {
        m_collider = GetComponentInParent<CharacterController>();

        GameObject grabPos = new GameObject("GrabPos");
        Rigidbody body = grabPos.AddComponent<Rigidbody>();
        body.isKinematic = true;
        grabPos.AddComponent<TransformInterpolator>();
        m_grabPos = grabPos.transform;
    }

    private void FixedUpdate()
    {
        Vector3 goalPos = transform.position + 0.3f * Vector3.down;
        goalPos.y = Mathf.Min(goalPos.y, m_collider.transform.TransformPoint(m_collider.center + ((m_collider.height / 2) * Vector3.down)).y + 0.35f);
        m_grabPos.position = Vector3.Lerp(m_grabPos.position, goalPos, Time.deltaTime * 12f);
    }

    private void Update()
    {
        bool hasInteracted = Controls.Instance.JustDown(GameButton.Interact);

        if (hasInteracted && m_currentInteraction != null)
        {
            EndInteract(m_currentInteraction);
            hasInteracted = false;
        }

        RaycastHit hit;
        if (m_currentInteraction == null && Physics.Raycast(transform.position, transform.forward, out hit, m_interactDistance, m_interactLayers))
        {
            Interactable interactable = hit.transform.GetComponentInParent<Interactable>();

            if (m_lastHovered != null && interactable != m_lastHovered)
            {
                m_lastHovered.SetOutline(false);
            }

            if (interactable != null && interactable.enabled)
            {
                interactable.SetOutline(true);
                m_lastHovered = interactable;

                if (hasInteracted)
                {
                    if (interactable.FireOnce)
                    {
                        interactable.InteractOnce(hit.transform, hit.point);
                        if (InteractOnce != null)
                        {
                            InteractOnce();
                        }
                    }
                    else
                    {
                        m_currentInteraction = interactable;
                        interactable.StartInteract(hit.transform, hit.point, EndInteract);
                        if (InteractStart != null)
                        {
                            InteractStart();
                        }
                    }
                }
            }
        }
        else if (m_lastHovered != null)
        {
            m_lastHovered.SetOutline(false);
            m_lastHovered = null;
        }
    }

    private void EndInteract(Interactable interactable)
    {
        if (m_currentInteraction != null && interactable == m_currentInteraction)
        {
            m_currentInteraction.EndInteract();
            m_currentInteraction = null;
            if (InteractEnd != null)
            {
                InteractEnd();
            }
        }
    }
}
