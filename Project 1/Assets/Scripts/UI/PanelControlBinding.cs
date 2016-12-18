using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelControlBinding : MonoBehaviour
{
    /*
    private Transform m_buttonP;
    private Transform m_buttonJ;
    private BufferedButton m_button;
    private InputAxis m_axis;

    void Awake()
    {
        m_buttonP = transform.FindChild("Button_PC");
        m_buttonJ = transform.FindChild("Button_Joystick");
    }

    public ButtonSource GetPButton()
    {
        return m_buttonP.GetComponent<ButtonSource>();
    }

    public ButtonSource GetJButton()
    {
        return m_buttonJ.GetComponent<ButtonSource>();
    }

    public void SetNavigation(PanelControlBinding panelAbove, PanelControlBinding panelBelow, Scrollbar scrollbar, ButtonSource above)
    {
        Navigation pNav = new Navigation();
        Navigation jNav = new Navigation();

        pNav.mode = Navigation.Mode.Explicit;
        jNav.mode = Navigation.Mode.Explicit;

        if (panelAbove)
        {
            pNav.selectOnUp = panelAbove.GetPButton();
            jNav.selectOnUp = panelAbove.GetJButton();
        }
        if (panelBelow)
        {
            pNav.selectOnDown = panelBelow.GetPButton();
            jNav.selectOnDown = panelBelow.GetJButton();
        }
        else
        {
            pNav.selectOnDown = above;
            jNav.selectOnDown = above;
        }

        pNav.selectOnRight = GetJButton();
        jNav.selectOnLeft = GetPButton();
        jNav.selectOnRight = scrollbar;

        m_buttonP.GetComponent<ButtonSource>().navigation = pNav;
        m_buttonJ.GetComponent<ButtonSource>().navigation = jNav;
    }

    public void Initialize(BufferedButton button)
    {
        GetComponentInChildren<Text>().text = button.name;
        m_buttonP.GetComponentInChildren<Text>().text = button.pcButton.ToString();
        m_buttonJ.GetComponentInChildren<Text>().text = button.joystickButton.Name;
        m_button = button;
    }

    public void Initialize(InputAxis axis)
    {
        GetComponentInChildren<Text>().text = axis.name;
        m_buttonP.GetComponentInChildren<Text>().text = axis.mouseAxis.Name;
        m_buttonJ.GetComponentInChildren<Text>().text = axis.joystickAxis.Name;
        m_axis = axis;
    }
    */
}