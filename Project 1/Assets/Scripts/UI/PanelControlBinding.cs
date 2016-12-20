using UnityEngine;
using UnityEngine.UI;

public class PanelControlBinding : MonoBehaviour
{
    private Button m_button;
    private Text m_bindingText;
    private Text m_controlText;

    public delegate string GetVal();
    private GetVal m_get;

    private void Awake()
    {
        m_button = GetComponentInChildren<Button>();
        m_controlText = GetComponentInChildren<Text>();
        m_bindingText = m_button.GetComponentInChildren<Text>();
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
        m_bindingText.text = m_get();
    }

    public void SetNav(Navigation nav)
    {
        m_button.navigation = nav;
    }

    public void OnButtonPressed()
    {

    }
}