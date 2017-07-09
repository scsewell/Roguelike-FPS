using UnityEngine;

namespace Framework
{
    public abstract class PooledObject : MonoBehaviour
    {
        private ObjectPool m_pool;

        public bool IsReleased { get; set; }

        public void SetPool(ObjectPool pool)
        {
            m_pool = pool;
        }

        /// <summary>
        /// Returns this object to the pool.
        /// </summary>
        protected void Release()
        {
            m_pool.Deactivate(this);
        }
    }
}