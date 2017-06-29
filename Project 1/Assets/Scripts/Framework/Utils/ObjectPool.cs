using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
    public class ObjectPool
    {
        private Queue<PooledObject> m_pool = new Queue<PooledObject>();
        private PooledObject m_prefab;
        private Transform m_poolRoot;

        public ObjectPool(PooledObject prefab, int startSize)
        {
            m_prefab = prefab;
            m_poolRoot = new GameObject(m_prefab.name + " Pool").transform;
            
            for (int i = 0; i < startSize; i++)
            {
                Deactivate(CreateInstance(Vector3.zero, Quaternion.identity, m_poolRoot));
            }
        }

        public PooledObject GetInstance(Vector3 position, Quaternion rotation, Transform parent)
        {
            PooledObject obj;
            if (m_pool.Count > 0)
            {
                obj = m_pool.Dequeue();
                obj.transform.SetParent(parent);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.gameObject.SetActive(true);
            }
            else
            {
                obj = CreateInstance(position, rotation, parent);
            }
            return obj;
        }

        private PooledObject CreateInstance(Vector3 position, Quaternion rotation, Transform parent)
        {
            PooledObject obj = Object.Instantiate(m_prefab, position, rotation, parent);
            obj.SetPool(this);
            return obj;
        }

        public void Deactivate(PooledObject pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
            pooledObject.transform.SetParent(m_poolRoot);
            m_pool.Enqueue(pooledObject);
        }

        public void ClearPool()
        {
            while (m_pool.Count > 0)
            {
                Object.Destroy(m_pool.Dequeue().gameObject);
            }
        }
    }
}