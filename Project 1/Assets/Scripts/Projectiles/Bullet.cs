using UnityEngine;
using Framework;
using Framework.Interpolation;

public class Bullet : PooledObject
{
    private TransformInterpolator m_interpolator;
    private TrailRenderer m_line;
    private BulletSettings m_settings;
    private Vector3 m_velocity;
    private float m_distanceTravelled;

    private void Awake()
    {
        m_interpolator = gameObject.AddComponent<TransformInterpolator>();
        m_line = GetComponent<TrailRenderer>();
    }

    public void Init(BulletSettings settings, float timeUntiNextUpdate)
    {
        m_settings = settings;
        m_velocity = transform.forward * m_settings.Speed;
        m_distanceTravelled = 0;

        m_line.Clear();
        m_interpolator.enabled = true;
        m_interpolator.ForgetPreviousValues();

        BulletManager.Instance.AddBullet(this);

        UpdateBullet(timeUntiNextUpdate);
    }

    public void UpdateBullet(float timeUntiNextUpdate)
    {
        // Verlet method, apply half the acceleration to the velocity before position update to remove dependancy on deltaTime 
        Vector3 deltaPos = (m_velocity + ((Physics.gravity / 2) * timeUntiNextUpdate)) * timeUntiNextUpdate;

        // raycast to see if anything was hit
        RaycastHit hit;
        if (Physics.Raycast(transform.position, deltaPos, out hit, deltaPos.magnitude, m_settings.HitLayers))
        {
            DestroyBullet();

            HitboxCollider hitbox = hit.transform.GetComponentInParent<HitboxCollider>();
            if (hitbox != null)
            {
                hitbox.Damage(m_settings.Damage);
            }

            if (hit.rigidbody)
            {
                hit.rigidbody.AddForceAtPosition(m_settings.Force * deltaPos.normalized, hit.point);
            }

            if (hit.transform.tag == "Bleed")
            {
                if (m_settings.Blood)
                {
                    ParticleSystem blood = Instantiate(m_settings.Blood, hit.point, Quaternion.LookRotation(hit.normal)) as ParticleSystem;
                    blood.transform.parent = hit.collider.transform;
                }
            }
            else
            {
                if (m_settings.Sparks)
                {
                    Instantiate(m_settings.Sparks, hit.point, Quaternion.LookRotation(hit.normal));
                }
                if (m_settings.BulletHole)
                {
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.back, hit.normal) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
                    Decal hole = Instantiate(m_settings.BulletHole, hit.point, rotation).GetComponent<Decal>();
                    hole.GetComponent<BulletHoles>().SetParent(hit.collider.transform);
                    hole.BuildDecal(hit.transform.gameObject);
                }
            }
        }
        else
        {
            m_distanceTravelled += deltaPos.magnitude;

            if (m_distanceTravelled > 1)
            {
                m_line.enabled = true;
            }

            if (m_distanceTravelled > m_settings.Range)
            {
                DestroyBullet();
            }

            transform.position += deltaPos;
            m_velocity += Physics.gravity * timeUntiNextUpdate;
            transform.rotation = Quaternion.LookRotation(m_velocity);
        }
    }

    private void DestroyBullet()
    {
        BulletManager.Instance.RemoveBullet(this);
        m_line.enabled = false;
        m_interpolator.enabled = false;
        Release();
    }
}
