using System.Linq;
using UnityEngine;
using Framework;

public class ImpulseSound : MonoBehaviour
{
    [SerializeField] [Range(0, 10)]
    private float m_triggerImpulse = 1.0f;
    
    [SerializeField] [Range(0, 1)]
    private float m_volume = 1.0f;

    [SerializeField]
    private AudioClip[] clips;

    private AudioSource m_audio;
    
	private void Awake()
    {
        foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>(true))
        {
            body.gameObject.AddComponent<RigidbodyCollisionListener>().OnCollision += ImpuseSound_OnCollision;
        }
        
        m_audio = new GameObject("ImpulseSound").AddComponent<AudioSource>();
        m_audio.spatialBlend = 1;
        m_audio.rolloffMode = AudioRolloffMode.Custom;
        m_audio.maxDistance = 5;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, 1, -2f, -2f));
        curve.AddKey(new Keyframe(1, 0, 0, 0));
        m_audio.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
    }

    private void OnDestroy()
    {
        if (m_audio != null)
        {
            Destroy(m_audio.gameObject);
        }
    }

    private void ImpuseSound_OnCollision(Collision collision)
    {
        if (collision.impulse.magnitude > m_triggerImpulse)
        {
            m_audio.transform.position = collision.contacts.First().point;
            m_audio.volume = m_volume;
            m_audio.clip = Utils.PickRandom(clips);
            m_audio.Play();
        }
    }
}