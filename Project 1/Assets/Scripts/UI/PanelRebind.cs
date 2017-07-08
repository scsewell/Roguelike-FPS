using UnityEngine;
using UnityEngine.UI;
using System;
using Framework.InputManagement;

public class PanelRebind : MonoBehaviour
{
    private Button m_buttonBinding;
    public Button buttonBinding
    {
        get { return m_buttonBinding; }
    }

    private Button m_buttonRemove;
    public Button buttonRemove
    {
        get { return m_buttonRemove; }
    }

    private Text m_typeText;
    private Text m_bindingText;

    private int m_bindingIndex;
    private Action<int> m_rebind;
    private Action<int> m_removeBinding;

    private void Awake()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        m_buttonBinding = buttons[0];
        m_buttonRemove = buttons[1];

        m_typeText = GetComponentInChildren<Text>();
        m_bindingText = m_buttonBinding.GetComponentInChildren<Text>();
    }

    public PanelRebind Init(SourceInfo sourceInfo, int bindingIndex, Action<int> rebind, Action<int> removeBinding)
    {
        m_bindingIndex = bindingIndex;
        m_rebind = rebind;
        m_removeBinding = removeBinding;

        m_bindingText.text = sourceInfo.Name;

        string typeText = "";
        switch (sourceInfo.SourceType)
        {
            case SourceType.MouseKeyboard:  typeText = "M/Kb"; break;
            case SourceType.Joystick:       typeText = "J"; break;
        }
        m_typeText.text = typeText;

        return this;
    }

    public void SetNav(Selectable bindingUp, Selectable removeUp, Selectable bindingDown, Selectable removeDown)
    {
        Navigation bindingNav = new Navigation();
        bindingNav.mode = Navigation.Mode.Explicit;
        bindingNav.selectOnUp = bindingUp;
        bindingNav.selectOnDown = bindingDown;
        bindingNav.selectOnRight = m_buttonRemove;
        m_buttonBinding.navigation = bindingNav;
        
        Navigation removeNav = new Navigation();
        removeNav.mode = Navigation.Mode.Explicit;
        removeNav.selectOnUp = removeUp;
        removeNav.selectOnDown = removeDown;
        removeNav.selectOnLeft = m_buttonBinding;
        m_buttonRemove.navigation = removeNav;
    }

    public void Rebind()
    {
        m_rebind(m_bindingIndex);
    }

    public void Remove()
    {
        m_removeBinding(m_bindingIndex);
    }
}
