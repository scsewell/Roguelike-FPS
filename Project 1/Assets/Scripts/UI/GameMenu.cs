using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputController;

public class GameMenu : MonoBehaviour
{
    public RectTransform prefab_header;
    public RectTransform prefab_settingsToggle;
    public RectTransform prefab_settingsSlider;
    public RectTransform prefab_settingsDropdown;
    public RectTransform prefab_headerBindings;
    public RectTransform prefab_controlBindings;
    public RectTransform prefab_binding;
    
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


    private enum Menu { None, Root, Settings, Controls, Bindings, Rebinding }
    private Menu m_activeMenu;
    
    private List<RectTransform> m_settingPanels;
    private List<RectTransform> m_controlPanels;
    private List<PanelRebind> m_bindingPanels;
    private Settings m_editSettings;
    private Controls m_editControls;
    private KeyValuePair<GameButton, BufferedButton> m_editButton;
    private KeyValuePair<GameAxis, BufferedAxis> m_editAxis;


    private void Awake()
    {
        Settings.Instance.Load();
        Settings.Instance.Apply();
    }

    private void Start()
    {
        SetMenu(Menu.None);
    }

    private void Update()
    {
        // show the cursor if a menu is open
        Cursor.lockState = IsMenuOpen() ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsMenuOpen();

        // prevent unwanted user input when menu is open
        Controls.Instance.IsMuted = IsMenuOpen();

        // toggle the menu if the menu button was hit, or if the cancel button was hit go back a menu
        if (Controls.Instance.rebindState == Controls.RebindState.None)
        {
            if (Controls.Instance.JustDown(GameButton.Menu))
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
                case Menu.Settings: btn_backSettings.Select(); break;
                case Menu.Controls: btn_backControls.Select(); break;
                case Menu.Bindings: btn_backBindings.Select(); break;
            }
        }

