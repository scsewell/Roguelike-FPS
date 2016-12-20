namespace InputController
{
    /*
     * Stores an axis type input for a pair of keys.
     */
    public class JoystickButtonAxis : AxisSource
    {
        private GamepadButton m_negative;
        private GamepadButton m_positive;
        private float m_multiplier;

        public JoystickButtonAxis(GamepadButton negative, GamepadButton positive, float multiplier)
        {
            m_negative = negative;
            m_positive = positive;
            m_multiplier = multiplier;
        }

        // returns the value of the axis
        public float GetValue()
        {
            return (GetButtonValue(m_positive) - GetButtonValue(m_negative)) * m_multiplier;
        }

        public string GetName()
        {
            return ControlNames.GetName(m_positive) + "-" + ControlNames.GetName(m_negative);
        }

        private float GetButtonValue(GamepadButton button)
        {
            return JoystickButton.GetButtonValue(button) ? 1 : 0;
        }
    }
}