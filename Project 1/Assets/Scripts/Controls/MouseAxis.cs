using UnityEngine;

namespace InputController
{
    /*
     * Stores an axis type input for the mouse.
     */
    public class MouseAxis : ISource<float>
    {
        public enum Axis
        {
            ScrollWheel,
            MouseX,
            MouseY
        }

        private Axis m_axis;
        private float m_threshold;

        public MouseAxis(Axis axis, float threshold = 0)
        {
            m_axis = axis;
            m_threshold = threshold;
        }

        // returns the value of the relevant axis
        public float GetValue()
        {
            return GetAxisValue(m_axis, m_threshold);
        }

        public string GetName()
        {
            return ControlNames.GetName(m_axis);
        }

        public SourceType GetSourceType()
        {
            return SourceType.MouseKeyboard;
        }

        public static float GetAxisValue(Axis mouseAxis, float thresh = 0)
        {
            switch (mouseAxis)
            {
                case Axis.ScrollWheel: return ThresholdValue(Input.GetAxis("Mouse ScrollWheel"), thresh) * 0.08f / Time.deltaTime;
                case Axis.MouseX: return ThresholdValue(Input.GetAxis("Mouse X"), thresh) * 0.008f / Time.deltaTime;
                case Axis.MouseY: return ThresholdValue(Input.GetAxis("Mouse Y"), thresh) * 0.008f / Time.deltaTime;
            }
            return 0;
        }

        private static float ThresholdValue(float value, float thresh)
        {
            return Mathf.Abs(value) > thresh ? value : 0;
        }
    }
}