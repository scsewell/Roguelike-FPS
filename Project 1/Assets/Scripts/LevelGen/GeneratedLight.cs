using UnityEngine;

public class GeneratedLight : MonoBehaviour
{
    [SerializeField]
    private LightProfile[] m_profiles;
    [SerializeField]
    private Renderer m_emissionSource;
    [SerializeField]
    private Light m_light;

    public enum LightMode
    {
        Static, Sine, RandomFluctuation, Flicker,
    }
    
	public float lightDeactivateDistance = 40;
	
    private LightProfile m_profile;
	private Transform m_player;
	private float m_targetIntensity;
	private bool m_isFlickering;
	private float m_flickerEndTime;
	
	private void Start() 
    {
        m_profile = Utils.PickRandom(m_profiles);
        m_player = GameObject.FindGameObjectWithTag("Player").transform;

        m_light.color = Utils.PickRandom(m_profile.colors);
        m_targetIntensity = m_profile.maxIntensity;
	}

	private void Update()
    {
        m_light.enabled = Vector3.Distance(m_player.position, m_light.transform.position) < lightDeactivateDistance;

		if (m_profile.lightMode == LightMode.Sine)
        {
            float average = (m_profile.maxIntensity + m_profile.minIntensity) / 2;
            float amplitude = (m_profile.maxIntensity - m_profile.minIntensity) / 2;
            m_light.intensity = amplitude * Mathf.Sin(Time.time * m_profile.intensityChangeRate) + average;
		}
        else if (m_profile.lightMode == LightMode.RandomFluctuation)
        {
			if (Mathf.Abs(m_light.intensity - m_targetIntensity) < 0.1f)
            {
				m_targetIntensity = Random.Range(m_profile.minIntensity, m_profile.maxIntensity);
			}
			m_light.intensity = Mathf.Lerp(m_light.intensity, m_targetIntensity, Time.deltaTime * m_profile.intensityChangeRate);
		}
        else if (m_profile.lightMode == LightMode.Flicker)
        {
			if (!m_isFlickering && Random.Range(0, m_profile.flickerChance) < Time.deltaTime)
            {
				m_isFlickering = true;
                m_flickerEndTime = Time.time + Random.Range(m_profile.flickerMinDuration, m_profile.flickerMaxDuration);
                m_light.intensity = m_profile.minIntensity;
            }
			if (m_isFlickering && Time.time > m_flickerEndTime)
            {
				m_isFlickering = false;
				m_light.intensity = m_profile.maxIntensity;
			}
		}
        else
        {
            m_light.intensity = m_profile.maxIntensity;
        }

        m_emissionSource.material.SetColor("_EmissionColor", m_light.color * 1.35f * (m_light.intensity / m_profile.maxIntensity));
	}
}
