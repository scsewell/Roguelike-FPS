using UnityEngine;
using System;
using Framework.Interpolation;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_interactLayers;
    [SerializeField]
    private LayerMask m_grabBlockLayers;
	[SerializeField] [Range(0, 2)]
    private float m_interactDistance = 0.6f;
	[SerializeField] [Range(0.01f, 2)]
    private float m_grabTime = 0.75f;
	[SerializeField] [Range(0, 1)]
    private float m_grabDistance = 0.2f;
	[SerializeField] [Range(0, 1)]
    private float m_grabVerticalOffset = 0.05f;
    
    private Transform m_grabTarget;
    private float m_grabTransition = 0;

    public event Action InteractOnce;
    public event Action InteractStart;
    public event Action InteractEnd;

    private Interactable m_hovered;
    private Interactable Hovered
    {
        get { return m_hovered; }
        set
        {
            if (m_hovered != value)
            {
                if (m_hovered != null)
                {
                    m_hovered.SetOutline(false);
                }
                m_hovered = value;
                if (m_hovered != null)
                {
                    m_hovered.SetOutline(true);
                }
            }
        }
    }

    private Interactable m_currentInteraction;
    public Interactable CurrentInteraction
    {
        get { return m_currentInteraction; }
    }

    public bool IsInteracting
    {
        get { return m_currentInteraction != null; }
    }

    public bool IsCarryingHeavy
    {
        get { return m_currentInteraction != null && m_currentInteraction.Heavy; }
    }

    private void Start()
    {
        GameObject grabTarget = new GameObject("GrabPos");
        Rigidbody body = grabTarget.AddComponent<Rigidbody>();
        body.isKinematic = true;
        grabTarget.AddComponent<TransformInterpolator>();
        m_grabTarget = grabTarget.transform;
    }

    public void MoveGrapTarget(CharacterMovement movement)
    {
        m_grabTransition = Mathf.Clamp01(m_grabTransition + (Time.deltaTime / m_grabTime));
        float lerpFac = 0.5f - (0.5f * Mathf.Cos(m_grabTransition * Mathf.PI));

        Vector3 goalPos = transform.position + m_grabDistance * transform.forward - m_grabVerticalOffset * transform.forward;
        goalPos.y = Mathf.Min(goalPos.y, movement.Controller.bounds.center.y);

        m_grabTarget.position = Vector3.Lerp(m_grabTarget.position, goalPos, lerpFac);
    }

    public void ProcessInteractions()
    {
        bool hasInteracted = ControlsManager.Instance.JustDown(GameButton.Interact);

        if (hasInteracted && IsInteracting)
        {
            EndInteract(m_currentInteraction);
            hasInteracted = false;
        }

        RaycastHit hit;
        if (!IsInteracting && Physics.Raycast(transform.position, transform.forward, out hit, m_interactDistance, m_interactLayers))
        {
            Interactable interactable = hit.transform.GetComponentInParent<Interactable>();

            if (interactable != null && interactable.enabled)
            {
                Hovered = interactable;

                if (hasInteracted)
                {
                    if (interactable.FireOnce)
                    {
                        interactable.OnInteractOnce(hit.transform, hit.point);
                        if (InteractOnce != null)
                        {
                            InteractOnce();
                        }
                    }
                    else
                    {
                        m_grabTransition = 0;
                        m_currentInteraction = interactable;
                        interactable.OnStartInteract(m_grabTarget, hit.transform, hit.point, () => EndInteract(interactable));
                        if (InteractStart != null)
                        {
                            InteractStart();
                        }
                    }
                }
            }
            else
            {
                Hovered = null;
            }
        }
        else
        {
            Hovered = null;
        }
    }

    private void EndInteract(Interactable interactable)
    {
        if (m_currentInteraction != null && interactable == m_currentInteraction)
        {
            m_currentInteraction.OnEndInteract();
            m_currentInteraction = null;
            if (InteractEnd != null)
            {
                InteractEnd();
            }
        }
    }
}
