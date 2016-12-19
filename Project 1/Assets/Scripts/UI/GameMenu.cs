using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameMenu : MonoBehaviour
{
    public RectTransform prefab_settingsToggle;
    public RectTransform prefab_settingsSlider;
    public RectTransform prefab_settingsDropdown;
    public RectTransform prefab_header;

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
    public Button btn_acceptControls;
    public Button btn_useDefalutsControls;

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

        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null)
        {
            switch (m_activeMenu)
            {
                case Menu.Root: btn_resume.Select(); break;
                case Menu.Settings: btn_backSettings.Select(); break;
                case Menu.Controls: btn_acceptControls.Select(); break;
            }
        }
        else
        {
            /*
            RectTransform selectedParent = selected.transform.parent.GetComponent<RectTransform>();

            if (selectedParent != null && m_settingPanels.Contains(selectedParent))
            {
                Vector3[] corners = new Vector3[4];
                selectedParent.GetWorldCorners(corners);

                Vector3 center = Vector3.zero;
                foreach (Vector3 corner in corners)
                {
                    center += corner;
                }
                center /= 4;

                if (!RectTransformUtility.RectangleContainsScreenPoint(panel_settingsViewport, RectTransformUtility.WorldToScreenPoint(null, center)))
                {
                    scrollbar_settings.value = 1 - (m_settingPanels.IndexOf(selectedParent) * 1.25f / m_settingPanels.Count);
                }
            }
            */
        }
    }

    private void InitMenu()
    {
        // Create settings panels
        m_settingPanels = new List<RectTransform>();

        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Screen";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Resolution", m_settings.GetResolution, m_settings.SetResolution, m_settings.GetSupportedResolutions()));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Fullscreen", m_settings.GetFullscreen, m_settings.SetFullscreen));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Target Frame Rate", m_settings.GetFrameRate, m_settings.SetFrameRate, Settings.TARGET_FRAME_RATES.Select(x => x.ToString()).ToArray()));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("VSync", m_settings.GetVsync, m_settings.SetVsync));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Field Of View", m_settings.GetFieldOfView, m_settings.SetFieldOfView, Settings.MIN_FOV, Settings.MAX_FOV, true));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Brightness", m_settings.GetBrightness, m_settings.SetBrightness, Settings.MIN_BRIGHTNESS, Settings.MAX_BRIGHTNESS, false));

        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Quality";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Shadow Quality", m_settings.GetShadowQuality, m_settings.SetShadowQuality, Enum.GetNames(typeof(Settings.ShadowQualityLevels))));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Shadow Distance", m_settings.GetShadowDistance, m_settings.SetShadowDistance, Settings.MIN_SHADOW_DISTANCE, Settings.MAX_SHADOW_DISTANCE, true));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsDropdown, panel_setingsContent).GetComponent<PanelDropdown>().Init("Texture Quality", m_settings.GetTextureResolution, m_settings.SetTextureResolution, Enum.GetNames(typeof(Settings.TextureResolution))));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Antialiasing", m_settings.GetAntialiasing, m_settings.SetAntialiasing));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("SSAO", m_settings.GetSSAO, m_settings.SetSSAO));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Bloom", m_settings.GetBloom, m_settings.SetBloom));
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Motion Blur", m_settings.GetMotionBlur, m_settings.SetMotionBlur));

        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Audio";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsSlider, panel_setingsContent).GetComponent<PanelSlider>().Init("Volume", m_settings.GetVolume, m_settings.SetVolume, 0, 1, false));

        UIHelper.Create(prefab_header, panel_setingsContent).GetComponentInChildren<Text>().text = "Other";
        m_settingPanels.Add(UIHelper.Create(prefab_settingsToggle, panel_setingsContent).GetComponent<PanelToggle>().Init("Show FPS", m_settings.GetShowFPS, m_settings.SetShowFPS));

        Navigation middleNav = new Navigation();
        middleNav.mode = Navigation.Mode.Explicit;

        Navigation topNav = middleNav;
        topNav.selectOnUp = btn_applySettings;
        
        Navigation bottomNav = middleNav;

        UIHelper.SetNavigationVertical(m_settingPanels, topNav, middleNav, bottomNav);

        Navigation nav;
        Selectable first = m_settingPanels[0].GetComponentInChildren<Selectable>();

        nav = btn_backSettings.navigation;
        nav.selectOnDown = first;
        btn_backSettings.navigation = nav;

        nav = btn_applySettings.navigation;
        nav.selectOnDown = first;
        btn_applySettings.navigation = nav;

        nav = btn_loadDefalutsSettings.navigation;
        nav.selectOnDown = first;
        btn_loadDefalutsSettings.navigation = nav;


        // Create controls panels
        m_controlPanels = new List<RectTransform>();
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

        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator RefreshScrollbar(Scrollbar scrollbar)
    {
        yield return null;
        yield return null;
        scrollbar.value = 1;
    }

    // Main Menu
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
        RefreshSettings();
    }

    public void OpenControls()
    {
        SetMenu(Menu.Controls);
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Settings
    public void BackToRootMenu()
    {
        SetMenu(Menu.Root);
        RefreshSettings();
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

    // Controls
    public void AcceptControls()
    {
        SetMenu(Menu.Root);
    }

    public void UseDefaultControls()
    {

    }
}