using UnityEngine;
using System.Collections.Generic;

namespace Framework.Interpolation
{
    public class InterpolationController : ComponentSingleton<InterpolationController>
    {
        private List<IInterpolator> m_interpolators = new List<IInterpolator>();
        private float m_lastFixedTime;
        
        private void FixedUpdate()
        {
            m_lastFixedTime = Time.time;

            foreach (IInterpolator component in m_interpolators)
            {
                component.FixedFrame();
            }
        }

        private void Update()
        {
            float factor = (Time.time - m_lastFixedTime) / Time.fixedDeltaTime;

            foreach (IInterpolator interpolator in m_interpolators)
            {
                interpolator.UpdateFrame(factor);
            }
        }

        public void AddInterpolator(IInterpolator interpolator)
        {
            if (!m_interpolators.Contains(interpolator))
            {
                m_interpolators.Add(interpolator);
            }
        }

        public void RemoveInterpolator(IInterpolator interpolator)
        {
            m_interpolators.Remove(interpolator);
        }
    }
}