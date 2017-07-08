using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Interpolation;

public class ExoEnemyAnimation : MonoBehaviour
{
    [SerializeField] private float m_moveChangeRate = 4f;
    [SerializeField] TransformPair[] m_toParent;
    [SerializeField] DragCollider[] m_dragLimbs;
    [SerializeField] private float m_ragdollCenterSpeed = 2f;

    private ExoEnemy m_exoEnemy;
    private CharacterMovement m_movement;
    private Animator m_anim;

    private Rigidbody[] m_ragdollBodies;
    private RagdollCollider[] m_ragdollColliders;
    private List<TransformInterpolator> m_interpolators; 
    private bool m_ragdollActive = false;
    private CharacterJoint m_grabJoint;
    private DragCollider m_grabbedColllider;

    private void Start()
    {
        m_exoEnemy = GetComponent<ExoEnemy>();
        m_movement = GetComponent<CharacterMovement>();
        m_anim = GetComponent<Animator>();
        m_ragdollBodies = GetComponentsInChildren<Rigidbody>();
        m_ragdollColliders = GetComponentsInChildren<RagdollCollider>();

        m_interpolators = new List<TransformInterpolator>();
        foreach (Rigidbody rigidbody in m_ragdollBodies)
        {
            TransformInterpolator interpolator = rigidbody.gameObject.AddComponent<TransformInterpolator>();
            interpolator.useThresholds = true;
            interpolator.SetThresholds(0.005f, 0.5f, 0.01f);
            m_interpolators.Add(interpolator);
        }
        m_interpolators.ForEach(i => i.enabled = false);

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
            m_grabbedColllider = m_dragLimbs.OrderBy(c => Vector3.Distance(c.collider.bounds.center, interactPoint)).First();

            Transform grabTarget = Camera.main.GetComponent<PlayerInteract>().GrabTarget;

            m_grabJoint = m_grabbedColllider.collider.GetComponentInParent<Rigidbody>().gameObject.AddComponent<CharacterJoint>();
            m_grabJoint.connectedBody = grabTarget.GetComponent<Rigidbody>();
            m_grabJoint.autoConfigureConnectedAnchor = false;
            m_grabJoint.connectedAnchor = Vector3.zero;
            m_grabJoint.anchor = m_grabbedColllider.pivot;

            SoftJointLimitSpring spring = new SoftJointLimitSpring();
            spring.spring = 0.01f;
            m_grabJoint.twistLimitSpring = spring;
            m_grabJoint.swingLimitSpring = spring;
            
            SoftJointLimit swing = new SoftJointLimit();
            swing.limit = -180;
            m_grabJoint.lowTwistLimit = swing;

            swing.limit = 180;
            m_grabJoint.highTwistLimit = swing;
            m_grabJoint.swing1Limit = swing;
            m_grabJoint.swing2Limit = swing;

            m_grabJoint.breakForce = 5000;

            grabTarget.position = m_grabbedColllider.collider.transform.TransformPoint(m_grabbedColllider.pivot);
            grabTarget.rotation = m_grabbedColllider.collider.transform.rotation;
            grabTarget.GetComponent<TransformInterpolator>().ForgetPreviousValues();

            m_interpolators.ForEach(i => i.enabled = true);
        }
    }

    public void ReleaseBody()
    {
        if (m_ragdollActive)
        {
            Destroy(m_grabJoint);
            m_interpolators.ForEach(i => i.enabled = false);
        }
    }

    public void Animate(MoveInputs input)
    {
        if (!m_ragdollActive)
        {
            float targetSpeed = m_movement.Velocity.magnitude > 0 ? (m_movement.IsRunning ? 2 : 1) : 0;
            m_anim.SetFloat("forwardSpeed", Mathf.MoveTowards(m_anim.GetFloat("forwardSpeed"), targetSpeed, Time.deltaTime * m_moveChangeRate));
            m_anim.SetFloat("sidewaysSpeed", 0);
        }
        else if (m_grabJoint != null)
        {
            m_grabJoint.connectedAnchor = Vector3.Lerp(m_grabJoint.connectedAnchor, Vector3.zero, Time.deltaTime * m_ragdollCenterSpeed);
            m_grabJoint.anchor = Vector3.Lerp(m_grabJoint.anchor, m_grabbedColllider.pivot, Time.deltaTime * m_ragdollCenterSpeed);
        }
        else
        {
            m_exoEnemy.EndInteract();
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
            if (activated)
            {
                rigidbody.drag = 1f;
                rigidbody.velocity = m_movement.Velocity;
            }
        }
        foreach (RagdollCollider collider in m_ragdollColliders)
        {
            collider.SetEnabled(activated);
        }
        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.updateWhenOffscreen = activated;
        }
    }

    [Serializable]
    private struct TransformPair
    {
        public Transform toChild;
        public Transform parent;
    }

    [Serializable]
    private struct DragCollider
    {
        public Collider collider;
        public Vector3 pivot;
    }
}
