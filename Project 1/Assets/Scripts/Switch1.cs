using UnityEngine;
using System.Collections;

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

	private Animator anim;
	private SkinnedMeshRenderer mesh;

	private bool targetSwitchOn;
	private float changingSwitchTime = 0;

	void Start () 
	{
		anim = GetComponent<Animator>();
		mesh = switchMesh.GetComponent<SkinnedMeshRenderer>();

		targetSwitchOn = switchOn;
		setSwitchEffects();
	}

	void Update () 
	{
		anim.SetBool("On", targetSwitchOn);
		AnimatorStateInfo animationInfo = anim.GetCurrentAnimatorStateInfo(0);

		if (switchOn && !targetSwitchOn) 
		{
			ToggleSwitch();
		} 
		else if (!switchOn && targetSwitchOn)
		{
			if (changingSwitchTime < animationInfo.length)
			{
				changingSwitchTime += Time.deltaTime;
			}
			else
			{
				ToggleSwitch();
			}
		}
	}

	public void Interact () 
	{
		targetSwitchOn = !targetSwitchOn;
		changingSwitchTime = 0;
	}

	void ToggleSwitch ()
	{
		switchOn = !switchOn;
		changingSwitchTime = 0;
		setSwitchEffects();
		switchingAudio.Play();
	}

	void setSwitchEffects ()
	{
		mesh.material = switchOn ? onMaterial : offMaterial;

		whileSwitchIsOnAudio.enabled = switchOn;
		switchingAudio.clip = switchOn ? switchTurnOnSound : switchTurnOffSound;
	}
}
