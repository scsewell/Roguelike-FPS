using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Framework.SettingManagement;
using Framework.UI;
using Framework.InputManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private RectTransform prefab_header;
    [SerializeField] private PanelToggle prefab_settingsToggle;
    [SerializeField] private PanelSlider prefab_settingsSlider;
    [SerializeField] private PanelDropdown prefab_settingsDropdown;
    [SerializeField] private RectTransform prefab_headerBindings;
    [SerializeField] private PanelControlBinding prefab_controlBindings;
    [SerializeField] private PanelRebind prefab_binding;
    
    public Canvas canvas_root;
    public Button btn_resume;

    public Canvas canvas_settings;
    public RectTransform panel_settingsViewport;
    public RectTransform panel_setingsContent;
    public Button btn_backSettings;
    public Button btn_applySettings;
    public Button btn_loadDefalutsSettings;
    public Scrollbar scrollbar_settings;

    public Canvas canvas_controls;
    public RectTransform panel_controlsContent;
    public Button btn_backControls;
    public Button btn_applyControls;
    public Button btn_useDefalutsControls;
    public Scrollbar scrollbar_controls;

    public Canvas canvas_bindings;
    public RectTransform panel_currentBindings;
    public Text txt_bindingsTitle;
    public Button btn_backBindings;
    public Button btn_newBinding;

    public Canvas canvas_rebinding;
    public Text txt_rebindTitle;
    public Text txt_rebindMessage;


    private enum Menu
    {
        None,
        Root,
        Settings,
        Controls,
        Bindings,
        Rebinding
    }

    private Menu m_activeMenu;
    private List<ISettingPanel> m_settingPanels;
    private List<ISettingPanel> m_controlPanels;
    private List<PanelRebind> m_bindingPanels;
    private Controls m_editControls;
    private IInputSource m_editSource;
    

    private void Start()
    {
        CreateSettings();
        CreateControls();

        SetMenu(Menu.None);
    }

    private void Update()
    {
        // show the cursor if a menu is open
        Cursor.lockState = IsMenuOpen() ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsMenuOpen();

        // prevent unwanted user input when menu is open
        ControlsManager.Instance.SetMuting(IsMenuOpen());

        // toggle the menu if the menu button was hit, or if the cancel button was hit go back a menu
        if (m_editControls == null || m_editControls.rebindState == Controls.RebindState.None)
        {
            if (ControlsManager.Instance.JustDown(GameButton.Menu))
            {
                SetMenu(IsMenuOpen() ? Menu.None : Menu.Root);
            }
            else if (IsMenuOpen() && Input.GetButtonDown("Cancel"))
            {
                switch (m_activeMenu)
                {
                    case Menu.Root: SetMenu(Menu.None); break;
                    case Menu.Settings:
                    case Menu.Controls: SetMenu(Menu.Root); break;
                    case Menu.Bindings: SetMenu(Menu.Controls); break;
                }
            }
        }

        // ensure there is always something selected so that controllers can always be used
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            switch (m_activeMenu)
            {
                case Menu.Root: btn_resume.Select(); break;
                case Menu.Settings: btn_applySettings.Select(); break;
                case Menu.Controls: btn_applyControls.Select(); break;
                case Menu.Bindings: btn_backBindings.Select(); break;
            }
        }

        // handle key rebinding
        if (m_editControls != null)
        {
            m_editControls.UpdateRebinding();
            switch (m_editControls.rebindState)
            {
                case Controls.RebindState.Button:   txt_rebindMessage.text = "Press any key..."; break;
                case Controls.RebindState.Axis:     txt_rebindMessage.text = "Use any axis, or press a key for the axis <b>positive</b> direction..."; break;
                case Controls.RebindState.ButtonAxis:
                case Controls.RebindState.KeyAxis:  txt_rebindMessage.text = "Press a key for the axis <b>negative</b> direction..."; break;
            }
        }
    }

    public bool IsMenuOpen()
    {
        return m_activeMenu != Menu.None;
    }

    private void SetMenu(Menu menu)
    {
        canvas_root.enabled = (menu == Menu.Root);
        canvas_settings.enabled = (menu == Menu.Settings);
        canvas_controls.enabled = (menu == Menu.Controls);
        canvas_bindings.enabled = (menu == Menu.Bindings);
        canvas_rebinding.enabled = (menu == Menu.Rebinding);

        bool fromRoot = m_activeMenu == Menu.Root;
        m_activeMenu = menu;

        if (fromRoot)
        {
            StartCoroutine(RefreshScrollbar(scrollbar_settings));
            StartCoroutine(RefreshScrollbar(scrollbar_controls));
        }

        switch (menu)
        {
            case Menu.Settings: RefreshSettings(); break;
            case Menu.Controls: RefreshControls(fromRoot); break;
            case Menu.Bindings: RefreshBindings(); break;
        }

        if (!(menu == Menu.Bindings || menu == Menu.Rebinding))
        {
            m_editSource = null;
        }
        else
        {
            string controlName = m_editSource.DisplayName;
            txt_rebindTitle.text = "Rebinding \"" + controlName + "\"";
            txt_bindingsTitle.text = "Bindings for \"" + controlName + "\"";
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Resume()
    {
        SetMenu(Menu.None);
    }

    public void Restart()
    {
        Main.Instance.LoadMainScene();
    }

    public void OpenSettings()
    {
        SetMenu(Menu.Settings);
    }

    public void OpenControls()
    {
        SetMenu(Menu.Controls);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OpenBindings(IInputSource source)
    {
        m_editSource = source;
        SetMenu(Menu.Bindings);
    }

    public void BackToRootMenu()
    {
        SetMenu(Menu.Root);
    }

    private void OnRebindComplete()
    {
        SetMenu(Menu.Bindings);
    }

    public void ApplySettings()
    {
        m_settingPanels.ForEach(s => s.Apply());
        UpdateSettings();
    }

    public void LoadDefaultSettings()
    {
        SettingManager.Instance.UseDefaults();
        UpdateSettings();
    }

    private void UpdateSettings()
    {
        SettingManager.Instance.Apply();
        SettingManager.Instance.Save();
        RefreshSettings();
    }

    public void ApplyControls()
    {
        m_controlPanels.ForEach(s => s.Apply());
        UpdateControls();
    }

    public void UseDefaultControls()
    {
        m_editControls.UseDefaults();
        UpdateControls();
    }

    private void UpdateControls()
    {
        ControlsManager.Instance.SetControls(m_editControls);
        ControlsManager.Instance.Save();
        RefreshControls(true);
    }
    
    private void Rebind(int index)
    {
        RemoveBinding(index);
        AddBinding();
    }

    public void AddBinding()
    {
        if (m_editSource != null)
        {
            m_editControls.AddBinding(m_editSource, OnRebindComplete);
            SetMenu(Menu.Rebinding);
        }
    }

    private void RemoveBinding(int index)
    {
        if (m_editSource != null)
        {
            m_editSource.RemoveSource(index);
            RefreshBindings();
        }
    }

    private IEnumerator RefreshScrollbar(Scrollbar scrollbar)
    {
        yield return null;
        scrollbar.value = 1;
    }

    private void RefreshSettings()
    {
        m_settingPanels.ForEach(s => s.GetValue());
    }

    private void RefreshControls(bool overwriteEditControls)
    {
        if (overwriteEditControls)
        {
            m_editControls = ControlsManager.Instance.ControlsCopy();
        }
        m_controlPanels.Where(p => overwriteEditControls || p is PanelControlBinding).ToList().ForEach(s => s.GetValue());
    }

    private void RefreshBindings()
    {
        panel_currentBindings.Cast<Transform>().ToList().ForEach(panel => Destroy(panel.gameObject));
        m_bindingPanels = new List<PanelRebind>();

        int index = 0;
        foreach (SourceInfo info in m_editSource.SourceInfos)
        {
            m_bindingPanels.Add(UIHelper.Create(prefab_binding, panel_currentBindings).Init(info, index++, Rebind, RemoveBinding));
        }

        if (m_bindingPanels.Count > 1)
        {
            for (int i = 0; i < m_bindingPanels.Count; i++)
            {
                PanelRebind current = m_bindingPanels[i];

                if (i == 0)
                {
                    PanelRebind down = m_bindingPanels[i + 1];
                    current.SetNav(null, null, down.buttonBinding, down.buttonRemove);
                }
                else if (i == m_bindingPanels.Count - 1)
                {
                    PanelRebind up = m_bindingPanels[i - 1];
                    current.SetNav(up.buttonBinding, up.buttonRemove, btn_newBinding, btn_newBinding);

                    Navigation nav = btn_newBinding.navigation;
                    nav.selectOnUp = current.buttonBinding;
                    btn_newBinding.navigation = nav;
                }
                else
                {
                    PanelRebind up = m_bindingPanels[i - 1];
                    PanelRebind down = m_bindingPanels[i + 1];
                    current.SetNav(up.buttonBinding, up.buttonRemove, down.buttonBinding, down.buttonRemove);
                }
            }
        }
        else if (m_bindingPanels.Count > 0)
        {
            m_bindingPanels[0].SetNav(null, null, btn_newBinding, btn_newBinding);

            Navigation nav = btn_newBinding.navigation;
            nav.selectOnUp = m_bindingPanels[0].buttonBinding;
            btn_newBinding.navigation = nav;
        }
    }

    private void CreateSettings()
    {
        m_settingPanels = new List<ISettingPanel>();

        float spacing = 10;

        UIHelper.AddSpacer(panel_setingsContent, spacing);
        CreateSettingsPanels(panel_setingsContent, m_settingPanels, spacing, () => SettingManager.Instance.Settings);

        Navigation explicitNav = new Navigation();
        explicitNav.mode = Navigation.Mode.Explicit;

        Navigation topNav = explicitNav;
        topNav.selectOnUp = btn_applySettings;

        Selectable firstSetting = UIHelper.SetNavigationVertical(panel_setingsContent, topNav, explicitNav, explicitNav);
        Navigation tempNav;

        tempNav = btn_backSettings.navigation;
        tempNav.selectOnDown = firstSetting;
        btn_backSettings.navigation = tempNav;

        tempNav = btn_applySettings.navigation;
        tempNav.selectOnDown = firstSetting;
        btn_applySettings.navigation = tempNav;

        tempNav = btn_loadDefalutsSettings.navigation;
        tempNav.selectOnDown = firstSetting;
        btn_loadDefalutsSettings.navigation = tempNav;
    }

    private void CreateControls()
    {
        m_controlPanels = new List<ISettingPanel>();
        m_editControls = ControlsManager.Instance.ControlsCopy();

        float spacing = 10;

        UIHelper.AddSpacer(panel_controlsContent, spacing);
        CreateSettingsPanels(panel_controlsContent, m_controlPanels, spacing, () => m_editControls.Settings);
        
        UIHelper.Create(prefab_headerBindings, panel_controlsContent);
        foreach (GameButton button in Enum.GetValues(typeof(GameButton)))
        {
            string name = button.ToString();
            if (m_editControls.NameToButton.ContainsKey(name) && m_editControls.NameToButton[name].CanRebind)
            {
                m_controlPanels.Add(UIHelper.Create(prefab_controlBindings, panel_controlsContent).Init(() => m_editControls.NameToButton[name], OpenBindings));
            }
        }
        foreach (GameAxis axis in Enum.GetValues(typeof(GameAxis)))
        {
            string name = axis.ToString();
            if (m_editControls.NameToAxis.ContainsKey(name) && m_editControls.NameToAxis[name].CanRebind)
            {
                m_controlPanels.Add(UIHelper.Create(prefab_controlBindings, panel_controlsContent).Init(() => m_editControls.NameToAxis[name], OpenBindings));
            }
        }
        UIHelper.AddSpacer(panel_controlsContent, spacing);

        Navigation explicitNav = new Navigation();
        explicitNav.mode = Navigation.Mode.Explicit;

        Navigation topNav = explicitNav;
        topNav.selectOnUp = btn_applyControls;
        UIHelper.SetNavigationVertical(panel_controlsContent, topNav, explicitNav, explicitNav);

        Selectable firstControl = UIHelper.SetNavigationVertical(panel_controlsContent, topNav, explicitNav, explicitNav);
        Navigation tempNav;

        tempNav = btn_backControls.navigation;
        tempNav.selectOnDown = firstControl;
        btn_backControls.navigation = tempNav;

        tempNav = btn_applyControls.navigation;
        tempNav.selectOnDown = firstControl;
        btn_applyControls.navigation = tempNav;

        tempNav = btn_useDefalutsControls.navigation;
        tempNav.selectOnDown = firstControl;
        btn_useDefalutsControls.navigation = tempNav;
    }

    private void CreateSettingsPanels(Transform parent, List<ISettingPanel> panels, float spacing, Func<Settings> getSettings)
    {
        Settings settings = getSettings();
        foreach (string category in settings.Categories)
        {
            UIHelper.Create(prefab_header, parent).GetComponentInChildren<Text>().text = category;

            foreach (ISetting setting in settings.CategoryToSettings[category])
            {
                if (setting.DisplayOptions != null)
                {
                    Func<ISetting> getSetting = () => getSettings().GetSetting(setting.Name);

                    if (setting.DisplayOptions.DisplayType == DisplayType.Toggle)
                    {
                        panels.Add(UIHelper.Create(prefab_settingsToggle, parent).Init(getSetting));
                    }
                    else if (setting.DisplayOptions.DisplayType == DisplayType.Slider)
                    {
                        panels.Add(UIHelper.Create(prefab_settingsSlider, parent).Init(getSetting));
                    }
                    else if (setting.DisplayOptions.DisplayType == DisplayType.Dropdown)
                    {
                        panels.Add(UIHelper.Create(prefab_settingsDropdown, parent).Init(getSetting));
                    }
                }
            }
            UIHelper.AddSpacer(parent, spacing);
        }
    }
}