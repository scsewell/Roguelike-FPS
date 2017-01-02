using UnityEngine;

public class AmbientSounds : MonoBehaviour
{
    public float ambientSoundChance = 5.0f;
    public float ambientSoundMinVolume = 0;
    public float ambientSoundMaxVolume = 1.0f;
    public AudioSource ambientAudio;
    public AudioClip[] ambientSounds;

	private void Update()
    {
		if (Random.Range(0, ambientSoundChance) < Time.deltaTime && !ambientAudio.isPlaying)
        {
			ambientAudio.clip = Utils.PickRandom(ambientSounds);
            ambientAudio.panStereo = Random.Range(0, 2) - 1;
			ambientAudio.volume = Random.Range(ambientSoundMinVolume, ambientSoundMaxVolume);
			ambientAudio.Play();
		}
	}
}
