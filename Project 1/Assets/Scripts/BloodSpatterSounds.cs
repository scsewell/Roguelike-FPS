using UnityEngine;
using System.Collections;

public class BloodSpatterSounds : MonoBehaviour
{
	[SerializeField] private float m_basePitch = 1f;
	[SerializeField] private float m_pitchVariation = 0.2f;
	[SerializeField] private AudioClip[] m_bulletSounds;
	
	private AudioSource m_audioSource;
	
	private void Start()
    {
		m_audioSource = transform.GetComponent<AudioSource>();
		m_audioSource.clip = m_bulletSounds[Random.Range(0, m_bulletSounds.Length)];
		m_audioSource.pitch = m_basePitch + (Random.value - 0.5f) * m_pitchVariation;
		m_audioSource.Play();
	}
}