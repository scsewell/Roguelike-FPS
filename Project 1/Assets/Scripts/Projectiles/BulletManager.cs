using System.Collections.Generic;
using UnityEngine;
using Framework;

public class BulletManager : Singleton<BulletManager>
{
    private Dictionary<Bullet, ObjectPool> m_bulletPools = new Dictionary<Bullet, ObjectPool>();

    private List<Bullet> m_bullets = new List<Bullet>();
    private List<Bullet> m_bulletsTemp = new List<Bullet>();
    
    public void InitBulletType(BulletSettings settings)
    {
        if (!m_bulletPools.ContainsKey(settings.BulletPrefab))
        {
            m_bulletPools.Add(settings.BulletPrefab, new ObjectPool(settings.BulletPrefab, 20));
        }
    }

    public void FireBullet(Vector3 position, Quaternion direction, BulletSettings settings, float timeUntilNextUpdate, float additionalInaccuracy, float inaccuracyMultiplier)
    {
        float maxInaccuracy = ((settings.BaseInaccuracy + additionalInaccuracy) * inaccuracyMultiplier);

        float angle = Random.value * 2 * Mathf.PI;
        float radius = Mathf.Pow(Random.value, Mathf.Clamp(settings.Spread, 1, 32)) * maxInaccuracy;
        Vector2 inaccuracy = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        Vector3 newDirection = direction * new Vector3(inaccuracy.x, inaccuracy.y, 1);

        ObjectPool pool = m_bulletPools[settings.BulletPrefab];
        pool.GetInstance(position, Quaternion.LookRotation(newDirection), null).GetComponent<Bullet>().Init(settings, timeUntilNextUpdate);
    }

    public void UpdateBullets()
    {
        m_bulletsTemp.Clear();
        m_bulletsTemp.AddRange(m_bullets);

        float deltaTime = Time.deltaTime;
        foreach (Bullet bullet in m_bulletsTemp)
        {
            bullet.UpdateBullet(deltaTime);
        }
    }

    public void Clear()
    {
        m_bullets.Clear();
        foreach (KeyValuePair<Bullet, ObjectPool> bulletPool in m_bulletPools)
        {
            bulletPool.Value.ClearPool();
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