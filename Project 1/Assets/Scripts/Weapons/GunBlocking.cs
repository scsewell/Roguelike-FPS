using UnityEngine;

public class GunBlocking : MonoBehaviour
{
    [SerializeField] private float m_sweepDistance = 0.0165f;
    [SerializeField] private float m_sweepRadius = 0.065f;
    [SerializeField] private LayerMask m_blockingLayers;

    private bool m_isBlocked = false;

    private void FixedUpdate()
    {
        m_isBlocked = false;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_sweepRadius, transform.forward, m_sweepDistance, m_blockingLayers);

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.isTrigger)
            {
                m_isBlocked = true;
                break;
            }
        }
    }

    public bool IsBlocked()
    {
        return m_isBlocked;
    }
}
