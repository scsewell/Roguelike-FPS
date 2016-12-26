using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class CharacterInput : MonoBehaviour
{
    private CharacterMovement m_character;
    private Vector3 m_directionVector = Vector3.zero;
    private bool m_running = false;

    private void Awake()
    {
        m_character = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        float x = Controls.Instance.AverageValue(GameAxis.MoveX);
        float z = Controls.Instance.AverageValue(GameAxis.MoveY);

        m_directionVector = Vector3.ClampMagnitude(new Vector3(x, 0, z), 1f);
        m_directionVector *= m_directionVector.magnitude;
    }

    private void FixedUpdate()
    {
        m_character.inputMoveDirection = transform.rotation * m_directionVector;

        m_character.inputJump = Controls.Instance.IsDown(GameButton.Jump);
        m_character.inputCrouchToggle = Controls.Instance.JustDown(GameButton.Crouch);

        bool run;
        if (Controls.Instance.IsDown(GameButton.Fire) || m_directionVector.magnitude < 0.3f)
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
    }

    public Vector3 GetMoveDirection()
    {
        return m_directionVector;
    }
}