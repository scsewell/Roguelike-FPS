using System;
using UnityEngine;
using UnityEngine.UI;
using Framework.SettingManagement;

public class PanelSlider : MonoBehaviour, ISettingPanel
{
    [SerializeField]
    private Text m_label;
    [SerializeField]
    private Text m_valueText;
    [SerializeField]
    private Slider m_slider;

    private Func<ISetting> m_getSetting;

    public ISettingPanel Init(Func<ISetting> getSetting)
    {
        m_getSetting = getSetting;
        ISetting setting = getSetting();
        
        m_label.text = setting.Name;
        m_slider.minValue = setting.DisplayOptions.MinValue;
        m_slider.maxValue = setting.DisplayOptions.MaxValue;
        m_slider.wholeNumbers = setting.DisplayOptions.IntegerOnly;

        GetValue();

        return this;
    }

    public void GetValue()
    {
        m_slider.value = float.Parse(m_getSetting().Serialize());
        UpdateText();
    }

    public void Apply()
    {
        m_getSetting().Deserialize(m_slider.value.ToString());
    }
    
    public void OnValueChanged()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        m_valueText.text = m_getSetting().DisplayOptions.IntegerOnly ? m_slider.value.ToString() : (Mathf.Round(m_slider.value * 100) / 100.0f).ToString();
    }
}
