using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelToggle : MonoBehaviour, ISettingPanel
{
    public Text label;
    public Toggle toggle;
    
    private Action<bool> m_set;

    public RectTransform Init(string name, Func<bool> get, Action<bool> set)
    {
        m_set = set;
        toggle.isOn = get();
        label.text = name;

        return GetComponent<RectTransform>();
    }

    public void Apply()
    {
        m_set(toggle.isOn);
    }
}
