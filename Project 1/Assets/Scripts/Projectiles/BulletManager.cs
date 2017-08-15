using System.Collections.Generic;
using UnityEngine;
using Framework;

public class BulletManager : Singleton<BulletManager>
{
    private Dictionary<Object, ObjectPool> m_pools = new Dictionary<Object, ObjectPool>();
    public Dictionary<Object, ObjectPool> Pools
    {
        get { return m_pools; }
    }
    
    private List<Bullet> m_bullets = new List<Bullet>();
    private List<Bullet> m_bulletsTemp = new List<Bullet>();
    
    public void InitBulletType(BulletSettings settings)
    {
        if (!m_pools.ContainsKey(settings.BulletPrefab))
        {
            int instanceCount = 20;

            m_pools.Add(settings.BulletPrefab, new ObjectPool(settings.BulletPrefab, instanceCount));

            if (settings.Blood != null)
            {
                m_pools.Add(settings.Blood, new ObjectPool(settings.Blood, instanceCount));
            }
            if (settings.Sparks != null)
            {
                m_pools.Add(settings.Sparks, new ObjectPool(settings.Sparks, instanceCount));
            }
            if (settings.BulletHole != null)
            {
                m_pools.Add(settings.BulletHole, new ObjectPool(settings.BulletHole, instanceCount));
            }
        }
    }

    public void FireBullet(Vector3 position, Quaternion direction, BulletSettings settings, float timeUntilNextUpdate, float additionalInaccuracy, float inaccuracyMultiplier)
    {
        float maxInaccuracy = ((settings.BaseInaccuracy + additionalInaccuracy) * inaccuracyMultiplier);

        float angle = Random.value * 2 * Mathf.PI;
        float radius = Mathf.Pow(Random.value, Mathf.Clamp(settings.Spread, 1, 32)) * maxInaccuracy;
        Vector2 inaccuracy = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        Vector3 newDirection = direction * new Vector3(inaccuracy.x, inaccuracy.y, 1);

        ObjectPool pool = m_pools[settings.BulletPrefab];
        pool.GetInstance(position, Quaternion.LookRotation(newDirection), null).GetComponent<Bullet>().Init(settings, timeUntilNextUpdate);
    }

    public void UpdateBullets()
    {
        m_bulletsTemp.Clear();

        for (int i = 0; i < m_bullets.Count; i++)
        {
            m_bulletsTemp.Add(m_bullets[i]);
        }

        float deltaTime = Time.deltaTime;
        foreach (Bullet bullet in m_bulletsTemp)
        {
            bullet.UpdateBullet(deltaTime);
        }
    }

    public void Clear()
    {
        m_bullets.Clear();
        foreach (KeyValuePair<Object, ObjectPool> bulletPool in m_pools)
        {
            bulletPool.Value.ClearPool();
        }
    }
    
    public void Reinitalize()
    {
        foreach (KeyValuePair<Object, ObjectPool> bulletPool in m_pools)
        {
            bulletPool.Value.Initialize();
        }
    }

    public void AddBullet(Bullet bullet)
    {
        m_bullets.Add(bullet);
    }

    public void RemoveBullet(Bullet bullet)
    {
        m_bullets.Remove(bullet);
    }
}