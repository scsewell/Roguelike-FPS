namespace InputController
{
    /*
     * Stores an axis type input for a pair of keys.
     */
    public class JoystickButtonAxis : ISource<float>
    {
        private GamepadButton m_negative;
        private GamepadButton m_positive;

        public JoystickButtonAxis(GamepadButton negative, GamepadButton positive)
        {
            m_negative = negative;
            m_positive = positive;
        }

        // returns the value of the axis
        public float GetValue()
        {
            return GetButtonValue(m_positive) - GetButtonValue(m_negative);
        }

        public string GetName()
        {
            return ControlNames.GetName(m_positive) + "-" + ControlNames.GetName(m_negative);
        }

        public SourceType GetSourceType()
        {
            return SourceType.Joystick;
        }

        private float GetButtonValue(GamepadButton button)
        {
            return JoystickButton.GetButtonValue(button) ? 1 : 0;
        }
    }
}