using UnityEngine;
using Framework;

public class MainGunSounds : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_audioSource;

    [SerializeField]
    private AudioClip m_reloadStart;
    [SerializeField]
    private AudioClip m_reloadEnd;
    [SerializeField] [Range(0, 1)]
    private float m_reloadVolume = 1f;

    [SerializeField]
    private AudioClip[] m_bulletSounds;
	[SerializeField] [Range(0, 1)]
    private float m_bulletFireVolume = 1f;

    [SerializeField]
    private AudioClip m_dryFireSound;
    [SerializeField] [Range(0, 1)]
    private float m_dryFireVolume = 1f;

    [SerializeField]
    private AudioClip[] m_fireModeSwitchSounds;
    [SerializeField] [Range(0, 1)]
    private float m_fireModeVolume = 1f;

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

    public void PlayDryFireSound()
    {
        m_audioSource.PlayOneShot(m_dryFireSound, m_dryFireVolume);
    }

    public void PlayFireModeSound()
    {
        m_audioSource.PlayOneShot(Utils.PickRandom(m_fireModeSwitchSounds), m_bulletFireVolume);
    }
}
