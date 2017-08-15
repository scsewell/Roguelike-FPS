using UnityEngine;
using Framework;

public class ImpactEffect : PooledObject
{
    [SerializeField]
    private AudioClip[] m_bulletSounds;
    [SerializeField] [Range(0, 3)]
    private float m_basePitch = 1f;
	[SerializeField] [Range(0, 1)]
    private float m_pitchVariation = 0.2f;
	
	private AudioSource m_audio;
    private ParticleSystem m_particles;

    private void Awake()
    {
        m_audio = GetComponent<AudioSource>();
        m_particles = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        if (m_audio != null)
        {
            m_audio.clip = Utils.PickRandom(m_bulletSounds);
            m_audio.pitch = Mathf.Max(m_basePitch + (Random.value - 0.5f) * m_pitchVariation, 0.1f);
            m_audio.Play();
        }

        if (m_particles != null)
        {
            m_particles.Play();
        }
    }

    private void Update()
    {
        if ((m_audio == null || !m_audio.isPlaying) && (m_particles == null || !m_particles.IsAlive()))
        {
            Release();
        }
    }
}