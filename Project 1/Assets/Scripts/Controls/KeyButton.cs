using UnityEngine;

namespace InputController
{
    class KeyButton : ISource<bool>
    {
        private KeyCode m_button;

        public KeyButton(KeyCode button)
        {
            m_button = button;
        }

        public bool GetValue()
        {
            return GetButtonValue(m_button);
        }

        public string GetName()
        {
            return ControlNames.GetName(m_button);
        }

        public SourceType GetSourceType()
        {
            return SourceType.MouseKeyboard;
        }

        public static bool GetButtonValue(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }
    }
}
