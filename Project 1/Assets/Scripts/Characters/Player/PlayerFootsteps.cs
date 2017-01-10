using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private float m_crouchDistance = 0.75f;
    [SerializeField] private float m_walkDistance = 1.5f;
    [SerializeField] private float m_runDistance = 1.75f;

    private PlayerWeapons m_weapons;
    private CharacterMovement m_movement;
    private FootstepSounds m_footsteps;
    private bool m_footToggle = false;
    private float m_distance = 0;
    private Vector3 m_lastPos;

    private void Start()
    {
        m_weapons = GetComponentInChildren<PlayerWeapons>();
        m_movement = GetComponent<CharacterMovement>();
        m_footsteps = GetComponent<FootstepSounds>();

        m_movement.Land += OnLand;
    }

    private void OnLand()
    {
        Step();
    }

    private void FixedUpdate()
    {
        if (!m_weapons.IsPropActive())
        {
            m_distance += Vector3.Distance(transform.position, m_lastPos);
            m_lastPos = transform.position;

            float stepDistance = m_walkDistance;
            if (m_movement.IsCrouching())
            {
                stepDistance = m_crouchDistance;
            }
            else if (m_movement.IsRunning())
            {
                stepDistance = m_runDistance;
            }

            if (m_movement.IsGrounded() && m_distance > stepDistance)
            {
                Step();
            }
        }
        else
        {
            m_distance = 0;
        }
    }

    public void Step()
    {
        if (m_footToggle)
        {
            m_footsteps.FootstepL();
        }
        else
        {
            m_footsteps.FootstepR();
        }
        m_footToggle = !m_footToggle;
        m_distance = 0;
    }
}
