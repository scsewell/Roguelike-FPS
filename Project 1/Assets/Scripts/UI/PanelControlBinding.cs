using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Framework.InputManagement;

public class PanelControlBinding : MonoBehaviour, ISettingPanel
{
    private Button m_button;
    private Text[] m_bindingText;
    private Text m_controlText;
    
    private Func<IInputSource> m_getSource;
    private Action<IInputSource> m_onEdit;

    private void Awake()
    {
        m_button = GetComponentInChildren<Button>();
        m_controlText = GetComponentInChildren<Text>();
        m_bindingText = m_button.GetComponentsInChildren<Text>();
    }

    public PanelControlBinding Init(Func<IInputSource> getSource, Action<IInputSource> onEdit)
    {
        m_getSource = getSource;
        m_onEdit = onEdit;
        return this;
    }

    public void Apply() {}

    public void GetValue()
    {
        IInputSource source = m_getSource();

        m_controlText.text = source.DisplayName;

        foreach (SourceType sourceType in Enum.GetValues(typeof(SourceType)))
        {
            List<SourceInfo> sourceInfo = source.SourceInfos.Where(info => info.SourceType == sourceType).ToList();
            string str = "";
            foreach (SourceInfo info in sourceInfo)
            {
                str += info.Name;
                if (info != sourceInfo.Last())
                {
                    str += ", ";
                }
            }
            m_bindingText[(int)sourceType].text = str;
        }
    }

    public void OnButtonPressed()
    {
        m_onEdit(m_getSource());
    }
}