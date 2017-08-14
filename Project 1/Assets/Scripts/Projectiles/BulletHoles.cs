using UnityEngine;

public class BulletHoles : MonoBehaviour
{
    [SerializeField]
    private float m_glowIntensity = 10.0f;
    [SerializeField]
    [Range(0, 16)]
    private float m_glowfadeTime = 0.2f;

    [SerializeField] private int m_maxHoleCount = 50;
    [SerializeField] private float m_minScale = 0.025f;
    [SerializeField] private float m_maxScale = 0.03f;
    [SerializeField] private AudioClip[] m_bulletSounds;
    [SerializeField] private float m_basePitch = 2.15f;
    [SerializeField] private float m_pitchVariation = 0.5f;
    
    private AudioSource m_audio;
    private Decal m_decal;
    private float m_startTime;

    private void Awake()
    {
        m_audio = GetComponent<AudioSource>();
        m_decal = GetComponent<Decal>();
    }

    private void OnEnable()
    {
        m_startTime = Time.time;

        m_audio.enabled = true;
        m_audio.clip = m_bulletSounds[Random.Range(0, m_bulletSounds.Length)];
        m_audio.pitch = m_basePitch + (Random.value - 0.5f) * m_pitchVariation;
        m_audio.Play();
	}

    private void LateUpdate()
    {
        if (!m_audio.isPlaying)
        {
            m_audio.enabled = false;
        }

        if (m_glowfadeTime > 0)
        {
            float val = 1 - Mathf.Clamp01((Time.time - m_startTime) / m_glowfadeTime);
            m_decal.EmissionIntensity = m_glowIntensity * val * val * val;
        }
        else
        {
            m_decal.EmissionIntensity = 0;
        }
    }

    public void SetParent(Transform parent)
    {
        transform.localScale = Vector3.one * Random.Range(m_minScale, m_maxScale);
        transform.SetParent(parent, true);
        m_decal.LimitTo = parent.gameObject;
    }
}
