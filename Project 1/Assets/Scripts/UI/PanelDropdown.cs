using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PanelDropdown : MonoBehaviour, ISettingPanel
{
    public Text label;
    public Dropdown dropdown;
    
    private string[] m_values;
    private Action<string> m_set;

    public RectTransform Init(string name, Func<string> get, Action<string> set, string[] values)
    {
        m_set = set;
        m_values = values;
        label.text = name;
        
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string value in m_values)
        {
            options.Add(new Dropdown.OptionData(value));
        }
        dropdown.AddOptions(options);

        for (int i = 0; i < m_values.Length; i++)
        {
            if (m_values[i].Equals(get()))
            {
                dropdown.value = i;
                break;
            }
        }

        return GetComponent<RectTransform>();
    }

    public void Apply()
    {
        m_set(m_values[dropdown.value]);
    }
}
