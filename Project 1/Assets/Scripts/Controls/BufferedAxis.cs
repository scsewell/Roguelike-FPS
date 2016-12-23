using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace InputController
{
    /*
     * Stores all the mouse and joystick axes that are relevant to a specific in game command.
     */
    public class BufferedAxis : BufferedSource<float>
    {
        private float m_exponent;

        public BufferedAxis(bool canBeMuted, float exponent, List<ISource<float>> sources) : base(canBeMuted, sources)
        {
            m_exponent = exponent;
        }

        /*
         * Returns the average value of the axes over the last gamplay update frame, or the last visual update.
         */
        public float AverageValue()
        {
            return GetRelevantInput(false).Average(sourceInputs => sourceInputs.ToList().Sum(input => GetInputValue(input)));
        }

        /*
         * Returns the cumulative value of the axes over the last gamplay update frame, or the last visual update.
         */
        public float CumulativeValue()
        {
            return GetRelevantInput(false).Sum(sourceInputs => sourceInputs.ToList().Sum(input => GetInputValue(input)));
        }

        /*
         * Applies modifications to the input values based on the type of source as required.
         */
        private float GetInputValue(KeyValuePair<ISource<float>, float> input)
        {
            return (input.Key.GetSourceType() == SourceType.Joystick) ? Mathf.Sign(input.Value) * Mathf.Pow(Mathf.Abs(input.Value), m_exponent) : input.Value;
        }
    }
}