using UnityEngine;
using System.Collections;

public class GunBlocking : MonoBehaviour
{
    public float sweepDistance = 0.5f;
    public float sweepRadius = 0.1f;
    public LayerMask blockingLayers;

    private bool m_isBlocked = false;

    void FixedUpdate()
    {
        m_isBlocked = false;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sweepRadius, transform.forward, sweepDistance, blockingLayers);

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
