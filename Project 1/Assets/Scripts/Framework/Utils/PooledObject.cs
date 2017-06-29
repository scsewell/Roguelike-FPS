using UnityEngine;

namespace Framework
{
    public abstract class PooledObject : MonoBehaviour
    {
        private ObjectPool m_pool;

        public void SetPool(ObjectPool pool)
        {
            m_pool = pool;
        }

        protected void OnDestroy()
        {
            m_pool.Deactivate(this);
        }
    }
}