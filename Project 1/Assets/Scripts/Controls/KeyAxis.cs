using UnityEngine;

namespace InputController
{
    /*
     * Stores an axis type input for a pair of keys.
     */
    public class KeyAxis : ISource<float>
    {
        private KeyCode m_negative;
        private KeyCode m_positive;

        public KeyAxis(KeyCode negative, KeyCode positive)
        {
            m_negative = negative;
            m_positive = positive;
        }

        // returns the value of the axis
        public float GetValue()
        {
            return GetKeyValue(m_positive) - GetKeyValue(m_negative);
        }

        public string GetName()
        {
            return ControlNames.GetName(m_positive) + "-" + ControlNames.GetName(m_negative);
        }

        private float GetKeyValue(KeyCode key)
        {
            return Input.GetKey(key) ? 1 : 0;
        }

        public SourceType GetSourceType()
        {
            return SourceType.MouseKeyboard;
        }
    }
}