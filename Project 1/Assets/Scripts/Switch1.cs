using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Switch1 : MonoBehaviour
{
	public bool switchOn = false;
	public Material onMaterial;
	public Material offMaterial;
	public Transform switchMesh;

	public AudioClip switchTurnOnSound;
	public AudioClip switchTurnOffSound;
	public AudioSource whileSwitchIsOnAudio;
	public AudioSource switchingAudio;

    private Interactable m_interact;
	private Animator m_anim;
	private SkinnedMeshRenderer m_mesh;

	private bool m_targetSwitchOn;
	private float m_changingSwitchTime = 0;

    private void Start() 
	{
        m_interact = GetComponent<Interactable>();
        m_anim = GetComponent<Animator>();
		m_mesh = switchMesh.GetComponent<SkinnedMeshRenderer>();

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
		else if (!switchOn && m_targetSwitchOn)
		{
			if (m_changingSwitchTime < animationInfo.length)
			{
				m_changingSwitchTime += Time.deltaTime;
			}
			else
			{
				ToggleSwitch();
			}
		}
	}

	private void Interacted(Transform interacted, Vector3 interactPoint)
	{
		m_targetSwitchOn = !m_targetSwitchOn;
		m_changingSwitchTime = 0;
	}

    private void ToggleSwitch()
	{
		switchOn = !switchOn;
		m_changingSwitchTime = 0;
		SetSwitchEffects();
		switchingAudio.Play();
	}

    private void SetSwitchEffects()
	{
		m_mesh.sharedMaterial = switchOn ? onMaterial : offMaterial;

		whileSwitchIsOnAudio.enabled = switchOn;
		switchingAudio.clip = switchOn ? switchTurnOnSound : switchTurnOffSound;
	}
}