        // display correct rebind message if applicable
        string rebindMessage = "";
        switch (Controls.Instance.rebindState)
        {
            case Controls.RebindState.Button: rebindMessage = "Press any key..."; break;
            case Controls.RebindState.Axis: rebindMessage = "Use any axis, or press a key for the axis <b>positive</b> direction..."; break;
            case Controls.RebindState.ButtonAxis:
            case Controls.RebindState.KeyAxis: rebindMessage = "Press a key for the axis <b>negative</b> direction..."; break;
        }
        txt_rebindMessage.text = rebindMessage;
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
            m_editButton = default(KeyValuePair<GameButton, BufferedButton>);
            m_editAxis = default(KeyValuePair<GameAxis, BufferedAxis>);
        }
        else
        {
            string controlName = "";
            if (!m_editButton.Equals(default(KeyValuePair<GameButton, BufferedButton>)))
            {
                controlName = ControlNames.GetName(m_editButton.Key);
            }
            if (!m_editAxis.Equals(default(KeyValuePair<GameAxis, BufferedAxis>)))
            {
                controlName = ControlNames.GetName(m_editAxis.Key);
            }
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
        SceneManager.LoadScene(1);
    }

    public void OpenSettings()
    {
        SetMenu(Menu.Settings);
    }

    public void OpenControls()
    {
        SetMenu(Menu.Controls);
    }

    private void OpenBindings(KeyValuePair<GameButton, BufferedButton> button)
    {
        m_editButton = button;
        SetMenu(Menu.Bindings);
    }

    private void OpenBindings(KeyValuePair<GameAxis, BufferedAxis> axis)
    {
        m_editAxis = axis;
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

    public void Quit()
    {
        Application.Quit();
    }

    public void ApplySettings()
    {
        m_settingPanels.ForEach(rt => rt.GetComponent<ISettingPanel>().Apply());
        Settings.Instance = m_editSettings;
        Settings.Instance.Apply();
        Settings.Instance.Save();
        RefreshSettings();
    }

    public void ApplyControls()
    {
        m_controlPanels.ForEach(rt => rt.GetComponent<ISettingPanel>().Apply());
        Controls.Instance = m_editControls;
        Controls.Instance.Save();
        RefreshControls(true);
    }

    public void LoadDefaultSettings()
    {
        Settings.Instance.LoadDefaults();
        Settings.Instance.Apply();
        Settings.Instance.Save();
        RefreshSettings();
    }

    public void UseDefaultControls()
    {
        Controls.Instance.LoadDefaults();
        Controls.Instance.Save();
        RefreshControls(true);
    }

    private void Rebind(int index)
    {
        RemoveBinding(index);
        AddBinding();
    }

    public void AddBinding()
    {
        if (!m_editButton.Equals(default(KeyValuePair<GameButton, BufferedButton>)))
        {
            Controls.Instance.AddBinding(m_editButton.Value, OnRebindComplete);
            SetMenu(Menu.Rebinding);
        }
        if (!m_editAxis.Equals(default(KeyValuePair<GameAxis, BufferedAxis>)))
        {
            Controls.Instance.AddBinding(m_editAxis.Value, OnRebindComplete);
            SetMenu(Menu.Rebinding);
        }
    }

    private void RemoveBinding(int index)
    {
        if (!m_editButton.Equals(default(KeyValuePair<GameButton, BufferedButton>)))
        {
            m_editButton.Value.RemoveSource(index);
            RefreshBindings();
        }
        if (!m_editAxis.Equals(default(KeyValuePair<GameAxis, BufferedAxis>)))
        {
            m_editAxis.Value.RemoveSource(index);
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
        m_editSettings = JsonConverter.SerializedCopy(Settings.Instance);

        panel_setingsContent.Cast<Transform>().ToList().ForEach(panel => Destroy(panel.gameObject));
        m_settingPanels = new List<RectTransform>();

        int GroupSpacing = 10;

        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Screen";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Resolution", m_editSettings.GetResolution, m_editSettings.SetResolution, Settings.GetSupportedResolutions()));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Fullscreen", m_editSettings.GetFullscreen, m_editSettings.SetFullscreen));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Target Frame Rate", m_editSettings.GetFrameRate, m_editSettings.SetFrameRate, Settings.TARGET_FRAME_RATES.Select(x => x.ToString()).ToArray()));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("VSync", m_editSettings.GetVsync, m_editSettings.SetVsync));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Field Of View", m_editSettings.GetFieldOfView, m_editSettings.SetFieldOfView, Settings.MIN_FOV, Settings.MAX_FOV, true));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Brightness", m_editSettings.GetBrightness, m_editSettings.SetBrightness, Settings.MIN_BRIGHTNESS, Settings.MAX_BRIGHTNESS, false));

        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Quality";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Shadow Quality", m_editSettings.GetShadowQuality, m_editSettings.SetShadowQuality, Enum.GetNames(typeof(Settings.ShadowQualityLevels))));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Shadow Distance", m_editSettings.GetShadowDistance, m_editSettings.SetShadowDistance, Settings.MIN_SHADOW_DISTANCE, Settings.MAX_SHADOW_DISTANCE, true));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Texture Quality", m_editSettings.GetTextureResolution, m_editSettings.SetTextureResolution, Enum.GetNames(typeof(Settings.TextureResolution))));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Antialiasing", m_editSettings.GetAntialiasing, m_editSettings.SetAntialiasing));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("SSAO", m_editSettings.GetSSAO, m_editSettings.SetSSAO));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Bloom", m_editSettings.GetBloom, m_editSettings.SetBloom));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Motion Blur", m_editSettings.GetMotionBlur, m_editSettings.SetMotionBlur));

        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Audio";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Volume", m_editSettings.GetVolume, m_editSettings.SetVolume, 0, 1, false));

        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Other";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Show FPS", m_editSettings.GetShowFPS, m_editSettings.SetShowFPS));
        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);

        Navigation explicitNav = new Navigation();
        explicitNav.mode = Navigation.Mode.Explicit;

        Navigation settigsTopNav = explicitNav;
        settigsTopNav.selectOnUp = btn_applySettings;
        UIHelper.SetNavigationVertical(m_settingPanels, settigsTopNav, explicitNav, explicitNav);

        Selectable firstSetting = m_settingPanels.First().GetComponentInChildren<Selectable>();
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

    private void RefreshControls(bool resetControls)
    {
        if (resetControls)
        {
            m_editControls = JsonConverter.SerializedCopy(Controls.Instance);
        }

        panel_controlsContent.Cast<Transform>().ToList().ForEach(panel => Destroy(panel.gameObject));
        m_controlPanels = new List<RectTransform>();

        int GroupSpacing = 10;

        UIHelper.AddSpacer(panel_controlsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_controlsContent).GetComponentInChildren<Text>().text = "General";
        m_controlPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_controlsContent).GetComponent<PanelSlider>().Init("Look Sensitivity", m_editControls.GetLookSensitivity, m_editControls.SetLookSensitivity, Controls.MIN_LOOK_SENSITIVITY, Controls.MAX_LOOK_SENSITIVITY, false));

        UIHelper.AddSpacer(panel_controlsContent, GroupSpacing);
        UIHelper.Create(prefab_headerBindings, panel_controlsContent);
        foreach (KeyValuePair<GameButton, BufferedButton> button in m_editControls.Buttons)
        {
            m_controlPanels.Add(UIHelper.Create(prefab_controlBindings, panel_controlsContent).GetComponent<PanelControlBinding>().Init(button, OpenBindings));
        }
        foreach (KeyValuePair<GameAxis, BufferedAxis> axis in m_editControls.Axes)
        {
            m_controlPanels.Add(UIHelper.Create(prefab_controlBindings, panel_controlsContent).GetComponent<PanelControlBinding>().Init(axis, OpenBindings));
        }
        UIHelper.AddSpacer(panel_controlsContent, GroupSpacing);

        Navigation explicitNav = new Navigation();
        explicitNav.mode = Navigation.Mode.Explicit;

        Navigation controlsTopNav = explicitNav;
        controlsTopNav.selectOnUp = btn_applyControls;
        UIHelper.SetNavigationVertical(m_controlPanels, controlsTopNav, explicitNav, explicitNav);

        Selectable firstControl = m_controlPanels.First().GetComponentInChildren<Selectable>();
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

    private void RefreshBindings()
    {
        panel_currentBindings.Cast<Transform>().ToList().ForEach(panel => Destroy(panel.gameObject));
        m_bindingPanels = new List<PanelRebind>();

        int index = 0;
        if (!m_editButton.Equals(default(KeyValuePair<GameButton, BufferedButton>)))
        {
            foreach (SourceInfo info in m_editButton.Value.GetSourceInfos())
            {
                m_bindingPanels.Add(UIHelper.Create(prefab_binding, panel_currentBindings).GetComponent<PanelRebind>().Init(info, index++, Rebind, RemoveBinding));
            }
        }
        else if (!m_editAxis.Equals(default(KeyValuePair<GameAxis, BufferedAxis>)))
        {
            foreach (SourceInfo info in m_editAxis.Value.GetSourceInfos())
            {
                m_bindingPanels.Add(UIHelper.Create(prefab_binding, panel_currentBindings).GetComponent<PanelRebind>().Init(info, index++, Rebind, RemoveBinding));
            }
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
}