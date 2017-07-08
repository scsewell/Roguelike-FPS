using UnityEngine;
using System;
using Framework.Interpolation;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_interactLayers;
	[SerializeField] [Range(0,2)]
    private float m_interactDistance = 0.6f;
    [SerializeField]
    private LayerMask m_grabBlockLayers;

    private CharacterController m_collider;
    private Interactable m_lastHovered;
    private float m_grabTransition = 0;

    public event Action InteractOnce;
    public event Action InteractStart;
    public event Action InteractEnd;

    private Transform m_grabTarget;
    public Transform GrabTarget
    {
        get { return m_grabTarget; }
    }


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

        GameObject grabTarget = new GameObject("GrabPos");
        Rigidbody body = grabTarget.AddComponent<Rigidbody>();
        body.isKinematic = true;
        grabTarget.AddComponent<TransformInterpolator>();
        m_grabTarget = grabTarget.transform;
    }

    public void MoveGrapTarget(CharacterMovement movement)
    {
        m_grabTransition = Mathf.Clamp01(m_grabTransition + Time.deltaTime);
        float lerpFac = 0.5f - (0.5f * Mathf.Cos(m_grabTransition * Mathf.PI));

        Vector3 goalPos = transform.position + 0.3f * transform.forward;
        goalPos.y = Mathf.Min(goalPos.y, movement.Controller.bounds.center.y + 0.35f);
        
        GrabTarget.position = Vector3.Lerp(GrabTarget.position, goalPos, lerpFac);
    }

    public void ProcessInteractions()
    {
        bool hasInteracted = ControlsManager.Instance.JustDown(GameButton.Interact);

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
                        m_grabTransition = 0;
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
