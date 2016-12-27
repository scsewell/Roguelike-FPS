using UnityEngine;
using System.Collections;

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

    private bool m_interact = false;
    public bool Interacted
    {
        get { return m_interact; }
    }

    private Interactable m_lastInteracted;
    public bool IsInteracting
    {
        get { return m_lastInteracted != null; }
    }

    public bool IsCarryingHeavy
    {
        get { return m_lastInteracted != null && m_lastInteracted.Heavy; }
    }

    private void Awake()
    {
        m_collider = GetComponentInParent<CharacterController>();

        GameObject grabPos = new GameObject("GrabPos");
        Rigidbody body = grabPos.AddComponent<Rigidbody>();
        body.isKinematic = true;
        grabPos.AddComponent<TransformInterpolator>();
        m_grabPos = grabPos.GetComponent<Transform>();
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

        if (hasInteracted && m_lastInteracted != null)
        {
            EndInteract(m_lastInteracted);
            hasInteracted = false;
        }

        if (m_lastInteracted == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, m_interactDistance, m_interactLayers))
            {
                Interactable interactable = hit.transform.GetComponentInParent<Interactable>();

                if (m_lastHovered != null && interactable != m_lastHovered)
                {
                    m_lastHovered.SetOutline(false);
                }

                if (interactable != null)
                {
                    interactable.SetOutline(true);
                    m_lastHovered = interactable;

                    if (hasInteracted)
                    {
                        if (interactable.FireOnce)
                        {
                            interactable.InteractOnce(hit.transform, hit.point);
                            StartCoroutine(ResetInteracting());
                        }
                        else
                        {
                            m_lastInteracted = interactable;
                            interactable.StartInteract(hit.transform, hit.point, EndInteract);
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
        else if (m_lastHovered != null)
        {
            m_lastHovered.SetOutline(false);
            m_lastHovered = null;
        }
    }

    private void EndInteract(Interactable interactable)
    {
        if (m_lastInteracted != null && interactable == m_lastInteracted)
        {
            m_lastInteracted.EndInteract();
            m_lastInteracted = null;
        }
    }

    private IEnumerator ResetInteracting()
    {
        m_interact = true;
        yield return null;
        m_interact = false;
    }
}
