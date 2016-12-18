using UnityEngine;
using System.Collections;

public class AmbientSounds : MonoBehaviour
{
    public float ambientSoundChance = 5.0f;
    public float ambientSoundMinVolume = 0;
    public float ambientSoundMaxVolume = 1.0f;
    public AudioSource ambientAudio;
    public AudioClip[] ambientSounds;

	void Update ()
    {
		if (Random.Range(0, ambientSoundChance) < Time.deltaTime && !ambientAudio.isPlaying)
        {
			ambientAudio.clip = (AudioClip)ambientSounds.GetValue(Random.Range(0, ambientSounds.Length));
			ambientAudio.panStereo = (Random.Range(0, 2) - 1);
			ambientAudio.volume = Random.Range(ambientSoundMinVolume, ambientSoundMaxVolume);
			ambientAudio.Play();
		}
	}
}
