using UnityEngine;
using Framework;

public class FootstepSounds : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_leftFoot;
    [SerializeField]
    private AudioSource m_rightFoot;
    [SerializeField]
    private AudioClip[] m_footsteps;

    [SerializeField] [Range(0, 1)]
    private float m_volume = 1.0f;
    [SerializeField] [Range(0, 1)]
    private float m_crouchVolume = 0.25f;

    private CharacterMovement m_character;

    private void Awake()
    {
        m_character = GetComponent<CharacterMovement>();
    }

    public void FootstepL()
    {
        if (m_character.IsGrounded)
        {
            float volume = (m_character.IsCrouching ? m_crouchVolume : m_volume);
            m_leftFoot.PlayOneShot(Utils.PickRandom(m_footsteps), volume);
        }
    }

    public void FootstepR()
    {
        if (m_character.IsGrounded)
        {
            float volume = (m_character.IsCrouching ? m_crouchVolume : m_volume);
            m_rightFoot.PlayOneShot(Utils.PickRandom(m_footsteps), volume);
        }
    }
}