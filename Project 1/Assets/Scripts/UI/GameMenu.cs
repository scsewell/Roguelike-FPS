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
    public RectTransform prefab_controlBindings;

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

    private Settings m_settings;
    private Menu m_activeMenu;
    private List<RectTransform> m_settingPanels;
    private List<RectTransform> m_controlPanels;

    public enum Menu
    {
        None,
        Root,
        Settings,
        Controls
    }

    private void Awake()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        m_settings = gameController.GetComponent<Settings>();

        m_settings.LoadSettings();
        m_settings.ApplySettings();
    }

    private void Start()
    {
        InitMenu();
        SetMenu(Menu.None);
    }

    private void Update()
    {
        // show the cursor if a menu is open
        Cursor.lockState = IsMenuOpen() ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsMenuOpen();

        // prevent unwanted user input when menu is open
        Controls.IsMuted = IsMenuOpen();

        // toggle the menu if the menu button was hit, or if the cancel button was hit go back a menu
        if (Controls.JustDown(GameButton.Menu))
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
            }
        }
    }

    private void InitMenu()
    {
        Navigation tempNav;
        Navigation explicitNav = new Navigation();
        explicitNav.mode = Navigation.Mode.Explicit;

        int GroupSpacing = 10;

        // Create settings panels
        m_settingPanels = new List<RectTransform>();

        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Screen";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Resolution", m_settings.GetResolution, m_settings.SetResolution, m_settings.GetSupportedResolutions()));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Fullscreen", m_settings.GetFullscreen, m_settings.SetFullscreen));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Target Frame Rate", m_settings.GetFrameRate, m_settings.SetFrameRate, Settings.TARGET_FRAME_RATES.Select(x => x.ToString()).ToArray()));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("VSync", m_settings.GetVsync, m_settings.SetVsync));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Field Of View", m_settings.GetFieldOfView, m_settings.SetFieldOfView, Settings.MIN_FOV, Settings.MAX_FOV, true));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Brightness", m_settings.GetBrightness, m_settings.SetBrightness, Settings.MIN_BRIGHTNESS, Settings.MAX_BRIGHTNESS, false));

        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Quality";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Shadow Quality", m_settings.GetShadowQuality, m_settings.SetShadowQuality, Enum.GetNames(typeof(Settings.ShadowQualityLevels))));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Shadow Distance", m_settings.GetShadowDistance, m_settings.SetShadowDistance, Settings.MIN_SHADOW_DISTANCE, Settings.MAX_SHADOW_DISTANCE, true));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Texture Quality", m_settings.GetTextureResolution, m_settings.SetTextureResolution, Enum.GetNames(typeof(Settings.TextureResolution))));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Antialiasing", m_settings.GetAntialiasing, m_settings.SetAntialiasing));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("SSAO", m_settings.GetSSAO, m_settings.SetSSAO));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Bloom", m_settings.GetBloom, m_settings.SetBloom));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Motion Blur", m_settings.GetMotionBlur, m_settings.SetMotionBlur));

        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Audio";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Volume", m_settings.GetVolume, m_settings.SetVolume, 0, 1, false));

        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Other";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Show FPS", m_settings.GetShowFPS, m_settings.SetShowFPS));
        UIHelper.AddSpacer(panel_setingsContent, GroupSpacing);

        Navigation settigsTopNav = explicitNav;
        settigsTopNav.selectOnUp = btn_applySettings;
        
        UIHelper.SetNavigationVertical(m_settingPanels, settigsTopNav, explicitNav, explicitNav);

        Selectable firstSetting = m_settingPanels.First().GetComponentInChildren<Selectable>();

        tempNav = btn_backSettings.navigation;
        tempNav.selectOnDown = firstSetting;
        btn_backSettings.navigation = tempNav;

        tempNav = btn_applySettings.navigation;
        tempNav.selectOnDown = firstSetting;
        btn_applySettings.navigation = tempNav;

        tempNav = btn_loadDefalutsSettings.navigation;
        tempNav.selectOnDown = firstSetting;
        btn_loadDefalutsSettings.navigation = tempNav;

        // Create controls panels
        m_controlPanels = new List<RectTransform>();

        UIHelper.AddSpacer(panel_controlsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_controlsContent).GetComponentInChildren<Text>().text = "General";
        m_controlPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_controlsContent).GetComponent<PanelSlider>().Init("Look Sensitivity", m_settings.GetLookSensitivity, m_settings.SetLookSensitivity, Settings.MIN_LOOK_SENSITIVITY, Settings.MAX_LOOK_SENSITIVITY, false));

        UIHelper.AddSpacer(panel_controlsContent, GroupSpacing);
        UIHelper.Create(prefab_header, panel_controlsContent).GetComponentInChildren<Text>().text = "Bindings";

        foreach (KeyValuePair<GameButton, BufferedButton> button in Controls.Buttons)
        {
            m_controlPanels.Add(UIHelper.Create(prefab_controlBindings, panel_controlsContent).GetComponent<PanelControlBinding>().Init(button.Key.ToString(), button.Value.GetSourceNames));
        }
        foreach (KeyValuePair<GameAxis, BufferedAxis> axis in Controls.Axis)
        {
            m_controlPanels.Add(UIHelper.Create(prefab_controlBindings, panel_controlsContent).GetComponent<PanelControlBinding>().Init(axis.Key.ToString(), axis.Value.GetSourceNames));
        }

        UIHelper.AddSpacer(panel_controlsContent, GroupSpacing);

        Navigation controlsTopNav = explicitNav;
        controlsTopNav.selectOnUp = btn_applyControls;

        UIHelper.SetNavigationVertical(m_controlPanels, controlsTopNav, explicitNav, explicitNav);

        Selectable firstControl = m_controlPanels.First().GetComponentInChildren<Selectable>();

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

    public bool IsMenuOpen()
    {
        return m_activeMenu != Menu.None;
    }

    private void SetMenu(Menu menu)
    {
        m_activeMenu = menu;

        canvas_root.enabled = (menu == Menu.Root);
        canvas_settings.enabled = (menu == Menu.Settings);
        canvas_controls.enabled = (menu == Menu.Controls);

        RefreshSettings();

        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator RefreshScrollbar(Scrollbar scrollbar)
    {
        yield return null;
        yield return null;
        scrollbar.value = 1;
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

    public void BackToRootMenu()
    {
        SetMenu(Menu.Root);
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    private void RefreshSettings()
    {
        foreach (RectTransform rt in m_settingPanels)
        {
            PanelToggle toggle = rt.GetComponent<PanelToggle>();
            PanelSlider slider = rt.GetComponent<PanelSlider>();
            PanelDropdown dropdown = rt.GetComponent<PanelDropdown>();
            if (toggle)
            {
                toggle.Load();
            }
            else if (slider)
            {
                slider.Load();
            }
            else if (dropdown)
            {
                dropdown.Load();
            }
        }
        StartCoroutine(RefreshScrollbar(scrollbar_settings));

        foreach (RectTransform rt in m_controlPanels)
        {
            PanelToggle toggle = rt.GetComponent<PanelToggle>();
            PanelSlider slider = rt.GetComponent<PanelSlider>();
            PanelControlBinding binding = rt.GetComponent<PanelControlBinding>();
            if (toggle)
            {
                toggle.Load();
            }
            else if (slider)
            {
                slider.Load();
            }
            else if (binding)
            {
                binding.Load();
            }
        }
        StartCoroutine(RefreshScrollbar(scrollbar_controls));
    }

    public void ApplySettings()
    {
        foreach (RectTransform rt in m_settingPanels)
        {
            PanelToggle toggle = rt.GetComponent<PanelToggle>();
            PanelSlider slider = rt.GetComponent<PanelSlider>();
            PanelDropdown dropdown = rt.GetComponent<PanelDropdown>();
            if (toggle)
            {
                toggle.Apply();
            }
            else if (slider)
            {
                slider.Apply();
            }
            else if (dropdown)
            {
                dropdown.Apply();
            }
        }

        m_settings.ApplySettings();
        m_settings.SaveSettings();
        RefreshSettings();
    }

    public void LoadDefaultSettings()
    {
        m_settings.LoadDefaults();
        m_settings.ApplySettings();
        m_settings.SaveSettings();
        RefreshSettings();
    }

    public void UseDefaultControls()
    {
        Controls.loadDefaultControls();
        RefreshSettings();
    }
}