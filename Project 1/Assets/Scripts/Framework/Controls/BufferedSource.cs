using System.Collections.Generic;
using System.Linq;

namespace Framework.InputManagement
{
    /// <summary>
    /// Manages input buffering.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    public class BufferedSource<T> : IInputSource
    {
        private ISource<T>[] m_defaultSources;

        protected List<ISource<T>> m_sources;
        public List<ISource<T>> Sources
        {
            get { return m_sources; }
            set
            {
                if (m_canRebind)
                {
                    m_sources = value;
                    ResetBuffers();
                }
            }
        }

        protected bool m_canBeMuted;
        public bool CanBeMuted
        {
            get { return m_canBeMuted; }
        }

        private bool m_canRebind;
        public bool CanRebind
        {
            get { return m_canRebind; }
        }

        private string m_displayName;
        public string DisplayName
        {
            get { return m_displayName; }
        }

        private List<SourceInfo> m_sourceInfos;
        public List<SourceInfo> SourceInfos
        {
            get { return m_sourceInfos; }
        }

        private List<List<T>[]> m_buffer;
        private List<List<T>> m_relevantInput;

        protected BufferedSource(string displayName, bool canRebind, bool canBeMuted, ISource<T>[] defaultSources)
        {
            m_displayName = displayName;
            m_canRebind = canRebind;
            m_canBeMuted = canBeMuted;
            m_defaultSources = defaultSources != null ? defaultSources : new ISource<T>[0];

            m_buffer = new List<List<T>[]>();
            m_relevantInput = new List<List<T>>();
            m_sourceInfos = new List<SourceInfo>();

            UseDefaults();
        }

        /*
         * Loads the default sources.
         */
        public void UseDefaults()
        {
            m_sources = new List<ISource<T>>(m_defaultSources);
            ResetBuffers();
        }

        /*
         * Initializes the buffer lists from the current sources.
         */
        public void ResetBuffers()
        {
            m_sources = m_sources.OrderBy(s => (int)s.SourceInfo.SourceType).ToList();

            m_buffer.Clear();
            m_relevantInput.Clear();
            m_sourceInfos.Clear();

            foreach (ISource<T> source in m_sources)
            {
                List<T>[] fixedFrames = new List<T>[2];
                fixedFrames[0] = new List<T>();
                fixedFrames[1] = new List<T>();
                m_buffer.Add(fixedFrames);

                m_relevantInput.Add(new List<T>());

                m_sourceInfos.Add(source.SourceInfo);
            }
        }

        /*
         * Clears out any unprocessed input frames.
         */
        public void ClearBuffers()
        {
            for (int i = 0; i < m_sources.Count; i++)
            {
                m_buffer[i][0].Clear();
                m_buffer[i][1].Clear();
                m_relevantInput[i].Clear();
            }
        }

        /*
         * Run at the end of every input frame to record the input state for that frame.
         */
        public void RecordUpdateState(bool muting)
        {
            if (!(muting && m_canBeMuted))
            {
                for (int i = 0; i < m_sources.Count; i++)
                {
                    m_buffer[i][1].Add(m_sources[i].GetValue());
                }
            }
        }

        /*
         * Run at the end of every fixed update cycle to remove old input frames from the buffer.
         * Ensures the last fixed update with input frames is in the buffer along with an empty frame for new inputs.
         */
        public void RecordFixedUpdateState()
        {
            foreach (List<T>[] source in m_buffer)
            {
                if (source[1].Count > 0)
                {
                    List<T> temp = source[0];
                    source[0] = source[1];
                    source[1] = temp;
                    source[1].Clear();
                }
            }
        }

        /*
         * Gets all inputs since the last FixedUpdate step.
         * Allow for adding the last input frame of the previous FixedUpdate to be included, needed for detecting button state changes.
         */
        protected List<List<T>> GetRelevantInput(bool includePrevious)
        {
            for (int i = 0; i < m_sources.Count; i++)
            {
                m_relevantInput[i].Clear();
                if (includePrevious && m_buffer[i][0].Count > 0)
                {
                    m_relevantInput[i].Add(m_buffer[i][0].Last());
                }
                m_relevantInput[i].AddRange(m_buffer[i][1]);
            }
            return m_relevantInput;
        }

        /*
         * Adds a new source and resets the buffer.
         */
        public void AddSource(ISource<T> source)
        {
            if (m_canRebind && !Contains(source))
            {
                m_sources.Add(source);
                ResetBuffers();
            }
        }

        /*
         * Removes a source and resets the buffer.
         */
        public void RemoveSource(int index)
        {
            if (m_canRebind)
            {
                m_sources.RemoveAt(index);
                ResetBuffers();
            }
        }

        /*
         * Checks if a source is already used in the buffer.
         */
        private bool Contains(ISource<T> source)
        {
            return m_sourceInfos.Any(s => s == source.SourceInfo);
        }
    }
}