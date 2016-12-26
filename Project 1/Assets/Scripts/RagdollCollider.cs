using UnityEngine;

public class RagdollCollider : MonoBehaviour
{
    [SerializeField]
    private Collider[] m_colliders;
    
	public void SetEnabled(bool activated)
    {
        foreach (Collider collider in m_colliders)
        {
            collider.enabled = activated;
        }
    }
}
