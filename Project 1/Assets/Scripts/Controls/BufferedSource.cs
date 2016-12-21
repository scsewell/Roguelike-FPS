﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace InputController
{
    /*
     * Manages input buffering.
     */
    public class BufferedSource<T>
    {
        protected List<ISource<T>> m_sources;
        private List<List<Dictionary<ISource<T>, T>>> m_buffers;

        private bool m_canBeMuted;
        public bool CanBeMuted
        {
            get { return m_canBeMuted; }
        }

        protected BufferedSource(bool canBeMuted, List<ISource<T>> sources)
        {
            m_canBeMuted = canBeMuted;
            m_sources = new List<ISource<T>>(sources);
            ResetBuffers();
        }

        /*
         * Initializes the buffer lists from the current sources.
         */
        public void ResetBuffers()
        {
            m_buffers = new List<List<Dictionary<ISource<T>, T>>>();
            m_buffers.Add(new List<Dictionary<ISource<T>, T>>());
            m_buffers.Last().Add(new Dictionary<ISource<T>, T>());
            foreach (ISource<T> source in m_sources)
            {
                m_buffers.Last().Last().Add(source, source.GetValue());
            }
            m_buffers.Add(new List<Dictionary<ISource<T>, T>>());
        }

        /*
         * Run at the end of every visual update frame to record the input state for that frame.
         */
        public void RecordUpdateState()
        {
            m_buffers.Last().Add(new Dictionary<ISource<T>, T>());

            foreach (ISource<T> source in m_sources)
            {
                m_buffers.Last().Last().Add(source, source.GetValue());
            }
        }

        /*
         * Run at the end of every fixed update to remove old inputs from the buffer.
         */
        public void RecordFixedUpdateState()
        {
            // ensures there are inputs from two visual updates and inputs from two fixed updates in the buffer
            while (m_buffers.GetRange(1, m_buffers.Count - 1).Sum(fixedUpdateInputs => fixedUpdateInputs.Count) >= 1)
            {
                m_buffers.RemoveAt(0);
            }
            m_buffers.Add(new List<Dictionary<ISource<T>, T>>());
        }

        /*
         * Gets all inputs since the last FixedUpdate step.
         */
        protected List<Dictionary<ISource<T>, T>> GetRelevantInput(bool includePrevious)
        {
            List<Dictionary<ISource<T>, T>> buffer = new List<Dictionary<ISource<T>, T>>();

            // Allow for forcing the last frame with useful input to be included, needed for detecting button state changes.
            // Otherwise, if we have not recieved any inputs since the last FixedUpdate, find the last FixedUpdate that does have inputs.
            if (includePrevious || m_buffers.Last().Count == 0)
            {
                buffer.Add(m_buffers.GetRange(0, m_buffers.Count - 1).Last(fixedUpdate => fixedUpdate.Any()).Last());
            }
            return buffer.Concat(m_buffers.Last()).ToList();
        }

        /*
         * Gets information about the sources for this buffer.
         */
        public List<SourceInfo> GetSourceInfo()
        {
            List<SourceInfo> sourceInfo = new List<SourceInfo>();
            foreach (ISource<T> source in m_sources)
            {
                sourceInfo.Add(new SourceInfo(source.GetSourceType(), source.GetName()));
            }
            return sourceInfo;
        }
    }
}