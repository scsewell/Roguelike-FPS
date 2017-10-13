using UnityEngine;
using Framework;

public class BulletHoles : PooledObject
{
    [SerializeField]
    private float m_glowIntensity = 10.0f;
    [SerializeField] [Range(0, 4)]
    private float m_glowFadeTime = 0.35f;
    [SerializeField]
    private float m_minScale = 0.025f;
    [SerializeField]
    private float m_maxScale = 0.03f;
    
    private Decal m_decal;
    private float m_startTime;

    private void Awake()
    {
        m_decal = GetComponent<Decal>();
    }

    private void OnEnable()
    {
        m_startTime = Time.time;
        
        Vector3 scale = Vector3.one;
        if (transform.parent != null)
        {
            m_decal.LimitTo = transform.parent.gameObject;
            scale = transform.parent.lossyScale;
        }
        transform.localScale = Random.Range(m_minScale, m_maxScale) * new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);
    }

    private void LateUpdate()
    {
        if (m_glowFadeTime > 0)
        {
            float val = 1 - Mathf.Clamp01((Time.time - m_startTime) / m_glowFadeTime);
            m_decal.EmissionIntensity = m_glowIntensity * val * val * val;
        }
        else
        {
            m_decal.EmissionIntensity = 0;
        }
    }
}
