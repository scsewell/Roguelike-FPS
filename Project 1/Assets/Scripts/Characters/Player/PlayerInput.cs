using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Vector2 m_desiredMove = Vector2.zero;
    public Vector2 DesiredMove
    {
        get { return m_desiredMove; }
    }

    private bool m_running = false;
    private bool m_crouching = false;

    public MoveInputs GetInput(PlayerInteract interaction)
    {
        MoveInputs inputs = new MoveInputs();

        float x = ControlsManager.Instance.AverageValue(GameAxis.MoveX);
        float y = ControlsManager.Instance.AverageValue(GameAxis.MoveY);

        m_desiredMove = Vector2.ClampMagnitude(new Vector2(x, y), 1f);
        m_desiredMove *= m_desiredMove.magnitude;

        inputs.MoveDirection = transform.rotation * new Vector3(m_desiredMove.x, 0, m_desiredMove.y);

        if (ControlsManager.Instance.IsDown(GameButton.Fire) || m_desiredMove.magnitude < 0.3f)
        {
            inputs.Run = false;
            m_running = false;
        }
        else
        {
            if (ControlsManager.Instance.JustDown(GameButton.RunTap))
            {
                m_running = !m_running;
            }
            inputs.Run = ControlsManager.Instance.IsDown(GameButton.RunHold) ? !m_running : m_running;
        }

        if (ControlsManager.Instance.JustDown(GameButton.Crouch))
        {
            m_crouching = !m_crouching;
        }
        if (inputs.Run)
        {
            m_crouching = false;
        }
        inputs.Crouch = m_crouching;

        inputs.Jump = ControlsManager.Instance.IsDown(GameButton.Jump);
        inputs.Burdened = interaction.IsCarryingHeavy;

        return inputs;
    }
}