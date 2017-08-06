using UnityEngine;
using Framework;

public class MainGunSounds : MonoBehaviour
{
	[SerializeField]
    private float m_reloadVolume = 1f;
	[SerializeField]
    private float m_bulletFireVolume = 1f;

    [SerializeField]
    private AudioSource m_audioSource;
    [SerializeField]
    private AudioClip m_reloadStart;
    [SerializeField]
    private AudioClip m_reloadEnd;
    [SerializeField]
    private AudioClip[] m_bulletSounds;
	
    public void PlayReloadStart()
    {
        m_audioSource.PlayOneShot(m_reloadStart, m_reloadVolume);
    }

    public void PlayReloadEnd()
    {
        m_audioSource.PlayOneShot(m_reloadEnd, m_reloadVolume);
    }

    public void PlayFireSound()
    {
        m_audioSource.PlayOneShot(Utils.PickRandom(m_bulletSounds), m_bulletFireVolume);
    }
}
