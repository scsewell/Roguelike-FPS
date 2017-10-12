using UnityEngine;

public class Flashlight : Prop
{
    [SerializeField]
    private Transform m_flashlightRig;
    [SerializeField]
    private Transform m_flashlight;
    
    [SerializeField]
    private AudioClip m_turnOn;
    [SerializeField]
    private AudioClip m_turnOff;
    
    private Animator m_anim;
    private Material m_flashlightMat;
    private Light m_light;
    private AudioSource m_audio;
    private bool m_on = true;

    public override GameButton HolsterButton { get { return GameButton.Flashlight; } }
    protected override int MainAnimatorState { get { return 0; } }

    private void Awake()
    {
        m_flashlightMat = GetComponentInChildren<MeshRenderer>().material;
        m_light = GetComponentInChildren<Light>();
        m_anim = GetComponentInChildren<Animator>();
        m_audio = GetComponent<AudioSource>();
    }
    
    protected override void AnimUpdate()
    {
        AnimatorStateInfo state = m_anim.GetCurrentAnimatorStateInfo(2);
        m_light.enabled = state.IsTag("On") && state.normalizedTime >= 0.5f;
        m_flashlightMat.SetColor("_EmissionColor", m_light.enabled ? m_light.color * 2 : Color.black);
        
        m_anim.SetBool("On", m_on);
    }

    private void LateUpdate()
    {
        m_flashlightRig.position = m_flashlight.position;
    }

    protected override void OnFireStart(bool fireInputStart)
    {
        if (fireInputStart)
        {
            m_on = !m_on;
            m_audio.PlayOneShot(m_on ? m_turnOn : m_turnOff);
        }
    }
}
