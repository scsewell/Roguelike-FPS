using UnityEngine;
using Framework;
using Framework.Interpolation;

public class Bullet : PooledObject
{
    private const int MAX_SEGMENTS = 10;

    private TransformInterpolator m_interpolator;
    private LineRenderer m_line;

    private BulletSettings m_settings;
    private Vector3 m_startPos;
    private Vector3 m_velocity;
    private float m_distanceTravelled;

    private Vector3[] m_positions = new Vector3[MAX_SEGMENTS];

    private void Awake()
    {
        m_interpolator = gameObject.AddComponent<TransformInterpolator>();
        m_line = GetComponent<LineRenderer>();
    }

    public void Init(BulletSettings settings, float timeUntiNextUpdate)
    {
        m_settings = settings;
        m_velocity = transform.forward * m_settings.Speed;
        m_distanceTravelled = 0;

        m_startPos = transform.position;

        if (!UpdateBullet(timeUntiNextUpdate))
        {
            m_interpolator.enabled = true;
            m_interpolator.ForgetPreviousValues();
            
            BulletManager.Instance.AddBullet(this);
        }

        m_line.enabled = true;
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;

        Vector3 lineStart = Vector3.MoveTowards(position, m_startPos, 3 * m_velocity.magnitude * Time.deltaTime);
        for (int i = 0; i < MAX_SEGMENTS; i++)
        {
            m_positions[i] = Vector3.Lerp(lineStart, position, ((float)i) / MAX_SEGMENTS);
        }
        m_line.positionCount = MAX_SEGMENTS;
        m_line.SetPositions(m_positions);
    }

    public bool UpdateBullet(float deltaTime)
    {
        // Verlet method, apply half the acceleration to the velocity before position update to remove dependancy on deltaTime 
        Vector3 deltaPos = (m_velocity + ((Physics.gravity / 2) * deltaTime)) * deltaTime;

        // raycast to see if anything was hit
        RaycastHit hit;
        bool hitCollider = Physics.Raycast(transform.position, deltaPos, out hit, deltaPos.magnitude, m_settings.HitLayers);

        if (hitCollider)
        {
            DestroyBullet();

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(m_settings.Force * deltaPos.normalized, hit.point);
            }

            bool bleed = false;

            HitboxCollider hitbox = hit.transform.GetComponentInParent<HitboxCollider>();
            if (hitbox != null)
            {
                hitbox.Damage(m_settings.Damage);

                if (hitbox.Bleed && m_settings.Blood)
                {
                    bleed = true;
                    BulletManager.Instance.Pools[m_settings.Blood].GetInstance(hit.point, Quaternion.LookRotation(hit.normal), hit.collider.transform);
                }
            }

            if (!bleed)
            {
                if (m_settings.Sparks)
                {
                    BulletManager.Instance.Pools[m_settings.Sparks].GetInstance(hit.point, Quaternion.LookRotation(hit.normal), null);
                }
                if (m_settings.BulletHole)
                {
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.AngleAxis(Random.value * 360, Vector3.up);
                    BulletManager.Instance.Pools[m_settings.BulletHole].GetInstance(hit.point, rotation, hit.collider.transform);
                }
            }
        }
        else
        {
            m_distanceTravelled += deltaPos.magnitude;
            
            if (m_distanceTravelled > m_settings.Range)
            {
                DestroyBullet();
            }

            transform.position += deltaPos;
            if (m_settings.UseGravity)
            {
                m_velocity += Physics.gravity * deltaTime;
            }
            transform.rotation = Quaternion.LookRotation(m_velocity);
        }

        return hitCollider;
    }

    private void DestroyBullet()
    {
        BulletManager.Instance.RemoveBullet(this);
        m_line.enabled = false;
        m_interpolator.enabled = false;
        Release();
    }
}
