using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PanelDropdown : MonoBehaviour, ISettingPanel
{
    public Text label;
    public Dropdown dropdown;

    public delegate string GetVal();
    public delegate void SetVal(string val);

    private string m_val;
    private string[] m_values;
    private GetVal m_get;
    private SetVal m_set;

    public RectTransform Init(string name, GetVal get, SetVal set, string[] values)
    {
        label.text = name;
        m_get = get;
        m_set = set;

        m_values = values;

        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string value in m_values)
        {
            options.Add(new Dropdown.OptionData(value));
        }
        dropdown.AddOptions(options);

        return GetComponent<RectTransform>();
    }

    public void SetNav(Navigation nav)
    {
        dropdown.navigation = nav;
    }

    public void Apply()
    {
        m_set(m_val);
    }

    public void Load()
    {
        m_val = m_get();

        for (int i = 0; i < m_values.Length; i++)
        {
            if (m_values[i].Equals(m_val))
            {
                dropdown.value = i;
                break;
            }
        }
    }

    public void OnValueChanged()
    {
        m_val = m_values[dropdown.value];
    }
}
