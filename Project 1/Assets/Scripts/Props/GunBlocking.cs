using UnityEngine;

public class GunBlocking : MonoBehaviour
{
    [SerializeField] [Range(0, 0.5f)]
    private float m_sweepDistance = 0.0165f;
    [SerializeField] [Range(0, 0.2f)]
    private float m_sweepRadius = 0.065f;
    [SerializeField]
    private LayerMask m_blockingLayers;

    private RaycastHit[] m_hits = new RaycastHit[20];

    public bool IsBlocked()
    {
        int hitCount = Physics.SphereCastNonAlloc(transform.position, m_sweepRadius, transform.forward, m_hits, m_sweepDistance, m_blockingLayers);

        for (int i = 0; i < hitCount; i++)
        {
            if (!m_hits[i].collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }
}
