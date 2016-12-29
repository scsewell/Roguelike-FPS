using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Switch1 : MonoBehaviour
{
	public bool switchOn = false;
	public Material onMaterial;
	public Material offMaterial;
	public Renderer switchRenderer;

	public AudioClip switchTurnOnSound;
	public AudioClip switchTurnOffSound;
	public AudioSource whileSwitchIsOnAudio;
	public AudioSource switchingAudio;

    private Interactable m_interact;
	private Animator m_anim;

	private bool m_targetSwitchOn;

    private void Start() 
	{
        m_interact = GetComponent<Interactable>();
        m_anim = GetComponent<Animator>();

        m_interact.Interacted += Interacted;

        m_targetSwitchOn = switchOn;
		SetSwitchEffects();
	}

    private void OnDestroy()
    {
        m_interact.Interacted -= Interacted;
    }

    private void Update() 
	{
		m_anim.SetBool("On", m_targetSwitchOn);
		AnimatorStateInfo animationInfo = m_anim.GetCurrentAnimatorStateInfo(0);

		if (switchOn && !m_targetSwitchOn) 
		{
			ToggleSwitch();
		} 
		else if (!switchOn && m_targetSwitchOn && animationInfo.IsTag("On"))
		{
			ToggleSwitch();
		}
	}

	private void Interacted(Transform interacted, Vector3 interactPoint)
	{
		m_targetSwitchOn = !m_targetSwitchOn;
	}

    private void ToggleSwitch()
	{
		switchOn = !switchOn;
		SetSwitchEffects();
		switchingAudio.Play();
	}

    private void SetSwitchEffects()
	{
        switchRenderer.sharedMaterial = switchOn ? onMaterial : offMaterial;

		whileSwitchIsOnAudio.enabled = switchOn;
		switchingAudio.clip = switchOn ? switchTurnOnSound : switchTurnOffSound;
	}
}
