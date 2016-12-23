using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using InputController;

public class PanelControlBinding : MonoBehaviour
{
    private Button m_button;
    private Text[] m_bindingText;
    private Text m_controlText;

    private KeyValuePair<GameButton, BufferedButton> m_buttonSource;
    private KeyValuePair<GameAxis, BufferedAxis> m_axisSource;
    
    private Func<List<SourceInfo>> m_getSourceInfo;
    private Action<KeyValuePair<GameButton, BufferedButton>> m_onEditButton;
    private Action<KeyValuePair<GameAxis, BufferedAxis>> m_onEditAxis;

    private void Awake()
    {
        m_button = GetComponentInChildren<Button>();
        m_controlText = GetComponentInChildren<Text>();
        m_bindingText = m_button.GetComponentsInChildren<Text>();
    }

    public RectTransform Init(KeyValuePair<GameButton, BufferedButton> buttonSource, Action<KeyValuePair<GameButton, BufferedButton>> onEdit)
    {
        m_buttonSource = buttonSource;
        m_controlText.text = ControlNames.GetName(buttonSource.Key);
        m_getSourceInfo = buttonSource.Value.GetSourceInfos;
        m_onEditButton = onEdit;

        Load();

        return GetComponent<RectTransform>();
    }

    public RectTransform Init(KeyValuePair<GameAxis, BufferedAxis> axisSource, Action<KeyValuePair<GameAxis, BufferedAxis>> onEdit)
    {
        m_axisSource = axisSource;
        m_controlText.text = ControlNames.GetName(axisSource.Key);
        m_getSourceInfo = axisSource.Value.GetSourceInfos;
        m_onEditAxis = onEdit;

        Load();

        return GetComponent<RectTransform>();
    }
    
    public void Load()
    {
        foreach (SourceType sourceType in Enum.GetValues(typeof(SourceType)))
        {
            List<SourceInfo> sourceInfo = m_getSourceInfo().Where(info => info.Type == sourceType).ToList();
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

    public void SetNav(Navigation nav)
    {
        m_button.navigation = nav;
    }

    public void OnButtonPressed()
    {
        if (m_onEditButton != null)
        {
            m_onEditButton(m_buttonSource);
        }
        if (m_onEditAxis != null)
        {
            m_onEditAxis(m_axisSource);
        }
    }
}