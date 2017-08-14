using UnityEngine;
using Framework;

public class BloodSpatterSounds : MonoBehaviour
{
	[SerializeField] private float m_basePitch = 1f;
	[SerializeField] private float m_pitchVariation = 0.2f;
	[SerializeField] private AudioClip[] m_bulletSounds;
	
	private AudioSource m_audioSource;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
		m_audioSource.clip = Utils.PickRandom(m_bulletSounds);
		m_audioSource.pitch = m_basePitch + (Random.value - 0.5f) * m_pitchVariation;
		m_audioSource.Play();
	}
}