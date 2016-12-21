using System.Collections.Generic;
using System.Linq;

namespace InputController
{
    /*
     * Stores all the keyboard and joystick keys that are relevant to a specific in game command.
     */
    public class BufferedButton
    {
        private List<IButtonSource> m_sources;
        private List<List<Dictionary<IButtonSource, bool>>> m_buffers;

        private bool m_canBeMuted;
        public bool CanBeMuted
        {
            get { return m_canBeMuted; }
        }

        public BufferedButton(bool canBeMuted, List<IButtonSource> sources)
        {
            m_canBeMuted = canBeMuted;
            m_sources = new List<IButtonSource>(sources);
            ResetBuffers();
        }

        /*
         * Initializes the buffer lists from the current sources
         */
        public void ResetBuffers()
        {
            m_buffers = new List<List<Dictionary<IButtonSource, bool>>>();
            m_buffers.Add(new List<Dictionary<IButtonSource, bool>>());
            m_buffers.Last().Add(new Dictionary<IButtonSource, bool>());
            foreach (IButtonSource source in m_sources)
            {
                m_buffers.Last().Last().Add(source, source.IsDown());
            }
            m_buffers.Add(new List<Dictionary<IButtonSource, bool>>());
        }

        /*
         * Returns true if any of the relevant keyboard or joystick keys are down.
         * The latest visual update input will be used for the entire current gameplay update.
         */
        public bool IsDown()
        {
            return m_sources.Any((source) => (source.IsDown()));
        }

        /*
         * Returns true if a relevant keyboard or joystick key was pressed since the last FixedUpdate.
         */
        public bool JustDown()
        {
            List<Dictionary<IButtonSource, bool>> buffer = GetRelevantInput();

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
            List<Dictionary<IButtonSource, bool>> buffer = GetRelevantInput();

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
            List<Dictionary<IButtonSource, bool>> buffer = GetRelevantInput();

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
            List<Dictionary<IButtonSource, bool>> buffer = GetRelevantInput();

            if (buffer.Count > 1 && !buffer[buffer.Count - 1].Values.Any(boolie => boolie) && buffer[buffer.Count - 2].Values.Any(boolie => boolie))
            {
                return true;
            }
            return false;
        }

        /*
         * Run at the end of every visual update frame to record the input state for that frame.
         */
        public void RecordUpdateState()
        {
            m_buffers.Last().Add(new Dictionary<IButtonSource, bool>());

            foreach (IButtonSource source in m_sources)
            {
                m_buffers.Last().Last().Add(source, source.IsDown());
            }
        }

        /*
         * Run at the end of every fixed update to remove old inputs from buffers.
         */
        public void RecordFixedUpdateState()
        {
            // ensures there are inputs from two visual updates and inputs from two fixed updates in the buffer
            while (m_buffers.GetRange(1, m_buffers.Count - 1).Sum((gameplayUpdateInputs) => (gameplayUpdateInputs.Count)) >= 1)
            {
                m_buffers.RemoveAt(0);
            }
            m_buffers.Add(new List<Dictionary<IButtonSource, bool>>());
        }

        private List<Dictionary<IButtonSource, bool>> GetRelevantInput()
        {
            List<Dictionary<IButtonSource, bool>> buffer = new List<Dictionary<IButtonSource, bool>>();
            buffer.Add(m_buffers.GetRange(0, m_buffers.Count - 1).Last((gameplayUpdate) => (gameplayUpdate.Any())).Last());
            return buffer.Concat(m_buffers.Last()).ToList();
        }
        
        public List<KeyValuePair<SourceType, string>> GetSourceNames()
        {
            List<KeyValuePair<SourceType, string>> sourceNames = new List<KeyValuePair<SourceType, string>>();
            foreach (ISource source in m_sources)
            {
                sourceNames.Add(new KeyValuePair<SourceType, string>(source.GetSourceType(), source.GetName()));
            }
            return sourceNames;
        }
    }
}