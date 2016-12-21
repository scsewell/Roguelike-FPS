using System.Collections.Generic;
using System.Linq;

namespace InputController
{
    /*
     * Stores all the mouse and joystick axes that are relevant to a specific in game command.
     */
    public class BufferedAxis
    {
        private List<IAxisSource> m_sources;
        private List<List<Dictionary<IAxisSource, float>>> m_buffers;

        private bool m_canBeMuted;
        public bool CanBeMuted
        {
            get { return m_canBeMuted; }
        }

        public BufferedAxis(bool canBeMuted, List<IAxisSource> sources)
        {
            m_canBeMuted = canBeMuted;
            m_sources = new List<IAxisSource>(sources);

            m_buffers = new List<List<Dictionary<IAxisSource, float>>>();
            m_buffers.Add(new List<Dictionary<IAxisSource, float>>());
            m_buffers.Last().Add(new Dictionary<IAxisSource, float>());
            foreach (IAxisSource source in m_sources)
            {
                m_buffers.Last().Last().Add(source, source.GetValue());
            }
            m_buffers.Add(new List<Dictionary<IAxisSource, float>>());
        }

        /*
         * Returns the average value of the axes over the last gamplay update frame, or the last visual update.
         */
        public float AverageValue()
        {
            return GetRelevantInput().Average((visualUpdateInputs) => (visualUpdateInputs.Values.Sum()));
        }

        /*
         * Returns the cumulative value of the axes over the last gamplay update frame, or the last visual update.
         */
        public float CumulativeValue()
        {
            return GetRelevantInput().Sum((visualUpdateInputs) => (visualUpdateInputs.Values.Sum()));
        }

        /*
         * Run at the end of every visual update frame to record the input state for that frame.
         */
        public void RecordUpdateState()
        {
            m_buffers.Last().Add(new Dictionary<IAxisSource, float>());

            foreach (IAxisSource source in m_sources)
            {
                m_buffers.Last().Last().Add(source, source.GetValue());
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
            m_buffers.Add(new List<Dictionary<IAxisSource, float>>());
        }

        private List<Dictionary<IAxisSource, float>> GetRelevantInput()
        {
            List<Dictionary<IAxisSource, float>> buffer = new List<Dictionary<IAxisSource, float>>();
            if (m_buffers.Last().Count == 0)
            {
                buffer.Add(m_buffers.GetRange(0, m_buffers.Count - 1).Last((gameplayUpdate) => (gameplayUpdate.Any())).Last());
            }
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