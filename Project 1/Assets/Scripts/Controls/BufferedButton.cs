using System.Collections.Generic;
using System.Linq;

namespace InputController
{
    /*
     * Stores all the keyboard and joystick keys that are relevant to a specific in game command.
     */
    public class BufferedButton : BufferedSource<bool>
    {
        public BufferedButton(bool canBeMuted, List<ISource<bool>> sources) : base(canBeMuted, sources)
        {
        }

        /*
         * Returns true if any of the relevant keyboard or joystick keys are down this frame.
         */
        public bool IsDown()
        {
            return m_sources.Any(source => source.GetValue());
        }

        /*
         * Returns true if a relevant keyboard or joystick key was pressed since the last FixedUpdate.
         */
        public bool JustDown()
        {
            List<Dictionary<ISource<bool>, bool>> buffer = GetRelevantInput(true);

            for (int i = buffer.Count - 1; i > 0; i--)
            {
                if (buffer[i].Values.Any(boolie => boolie) && !buffer[i - 1].Values.Any(boolie => boolie))
                {
                    return true;
                }
            }
            return false;
        }

        /*
         * Returns true if a relevant keyboard or joystick key was released since the last FixedUpdate.
         */
        public bool JustUp()
        {
            List<Dictionary<ISource<bool>, bool>> buffer = GetRelevantInput(true);

            for (int i = buffer.Count - 1; i > 0; i--)
            {
                if (!buffer[i].Values.Any(boolie => boolie) && buffer[i - 1].Values.Any(boolie => boolie))
                {
                    return true;
                }
            }
            return false;
        }

        /*
         * Returns true if a relevant keyboard or joystick key was pressed this frame.
         */
        public bool VisualJustDown()
        {
            List<Dictionary<ISource<bool>, bool>> buffer = GetRelevantInput(true);

            if (buffer.Count > 1 && buffer[buffer.Count - 1].Values.Any(boolie => boolie) && !buffer[buffer.Count - 2].Values.Any(boolie => boolie))
            {
                return true;
            }
            return false;
        }

        /*
         * Returns true if a relevant keyboard or joystick key was released this frame.
         */
        public bool VisualJustUp()
        {
            List<Dictionary<ISource<bool>, bool>> buffer = GetRelevantInput(true);

            if (buffer.Count > 1 && !buffer[buffer.Count - 1].Values.Any(boolie => boolie) && buffer[buffer.Count - 2].Values.Any(boolie => boolie))
            {
                return true;
            }
            return false;
        }
    }
}