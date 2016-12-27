using UnityEngine;

public class MainGunSounds : MonoBehaviour
{
	[SerializeField] private float m_reloadPitch = 1f;
	[SerializeField] private float m_reloadVolume = 1f;
	[SerializeField] private float m_bulletFirePitch = 1f;
	[SerializeField] private float m_bulletFireVolume = 1f;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_reloadStart;
    [SerializeField] private AudioClip m_reloadEnd;
    [SerializeField] private AudioClip[] m_bulletSounds;
	
    private FootstepSounds m_footsteps;
	
	private void Start()
    {
        m_footsteps = transform.root.GetComponent<FootstepSounds>();
    }

    public void Footstep()
    {
        m_footsteps.FootstepSound();
    }

    public void PlayReloadStart()
    {
        m_audioSource.clip = m_reloadStart;
        m_audioSource.pitch = m_reloadPitch;
        m_audioSource.volume = m_reloadVolume;
        m_audioSource.Play();
    }

    public void PlayReloadEnd()
    {
        m_audioSource.clip = m_reloadEnd;
        m_audioSource.pitch = m_reloadPitch;
        m_audioSource.volume = m_reloadVolume;
        m_audioSource.Play();
    }

    public void PlayFireSound()
    {
		m_audioSource.clip = m_bulletSounds[Random.Range(0, m_bulletSounds.Length)];
		m_audioSource.pitch = m_bulletFirePitch;
		m_audioSource.volume = m_bulletFireVolume;
		m_audioSource.Play();
    }
}
