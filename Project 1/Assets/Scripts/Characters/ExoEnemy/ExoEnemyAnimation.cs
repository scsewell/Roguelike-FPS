using UnityEngine;
using System;

public class ExoEnemyAnimation : MonoBehaviour
{
    [SerializeField] TransformPair[] m_toParent;

    private Animator m_anim;
    private Rigidbody[] m_ragdollBodies;
    private RagdollCollider[] m_ragdollColliders;

    private void Start()
    {
		m_anim = GetComponent<Animator>();
        m_ragdollBodies = GetComponentsInChildren<Rigidbody>();
        m_ragdollColliders = GetComponentsInChildren<RagdollCollider>();

        SetRagdoll(false);
    }

    public void OnDie()
    {
        m_anim.enabled = false;
        SetRagdoll(true);
    }

    public void Animate()
    {
		m_anim.SetFloat("forwardSpeed", 0);
		m_anim.SetFloat("sidewaysSpeed", 0);
	}

    private void SetRagdoll(bool activated)
    {
        if (activated)
        {
            foreach (TransformPair pair in m_toParent)
            {
                pair.toChild.SetParent(pair.parent, true);
            }
        }
        foreach (Rigidbody rigidbody in m_ragdollBodies)
        {
            rigidbody.isKinematic = !activated;
            if (activated)
            {
                //rigidbody.velocity = m_movement.ActualVelocity;
            }
        }
        foreach (RagdollCollider collider in m_ragdollColliders)
        {
            collider.SetEnabled(activated);
        }
    }
}

[Serializable]
public struct TransformPair
{
    public Transform toChild;
    public Transform parent;
}


