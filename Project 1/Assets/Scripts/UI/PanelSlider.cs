using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelSlider : MonoBehaviour, ISettingPanel
{
    public Text label;
    public Text valueText;
    public Slider slider;
    
    private bool m_intOnly;
    private Action<float> m_set;

    public RectTransform Init(string name, Func<float> get, Action<float> set, float min, float max, bool intOnly)
    {
        m_set = set;
        m_intOnly = intOnly;

        label.text = name;
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = m_intOnly;
        slider.value = get();
        UpdateText();

        return GetComponent<RectTransform>();
    }

    public void Apply()
    {
        m_set(slider.value);
    }

    public void OnValueChanged()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        valueText.text = m_intOnly ? slider.value.ToString() : (Mathf.Round(slider.value * 100) / 100.0f).ToString();
    }
}
