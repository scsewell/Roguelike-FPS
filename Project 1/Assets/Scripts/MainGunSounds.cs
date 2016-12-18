using UnityEngine;
using System.Collections;

public class MainGunSounds : MonoBehaviour
{
	[SerializeField] private float m_reloadPitch;
	[SerializeField] private float m_reloadVolume;
	[SerializeField] private float m_bulletFirePitch;
	[SerializeField] private float m_bulletFireVolume;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_reload;
    [SerializeField] private AudioClip[] m_bulletSounds;
	
    private MainGun m_gun;
    private FootstepSounds m_footsteps;
	
	private void Start()
    {
		m_gun = GetComponent<MainGun>();
        m_footsteps = transform.root.GetComponent<FootstepSounds>();

    }

    public void Footstep()
    {
        m_footsteps.FootstepSound();
    }

    public void Reload()
    {
        m_audioSource.clip = m_reload;
        m_audioSource.pitch = m_reloadPitch;
        m_audioSource.volume = m_reloadVolume;
        m_audioSource.Play();
    }

    public void Recoil()
    {
		m_audioSource.clip = m_bulletSounds[Random.Range(0, m_bulletSounds.Length)];
		m_audioSource.pitch = m_bulletFirePitch;
		m_audioSource.volume = m_bulletFireVolume;
		m_audioSource.Play();
    }

    public void ReloadCanceled()
    {
        m_audioSource.Stop();
    }
}
