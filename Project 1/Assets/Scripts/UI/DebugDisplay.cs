using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Framework;

public class DebugDisplay : MonoBehaviour
{
    [SerializeField] [Range(0,1)]
    public float m_fpsUpdateInterval = 0.1f;

    private int m_framesOverInterval = 0;
    private float m_fpsOverInterval = 0;
    private float m_timeLeftBeforeUpdate;
    private float m_fps;

    private Text m_text;
    private StringBuilder m_sb = new StringBuilder();
    
    private void Awake()
    {
        m_text = GetComponent<Text>();
    }

    private void Start()
    {
        m_timeLeftBeforeUpdate = m_fpsUpdateInterval;
    }

    private void LateUpdate()
    {
        // update fps
        m_fpsOverInterval += Time.timeScale / Time.deltaTime;
        m_framesOverInterval++;

        m_timeLeftBeforeUpdate -= Time.deltaTime;
        if (m_timeLeftBeforeUpdate <= 0)
        {
            m_fps = (m_fpsOverInterval / m_framesOverInterval);
            m_timeLeftBeforeUpdate = m_fpsUpdateInterval;
            m_fpsOverInterval = 0;
            m_framesOverInterval = 0;
        }

        m_text.enabled = SettingManager.Instance.ShowFPS;

        // set display information
        if (m_text.enabled)
        {
            m_sb.Clear();
            m_sb.Append("fps: ");
            m_sb.Concat(m_fps, 2);
            m_sb.Append('\n');
            
            m_text.text = m_sb.ToString();
        }
    }
}
