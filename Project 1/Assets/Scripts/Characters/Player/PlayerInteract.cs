using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask m_blockingLayers;
	[SerializeField] private float m_interactDistance = 0.65f;

    private Interactable m_lastInteracted;
    private Interactable m_lastHovered;

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
        else if(m_lastHovered != null)
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
}
