using UnityEngine;
using System;
using System.Linq;

public class ExoEnemyAnimation : MonoBehaviour
{
    [SerializeField] TransformPair[] m_toParent;
    [SerializeField] DragCollider[] m_dragLimbs;
    [SerializeField] private float m_ragdollCenterSpeed = 2.0f;

    private ExoEnemy m_exoEnemy;
    private Animator m_anim;

    private Rigidbody[] m_ragdollBodies;
    private RagdollCollider[] m_ragdollColliders;
    private bool m_ragdollActive = false;
    private CharacterJoint m_grabJoint;
    private DragCollider m_grabbedColllider;

    private void Start()
    {
        m_exoEnemy = GetComponent<ExoEnemy>();
        m_anim = GetComponentInChildren<Animator>();
        m_ragdollBodies = GetComponentsInChildren<Rigidbody>();
        m_ragdollColliders = GetComponentsInChildren<RagdollCollider>();

        SetRagdoll(false);
    }

    public void OnDie()
    {
        m_anim.enabled = false;
        SetRagdoll(true);
    }

    public void GrabBody(Transform interacted, Vector3 interactPoint)
    {
        if (m_ragdollActive)
        {
            m_grabbedColllider = m_dragLimbs.ToList().OrderBy(c => Vector3.Distance(c.collider.bounds.center, interactPoint)).First();
            m_grabJoint = m_grabbedColllider.collider.GetComponentInParent<Rigidbody>().gameObject.AddComponent<CharacterJoint>();
            Rigidbody grabTarget = Camera.main.GetComponentInChildren<Rigidbody>();
            m_grabJoint.connectedBody = grabTarget;
            m_grabJoint.autoConfigureConnectedAnchor = false;
            m_grabJoint.connectedAnchor = grabTarget.GetComponent<Transform>().InverseTransformPoint(interactPoint);
            m_grabJoint.anchor = m_grabbedColllider.collider.transform.InverseTransformPoint(interactPoint);
            SoftJointLimit swing = new SoftJointLimit();
            swing.limit = -180;
            m_grabJoint.lowTwistLimit = swing;
            swing.limit = 180;
            m_grabJoint.highTwistLimit = swing;
            m_grabJoint.swing1Limit = swing;
            m_grabJoint.swing2Limit = swing;
        }
    }

    public void ReleaseBody()
    {
        if (m_ragdollActive)
        {
            Destroy(m_grabJoint);
        }
    }

    public void Animate()
    {
        if (!m_ragdollActive)
        {
            m_anim.SetFloat("forwardSpeed", 0);
            m_anim.SetFloat("sidewaysSpeed", 0);
        }
        else if (m_grabJoint != null)
        {
            m_grabJoint.connectedAnchor = Vector3.Lerp(m_grabJoint.connectedAnchor, Vector3.zero, Time.deltaTime * m_ragdollCenterSpeed);
            m_grabJoint.anchor = Vector3.Lerp(m_grabJoint.anchor, m_grabbedColllider.pivot, Time.deltaTime * m_ragdollCenterSpeed);
        }
	}

    private void SetRagdoll(bool activated)
    {
        m_ragdollActive = activated;
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
            rigidbody.drag = 1f;
            if (activated)
            {
                //rigidbody.velocity = m_movement.ActualVelocity;
                if (!rigidbody.GetComponent<TransformInterpolator>())
                {
                    TransformInterpolator interpolator = rigidbody.gameObject.AddComponent<TransformInterpolator>();
                    interpolator.SetThresholds(true, 0.005f, 0.5f, 0.01f);
                }
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

[Serializable]
public struct DragCollider
{
    public Collider collider;
    public Vector3 pivot;
}
