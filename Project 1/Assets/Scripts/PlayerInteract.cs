using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask m_blockingLayers;
	[SerializeField] private float m_interactDistance = 0.65f;

    private Interactable m_lastHovered;

	private void Update()
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

                if (Controls.Instance.JustDown(GameButton.Interact))
                {
                    interactable.Interact();
                }
            }
        }
        else if (m_lastHovered != null)
        {
            m_lastHovered.SetOutline(false);
            m_lastHovered = null;
        }
	}
}
