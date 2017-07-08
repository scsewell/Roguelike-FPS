using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class ExoEnemyAI : MonoBehaviour
{
    [SerializeField]
    private float m_repathFactor = 0.25f;
    [SerializeField]
    private float m_waypointTolerance = 0.3f;
    [SerializeField]
    private float m_targetTolerance = 0.6f;

    private CharacterController m_controller;
    private CharacterMovement m_movement;

    private Transform m_player;
    private Transform m_target;
    private NavMeshPath m_path;
    private LinkedList<Vector3> m_waypoints;
    private bool m_run = false;

    public void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_movement = GetComponent<CharacterMovement>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            m_player = player.transform;
        }

        m_path = new NavMeshPath();
    }

    public MoveInputs DecideActions()
    {
        MoveInputs input = new MoveInputs();

        if (m_target == null)
        {
            m_target = SetTarget(m_player);
        }

        if (Random.value * 5 < Time.deltaTime)
        {
            m_run = !m_run;
        }
        input.Run = m_run;

        if (m_target != null)
        {
            Vector3 targetDisplacement = Vector3.ProjectOnPlane(m_target.position - transform.position, Vector3.up);

            // calculate a new path when the target has moved too far from where a path was last caculated to
            if (m_waypoints == null || m_waypoints.Count == 0 || Vector3.Distance(m_target.position, m_waypoints.Last()) > targetDisplacement.magnitude * m_repathFactor)
            {
                NavMesh.CalculatePath(transform.position, m_target.position, int.MaxValue, m_path);
                m_waypoints = new LinkedList<Vector3>(m_path.corners);
            }

            // if there is a path to follow set the movment
            if (m_waypoints != null && m_waypoints.Count > 0)
            {
                // remove waypoints that have been reached
                Vector3 waypointDisp = Vector3.ProjectOnPlane(m_waypoints.First() - transform.position, Vector3.up);
                while (m_waypoints.Count > 1 && waypointDisp.magnitude < m_waypointTolerance)
                {
                    m_waypoints.RemoveFirst();
                    waypointDisp = Vector3.ProjectOnPlane(m_waypoints.First() - transform.position, Vector3.up);
                }

                // move towards the next waypoint
                if (targetDisplacement.magnitude > m_targetTolerance)
                {
                    Vector3 moveDir = waypointDisp;
                    moveDir.y = 0;
                    Quaternion targetDir = Quaternion.LookRotation(moveDir, Vector3.up);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, Time.deltaTime * 8);
                    input.MoveDirection = transform.forward;
                }
            }
        }
        
        if (Application.isEditor)
        {
            DrawPath();
        }

        return input;
    }

    private Transform SetTarget(Vector3 targetPos)
    {
        Transform target = CreateTarget();
        target.position = targetPos;
        return target;
    }

    private Transform SetTarget(Transform toTarget)
    {
        Transform target = CreateTarget();
        target.SetParent(toTarget, false);
        return target;
    }

    private Transform CreateTarget()
    {
        if (m_target != null)
        {
            Destroy(m_target.gameObject);
        }
        return new GameObject("ExoTarget").transform;
    }

    private void DrawPath()
    {
        if (m_waypoints != null && m_waypoints.Count > 0)
        {
            Debug.DrawLine(transform.position, m_waypoints.First(), Color.magenta);
            for (int i = 0; i < (m_waypoints.Count - 1); i++)
            {
                Debug.DrawLine(m_waypoints.ElementAt(i), m_waypoints.ElementAt(i + 1), Color.yellow);
            }
        }
    }
}
