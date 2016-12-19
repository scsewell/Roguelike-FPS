using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour
{
    public float fpsUpdateInterval = 0.5f;

    private int m_framesOverInterval = 0;
    private float m_fpsOverInterval = 0.0f;
    private float m_timeLeftBeforeUpdate;
    private float m_fps;

    private Text m_text;
    private Settings m_settings;
    
    private void Awake()
    {
        m_settings = GameObject.FindGameObjectWithTag("GameController").GetComponent<Settings>();
        m_text = GetComponent<Text>();
    }

    private void Start()
    {
        m_timeLeftBeforeUpdate = fpsUpdateInterval;
    }

    private void LateUpdate()
    {
        m_text.text = "";

        // update fps
        m_fpsOverInterval += Time.timeScale / Time.deltaTime;
        m_framesOverInterval++;

        m_timeLeftBeforeUpdate -= Time.deltaTime;
        if (m_timeLeftBeforeUpdate <= 0.0f)
        {
            m_fps = (m_fpsOverInterval / m_framesOverInterval);
            m_timeLeftBeforeUpdate = fpsUpdateInterval;
            m_fpsOverInterval = 0.0f;
            m_framesOverInterval = 0;
        }

        // set display information
        if (m_settings.GetShowFPS())
        {
            m_text.text += "fps: " + m_fps + "\n";
        }
    }
}
