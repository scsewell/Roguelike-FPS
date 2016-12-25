using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelSlider : MonoBehaviour, ISettingPanel
{
    public Text label;
    public Text valueText;
    public Slider slider;

    public delegate float GetVal();
    public delegate void SetVal(float val);

    private float m_val;
    private float m_min;
    private float m_max;
    private bool m_intOnly;
    private GetVal m_get;
    private SetVal m_set;

    public RectTransform Init(string name, GetVal get, SetVal set, float min, float max, bool intOnly)
    {
        label.text = name;
        m_get = get;
        m_set = set;
        m_intOnly = intOnly;

        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = m_intOnly;
        return GetComponent<RectTransform>();
    }

    public void SetNav(Navigation nav)
    {
        slider.navigation = nav;
    }

    public void Apply()
    {
        m_set(m_val);
    }

    public void Load()
    {
        m_val = m_get();
        slider.value = m_val;
        UpdateText();
    }

    public void OnValueChanged()
    {
        m_val = slider.value;
        UpdateText();
    }

    private void UpdateText()
    {
        valueText.text = m_intOnly ? m_val.ToString() : (Mathf.Round(m_val * 100) / 100.0f).ToString();
    }
}
