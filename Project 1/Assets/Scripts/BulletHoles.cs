using UnityEngine;
using System.Collections.Generic;

public class BulletHoles : MonoBehaviour
{
    [SerializeField] private int m_maxHoleCount = 50;
    [SerializeField] private float m_minScale = 0.025f;
    [SerializeField] private float m_maxScale = 0.03f;
    [SerializeField] private float m_basePitch = 2.15f;
    [SerializeField] private float m_pitchVariation = 0.5f;
    [SerializeField] private AudioClip[] m_bulletSounds;

    private static List<Transform> m_bulletHoles;
    
	private void Start()
    {
        if (m_bulletHoles == null)
        {
            m_bulletHoles = new List<Transform>();
        }

        m_bulletHoles.Add(transform);
        if (m_bulletHoles.Count > m_maxHoleCount)
        {
            Destroy(m_bulletHoles[0].gameObject);
            m_bulletHoles.RemoveAt(0);
        }

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = m_bulletSounds[Random.Range(0, m_bulletSounds.Length)];
        audioSource.pitch = m_basePitch + (Random.value - 0.5f) * m_pitchVariation;
        audioSource.Play();
	}

    public void SetParent(Transform parent)
    {
        transform.localScale = Vector3.one * Random.Range(m_minScale, m_maxScale);
        transform.SetParent(parent, true);
    }
}
