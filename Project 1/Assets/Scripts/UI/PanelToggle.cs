using System;
using UnityEngine;
using UnityEngine.UI;
using Framework.SettingManagement;

public class PanelToggle : MonoBehaviour, ISettingPanel
{
    [SerializeField]
    private Text m_label;
    [SerializeField]
    private Toggle m_toggle;

    private Func<ISetting> m_getSetting;

    public ISettingPanel Init(Func<ISetting> getSetting)
    {
        m_getSetting = getSetting;
        ISetting setting = getSetting();

        m_label.text = setting.Name;

        GetValue();

        return this;
    }

    public void GetValue()
    {
        m_toggle.isOn = bool.Parse(m_getSetting().Serialize());
    }

    public void Apply()
    {
        m_getSetting().Deserialize(m_toggle.isOn.ToString());
    }
}
