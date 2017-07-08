using UnityEngine;

public class GunBlocking : MonoBehaviour
{
    [SerializeField] [Range(0, 0.5f)]
    private float m_sweepDistance = 0.0165f;
    [SerializeField] [Range(0, 0.2f)]
    private float m_sweepRadius = 0.065f;
    [SerializeField]
    private LayerMask m_blockingLayers;

    public bool IsBlocked()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_sweepRadius, transform.forward, m_sweepDistance, m_blockingLayers);

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }
}
