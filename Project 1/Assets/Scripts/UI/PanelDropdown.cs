using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.SettingManagement;

public class PanelDropdown : MonoBehaviour, ISettingPanel
{
    [SerializeField]
    private Text m_label;
    [SerializeField]
    private Dropdown m_dropdown;

    private Func<ISetting> m_getSetting;

    public ISettingPanel Init(Func<ISetting> getSetting)
    {
        m_getSetting = getSetting;
        ISetting setting = getSetting();

        m_label.text = setting.Name;
        
        m_dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string value in setting.DisplayOptions.Values)
        {
            options.Add(new Dropdown.OptionData(value));
        }
        m_dropdown.AddOptions(options);

        GetValue();

        return this;
    }

    public void GetValue()
    {
        ISetting setting = m_getSetting();
        for (int i = 0; i < setting.DisplayOptions.Values.Length; i++)
        {
            if (setting.DisplayOptions.Values[i] == setting.Serialize())
            {
                m_dropdown.value = i;
                break;
            }
        }
    }

    public void Apply()
    {
        ISetting setting = m_getSetting();
        setting.Deserialize(setting.DisplayOptions.Values[m_dropdown.value]);
    }
}
