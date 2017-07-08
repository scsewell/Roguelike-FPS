using UnityEngine;

[RequireComponent(typeof(FootstepSounds))]
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] [Range(0,3)]
    private float m_crouchDistance = 0.75f;
    [SerializeField] [Range(0,3)]
    private float m_walkDistance = 1.5f;
    [SerializeField] [Range(0,3)]
    private float m_runDistance = 1.75f;
    
    private FootstepSounds m_footsteps;
    private bool m_footToggle = false;
    private float m_distance = 0;
    private Vector3 m_lastPos;

    private void Awake()
    {
        m_footsteps = GetComponent<FootstepSounds>();
    }

    public void Init(CharacterMovement movement)
    {
        movement.OnLand += Step;
    }
    
    public void UpdateSounds(PlayerWeapons weapons, CharacterMovement movement)
    {
        if (!weapons.IsPropActive)
        {
            m_distance += Vector3.Distance(transform.position, m_lastPos);
            m_lastPos = transform.position;

            float stepDistance = m_walkDistance;
            if (movement.IsCrouching)
            {
                stepDistance = m_crouchDistance;
            }
            else if (movement.IsRunning)
            {
                stepDistance = m_runDistance;
            }

            if (movement.IsGrounded && m_distance > stepDistance)
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
