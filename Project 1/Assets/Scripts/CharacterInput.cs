using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterMovement))]

public class CharacterInput : MonoBehaviour
{
    private bool m_running = false;
    private Vector3 m_directionVector = Vector3.zero;
    private CharacterMovement m_character;

    private void Awake()
    {
        m_character = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        float x = Controls.AverageValue(GameAxis.MoveX);
        float z = Controls.AverageValue(GameAxis.MoveY);

        m_directionVector = Vector3.ClampMagnitude(new Vector3(x, 0, z), 1f);
        m_directionVector *= m_directionVector.magnitude;
    }

    private void FixedUpdate()
    {
        m_character.inputMoveDirection = transform.rotation * m_directionVector;

        m_character.inputJump = Controls.IsDown(GameButton.Jump);
        m_character.inputCrouchToggle = Controls.JustDown(GameButton.Crouch);

        if (Controls.JustDown(GameButton.Run) && !Controls.IsDown(GameButton.Fire))
        {
            m_running = true;
        }
        else if (Controls.JustUp(GameButton.Run) || Controls.IsDown(GameButton.Fire))
        {
            m_running = false;
        }

        m_character.inputRunning = m_running;
    }

    public Vector3 GetMoveDirection()
    {
        return m_directionVector;
    }
}