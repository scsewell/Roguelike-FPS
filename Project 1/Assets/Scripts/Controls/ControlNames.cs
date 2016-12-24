using UnityEngine;
using System.Linq;

namespace InputController
{
    public class ControlNames
    {
        public static string GetName(GameButton button)
        {
            switch (button)
            {
                case GameButton.Menu:       return "Toggle Menu";
                case GameButton.RunHold:    return "Run";
                case GameButton.RunTap:     return "Tap to Run";
            }
            return button.ToString();
        }

        public static string GetName(GameAxis axis)
        {
            switch (axis)
            {
                case GameAxis.MoveX:        return "Strafe";
                case GameAxis.MoveY:        return "Move";
                case GameAxis.LookX:        return "Look Horizontal";
                case GameAxis.LookY:        return "Look Vertical";
            }
            return axis.ToString();
        }

        public static string GetName(GamepadButton button)
        {
            switch (button)
            {
                case GamepadButton.A:               return "A";
                case GamepadButton.B:               return "B";
                case GamepadButton.X:               return "X";
                case GamepadButton.Y:               return "Y";
                case GamepadButton.RShoulder:       return "R Bumper";
                case GamepadButton.LShoulder:       return "L Bumper";
                case GamepadButton.Back:            return "Back";
                case GamepadButton.Start:           return "Start";
                case GamepadButton.LStick:          return "L Stick";
                case GamepadButton.RStick:          return "R Stick";

                case GamepadButton.LTrigger:        return "L Trigger";
                case GamepadButton.RTrigger:        return "R Trigger";
                case GamepadButton.DpadUp:          return "Dpad Up";
                case GamepadButton.DpadDown:        return "Dpad Down";
                case GamepadButton.DpadLeft:        return "Dpad Left";
                case GamepadButton.DpadRight:       return "Dpad Right";
                case GamepadButton.LStickUp:        return "L Stick Up";
                case GamepadButton.LStickDown:      return "L Stick Down";
                case GamepadButton.LStickLeft:      return "L Stick Left";
                case GamepadButton.LStickRight:     return "L Stick Right";
                case GamepadButton.RStickUp:        return "R Stick Up";
                case GamepadButton.RStickDown:      return "R Stick Down";
                case GamepadButton.RStickLeft:      return "R Stick Left";
                case GamepadButton.RStickRight:     return "R Stick Right";
            }
            return null;
        }

        public static string GetName(GamepadAxis axis)
        {
            switch (axis)
            {
                case GamepadAxis.LStickX:           return "L Stick Horizontal";
                case GamepadAxis.LStickY:           return "L Stick Vertical";
                case GamepadAxis.RStickX:           return "R Stick Horizontal";
                case GamepadAxis.RStickY:           return "R Stick Vertical";
                case GamepadAxis.DpadX:             return "Dpad Horizontal";
                case GamepadAxis.DpadY:             return "Dpad Vertical";
                case GamepadAxis.Triggers:          return "Triggers";
            }
            return null;
        }

        public static string GetName(MouseAxis.Axis axis)
        {
            switch (axis)
            {
                case MouseAxis.Axis.ScrollWheel:    return "ScrollWheel";
                case MouseAxis.Axis.MouseX:         return "Mouse Horizontal";
                case MouseAxis.Axis.MouseY:         return "Mouse Vertical";
            }
            return null;
        }

        public static string GetName(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Mouse0:    return "Left Mouse";
                case KeyCode.Mouse1:    return "Right Mouse";
                case KeyCode.Mouse2:    return "Middle Mouse";
            }
            // return the keycode with spaces inserted before capital letters
            string s = keyCode.ToString();
            return string.Join(string.Empty,
                    s.Select((x, i) => (
                         char.IsUpper(x) && i > 0 &&
                         (char.IsLower(s[i - 1]) || (i < s.Count() - 1 && char.IsLower(s[i + 1])))
                    ) ? " " + x : x.ToString()).ToArray());
        }
    }
}