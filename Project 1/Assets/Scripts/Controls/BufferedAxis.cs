using System.Collections.Generic;
using System.Linq;

namespace InputController
{
    /*
     * Stores all the mouse and joystick axes that are relevant to a specific in game command.
     */
    public class BufferedAxis : BufferedSource<float>
    {
        public BufferedAxis(bool canBeMuted, List<ISource<float>> sources) : base(canBeMuted, sources)
        {
        }

        /*
         * Returns the average value of the axes over the last gamplay update frame, or the last visual update.
         */
        public float AverageValue()
        {
            return GetRelevantInput(false).Average(sourceInputs => sourceInputs.Values.Sum());
        }

        /*
         * Returns the cumulative value of the axes over the last gamplay update frame, or the last visual update.
         */
        public float CumulativeValue()
        {
            return GetRelevantInput(false).Sum(sourceInputs => sourceInputs.Values.Sum());
        }
    }
}