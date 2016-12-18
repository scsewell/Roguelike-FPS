using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_footsteps;

    private CharacterMovement m_character;
	private AudioSource m_audioSource;

    private void Start()
    {
        m_character = GetComponent<CharacterMovement>();
        m_audioSource = GetComponent<AudioSource>();
    }

    public void FootstepSound()
    {
        if (m_character.grounded && !m_audioSource.isPlaying)
        {
            m_audioSource.clip = m_footsteps[Random.Range(0, m_footsteps.Length)];
            m_audioSource.Play();
        }
    }
}