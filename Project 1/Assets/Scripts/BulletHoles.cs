using UnityEngine;
using System.Collections.Generic;

public class BulletHoles : MonoBehaviour
{
    [SerializeField] private int m_maxHoleCount = 50;
    [SerializeField] private float m_minScale = 0.025f;
    [SerializeField] private float m_maxScale = 0.03f;
    [SerializeField] private int m_fadeWait = 500;
    [SerializeField] private float m_fadeSpeed = 1f;
    [SerializeField] private float m_basePitch = 2.15f;
    [SerializeField] private float m_pitchVariation = 0.5f;
    [SerializeField] private AudioClip[] m_bulletSounds = new AudioClip[1];

    private static List<Transform> m_bulletHoles;

    private AudioSource m_audioSource;
    
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

        m_audioSource = transform.GetComponent<AudioSource>();
        m_audioSource.clip = m_bulletSounds[Random.Range(0, m_bulletSounds.Length)];
        m_audioSource.pitch = m_basePitch + (Random.value - 0.5f) * m_pitchVariation;
        m_audioSource.Play();
	}

    private void Update()
    {
        if (m_audioSource != null && !m_audioSource.isPlaying)
        {
            Destroy(m_audioSource);
        }
        /*
		if (fadeWait == 0) {
    		GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, new Color(1,1,1,0), Time.deltaTime * fadeSpeed);
    		if(GetComponent<Renderer>().material.color.a <= .05)
       			Object.Destroy (this.gameObject);
		} else {
			fadeWait--;
		}
        */
    }

    public void SetParent(Transform parent)
    {
        transform.localScale = Vector3.one * Random.Range(m_minScale, m_maxScale);
        transform.parent = parent;
    }
}
