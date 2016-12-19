using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask m_blockingLayers;
	[SerializeField] private string m_interactiveTag = "Interactive";
	[SerializeField] private float m_interactDistance = 0.65f;

	private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, m_interactDistance, m_blockingLayers) && hit.collider.transform.tag == m_interactiveTag)
        {
            if (Controls.JustDown(GameButton.Interact))
            {
                hit.collider.SendMessageUpwards("Interact");
            }
        }
	}
}
