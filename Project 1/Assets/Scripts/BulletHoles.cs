using UnityEngine;
using System.Collections;

public class BulletHoles : MonoBehaviour
{
    public float minScale = 0.5f;
    public float maxScale = 1.0f;

    public int fadeWait = 120;
    public float fadeSpeed = 1f;
    public float basePitch = 2f;
    public float pitchVariation = 0.5f;
    public AudioClip[] bulletSounds = new AudioClip[1];

    AudioSource m_audioSource;
    
	private void Start()
    {
        m_audioSource = transform.GetComponent<AudioSource>();
        m_audioSource.clip = bulletSounds[Random.Range(0, bulletSounds.Length)];
        m_audioSource.pitch = basePitch + Random.value * pitchVariation;
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
        transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
        transform.parent = parent;
    }
}
