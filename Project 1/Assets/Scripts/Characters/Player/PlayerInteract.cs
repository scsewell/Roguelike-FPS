using UnityEngine;
using System.Collections;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask m_blockingLayers;
	[SerializeField] private float m_interactDistance = 0.65f;
    [SerializeField] private LayerMask m_grabBlockLayers;
    [SerializeField] private Transform m_grabTarget;

    private Interactable m_lastHovered;

    private Transform m_grabPos;
    public Rigidbody GrabTarget
    {
        get { return m_grabPos.GetComponent<Rigidbody>(); }
    }

    private Interactable m_lastInteracted;
    public bool Interacting
    {
        get { return m_lastInteracted != null; }
    }

    private bool m_interact = false;
    public bool Interact
    {
        get { return m_interact; }
    }

    private void Awake()
    {
        GameObject grabPos = new GameObject("GrabPos");
        grabPos.AddComponent<TransformInterpolator>();
        Rigidbody body = grabPos.AddComponent<Rigidbody>();
        body.isKinematic = true;
        m_grabPos = grabPos.GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        RaycastHit grabHit;
        Vector3 grapTargetPos;
        Vector3 grabDisp = m_grabTarget.position - transform.position;
        if (Physics.SphereCast(transform.position, 0.15f, grabDisp.normalized, out grabHit, grabDisp.magnitude, m_grabBlockLayers))
        {
            grapTargetPos = grabHit.point + (0.15f * grabHit.normal);
        }
        else
        {
            grapTargetPos = m_grabTarget.position;
        }
        m_grabPos.position = Vector3.Lerp(m_grabPos.position, grapTargetPos, Time.deltaTime * 16f);
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
            if (Physics.Raycast(transform.position, transform.forward, out hit, m_interactDistance, m_blockingLayers))
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
                        if (interactable.fireOnce)
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
