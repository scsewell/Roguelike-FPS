using UnityEngine;
using Framework;

public class ImpuseSound : MonoBehaviour
{
    public float triggerImpuse = 1;
    public float volume = 1;
    public AudioClip[] clips;

    private AudioSource m_audio;
    
	private void Awake()
    {
        m_audio = gameObject.AddComponent<AudioSource>();
        m_audio.spatialBlend = 1;
        m_audio.rolloffMode = AudioRolloffMode.Custom;
        m_audio.maxDistance = 5;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, 1, -2f, -2f));
        curve.AddKey(new Keyframe(1, 0, 0, 0));
        m_audio.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > triggerImpuse)
        {
            m_audio.volume = volume;
            m_audio.clip = Utils.PickRandom(clips);
            m_audio.Play();
        }
    }
}