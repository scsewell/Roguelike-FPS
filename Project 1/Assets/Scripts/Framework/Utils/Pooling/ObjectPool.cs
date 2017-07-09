using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework
{
    public class ObjectPool
    {
        private List<PooledObject> m_held = new List<PooledObject>();
        private List<PooledObject> m_released = new List<PooledObject>();
        private PooledObject m_prefab;
        private Transform m_poolRoot;

        public ObjectPool(PooledObject prefab, int startSize)
        {
            m_prefab = prefab;
            m_poolRoot = new GameObject(m_prefab.name + " Pool").transform;
            Object.DontDestroyOnLoad(m_poolRoot.gameObject);

            for (int i = 0; i < startSize; i++)
            {
                Deactivate(CreateInstance(Vector3.zero, Quaternion.identity, m_poolRoot));
            }
        }

        public PooledObject GetInstance(Vector3 position, Quaternion rotation, Transform parent)
        {
            PooledObject obj;
            if (m_held.Count > 0)
            {
                obj = m_held.First();
                m_held.RemoveAt(0);

                obj.transform.SetParent(parent);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            else
            {
                obj = CreateInstance(position, rotation, parent);
            }
            m_released.Add(obj);
            obj.gameObject.SetActive(true);
            obj.IsReleased = true;
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
            if (pooledObject.IsReleased)
            {
                pooledObject.gameObject.SetActive(false);
                pooledObject.transform.SetParent(m_poolRoot);
                m_held.Add(pooledObject);
                m_released.Remove(pooledObject);
            }
        }

        public void ClearPool()
        {
            while (m_held.Count > 0)
            {
                PooledObject obj = m_held.First();
                m_held.RemoveAt(0);
                Object.Destroy(obj.gameObject);
            }
        }

        public void DeactivateAll()
        {
            foreach(PooledObject pooledObject in new List<PooledObject>(m_released))
            {
                Deactivate(pooledObject);
            }
        }
    }
}