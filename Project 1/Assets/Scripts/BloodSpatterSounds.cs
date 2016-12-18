using UnityEngine;
using System.Collections;

public class BloodSpatterSounds : MonoBehaviour
{
	public AudioClip[] bulletSounds = new AudioClip[1];
	public float basePitch = 2f;
	public float pitchVariation = 0.5f;
	
	AudioSource m_audioSource;
	
	private void Start()
    {
		m_audioSource = transform.GetComponent<AudioSource>();
		m_audioSource.clip = bulletSounds[Random.Range(0, bulletSounds.Length)];
		m_audioSource.pitch = basePitch + Random.value * pitchVariation;
		m_audioSource.Play();
	}
}