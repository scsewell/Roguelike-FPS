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

    public delegate List<SourceInfo> GetVal();
    private GetVal m_get;

    private void Awake()
    {
        m_button = GetComponentInChildren<Button>();
        m_controlText = GetComponentInChildren<Text>();
        m_bindingText = m_button.GetComponentsInChildren<Text>();
    }

    public RectTransform Init(string name, GetVal get)
    {
        m_controlText.text = name;
        m_get = get;

        Load();

        return GetComponent<RectTransform>();
    }
    
    public void Load()
    {
        foreach (SourceType sourceType in Enum.GetValues(typeof(SourceType)))
        {
            List<SourceInfo> sourceInfo = m_get();
            string str = "";
            foreach (SourceInfo info in sourceInfo)
            {
                if (info.Type == sourceType)
                {
                    str += info.Name;
                    if (info != sourceInfo.Last())
                    {
                        str += ", ";
                    }
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

    }
}