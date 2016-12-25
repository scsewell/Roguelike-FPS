using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelToggle : MonoBehaviour, ISettingPanel
{
    public Text label;
    public Toggle toggle;

    public delegate bool GetVal();
    public delegate void SetVal(bool val);
    
    private bool m_val;
    private GetVal m_get;
    private SetVal m_set;

    public RectTransform Init(string name, GetVal get, SetVal set)
    {
        label.text = name;
        m_get = get;
        m_set = set;
        return GetComponent<RectTransform>();
    }

    public void SetNav(Navigation nav)
    {
        toggle.navigation = nav;
    }

    public void Apply()
    {
        m_set(m_val);
    }

    public void Load()
    {
        m_val = m_get();
        toggle.isOn = m_val;
    }

    public void OnValueChanged()
    {
        m_val = toggle.isOn;
    }
}
