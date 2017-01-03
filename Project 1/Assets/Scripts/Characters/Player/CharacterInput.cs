using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class CharacterInput : MonoBehaviour
{
    private CharacterMovement m_character;
    private PlayerInteract m_interact;
    private Vector3 m_moveInput = Vector3.zero;
    private bool m_running = false;
    private bool m_crouching = false;

    private void Awake()
    {
        m_character = GetComponent<CharacterMovement>();
        m_interact = GetComponentInChildren<PlayerInteract>();
    }

    private void Update()
    {
        float x = Controls.Instance.AverageValue(GameAxis.MoveX);
        float z = Controls.Instance.AverageValue(GameAxis.MoveY);

        m_moveInput = Vector3.ClampMagnitude(new Vector3(x, 0, z), 1f);
        m_moveInput *= m_moveInput.magnitude;
    }

    private void FixedUpdate()
    {
        m_character.inputMoveDirection = transform.rotation * m_moveInput;
        m_character.inputJump = Controls.Instance.IsDown(GameButton.Jump);
        m_character.inputBurdened = m_interact.IsCarryingHeavy;

        bool run;
        if (Controls.Instance.IsDown(GameButton.Fire) || m_moveInput.magnitude < 0.3f)
        {
            run = false;
            m_running = false;
        }
        else
        {
            if (Controls.Instance.JustDown(GameButton.RunTap))
            {
                m_running = !m_running;
            }
            run = Controls.Instance.IsDown(GameButton.RunHold) ? !m_running : m_running;
        }
        m_character.inputRunning = run;

        if (Controls.Instance.JustDown(GameButton.Crouch))
        {
            m_crouching = !m_crouching;
        }
        if (run)
        {
            m_crouching = false;
        }
        m_character.inputCrouch = m_crouching;
    }

    public Vector3 GetMoveInput()
    {
        return m_moveInput;
    }
}