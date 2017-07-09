using System.Collections.Generic;
using UnityEngine;
using Framework;

public class BulletManager : ComponentSingleton<BulletManager>
{
    [SerializeField]
    private Bullet m_bulletPrefab;

    private ObjectPool m_bulletPool;
    private List<Bullet> m_bullets = new List<Bullet>();
    private List<Bullet> m_bulletsTemp = new List<Bullet>();

    protected override void Awake()
    {
        base.Awake();

        m_bulletPool = new ObjectPool(m_bulletPrefab, 1);
    }

    public void FireBullet(Vector3 position, Quaternion direction, BulletSettings settings, float timeUntilNextUpdate, float additionalInaccuracy, float inaccuracyMultiplier)
    {
        float maxInaccuracy = ((settings.BaseInaccuracy + additionalInaccuracy) * inaccuracyMultiplier);

        float angle = Random.value * 2 * Mathf.PI;
        float radius = Mathf.Pow(Random.value, Mathf.Clamp(settings.Spread, 1, 32)) * maxInaccuracy;
        Vector2 inaccuracy = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        Vector3 newDirection = direction * new Vector3(inaccuracy.x, inaccuracy.y, 1);

        m_bulletPool.GetInstance(position, Quaternion.LookRotation(newDirection), null).GetComponent<Bullet>().Init(settings, timeUntilNextUpdate);
    }

    public void UpdateBullets()
    {
        m_bulletsTemp.Clear();
        m_bulletsTemp.AddRange(m_bullets);

        foreach (Bullet bullet in m_bulletsTemp)
        {
            bullet.UpdateBullet(Time.deltaTime);
        }
    }

    public void DeactivateAll()
    {
        m_bullets.Clear();
        m_bulletPool.DeactivateAll();
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