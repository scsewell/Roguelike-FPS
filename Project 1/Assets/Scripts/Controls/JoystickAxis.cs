using UnityEngine;

namespace InputController
{
    /*
     * Stores an axis type input for a joystick.
     */
    public class JoystickAxis : ISource<float>
    {
        private GamepadAxis m_axis;

        public JoystickAxis(GamepadAxis axis)
        {
            m_axis = axis;
        }
        
        public float GetValue()
        {
            return GetAxisValue(m_axis);
        }

        public string GetName()
        {
            return ControlNames.GetName(m_axis);
        }

        public SourceType GetSourceType()
        {
            return SourceType.Joystick;
        }

        public static float GetAxisValue(GamepadAxis axis)
        {
            switch (axis)
            {
                case GamepadAxis.DpadX:
                    return Input.GetAxis("DPad_XAxis");
                case GamepadAxis.DpadY:
                    return -Input.GetAxis("DPad_YAxis");
                case GamepadAxis.LStickX:
                    return Input.GetAxis("L_XAxis");
                case GamepadAxis.LStickY:
                    return -Input.GetAxis("L_YAxis");
                case GamepadAxis.RStickX:
                    return Input.GetAxis("R_XAxis");
                case GamepadAxis.RStickY:
                    return -Input.GetAxis("R_YAxis");
                case GamepadAxis.Triggers:
                    /*
                    return Input.GetAxis("Triggers");
                    /*/
                    float LTrigger = Input.GetAxis("TriggersL");
                    float RTrigger = Input.GetAxis("TriggersR");
                    return RTrigger - LTrigger;
                    //*/
            }
            return 0;
        }
    }
}