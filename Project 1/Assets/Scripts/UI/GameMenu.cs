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
    [Header("Audio")]
    [SerializeField] private AudioClip m_menuOpenSound;
    [SerializeField] private AudioClip m_cancelSound;
    [SerializeField] private AudioClip m_selectSound;
    [SerializeField] private AudioClip m_deselectSound;
    [SerializeField] private AudioClip m_submitSound;
    [SerializeField] private AudioClip m_sliderSound;

    [Header("Prefabs")]
    [SerializeField] private RectTransform prefab_header;
    [SerializeField] private PanelToggle prefab_settingsToggle;
    [SerializeField] private PanelSlider prefab_settingsSlider;
    [SerializeField] private PanelDropdown prefab_settingsDropdown;
    [SerializeField] private RectTransform prefab_headerBindings;
    [SerializeField] private PanelControlBinding prefab_controlBindings;
    [SerializeField] private PanelRebind prefab_binding;

    [Header("Root Menu")]
    [SerializeField] private Canvas m_menuRoot;
    [SerializeField] private Button m_btnResume;
    [SerializeField] private Button m_btnOpenSettings;
    [SerializeField] private Button m_btnOpenControls;
    [SerializeField] private Button m_btnRestart;
    [SerializeField] private Button m_btnQuit;

    [Header("Settings Menu")]
    [SerializeField] private Canvas m_menuSettings;
    [SerializeField] private RectTransform m_rectSettings;
    [SerializeField] private Button m_btnBackSettings;
    [SerializeField] private Button m_btnApplySettings;
    [SerializeField] private Button m_btnLoadDefaultSettings;
    [SerializeField] private RectTransform m_contentSettings;
    [SerializeField] private Scrollbar m_scrollbarSettings;

    [Header("Controls Menu")]
    [SerializeField] private Canvas m_menuControls;
    [SerializeField] private RectTransform m_rectControls;
    [SerializeField] private Button m_btnBackControls;
    [SerializeField] private Button m_btnApplyControls;
    [SerializeField] private Button m_btnUseDefaultControls;
    [SerializeField] private RectTransform m_contentControls;
    [SerializeField] private Scrollbar m_scrollbarControls;

    [Header("Bindings Menu")]
    [SerializeField] private Canvas m_menuBindings;
    [SerializeField] private RectTransform m_contentBindings;
    [SerializeField] private Text m_txtBindingsTitle;
    [SerializeField] private Button m_btnBackBindings;
    [SerializeField] private Button m_btnNewBinding;

    [Header("Rebinding Menu")]
    [SerializeField] private Canvas m_menuRebinding;
    [SerializeField] private Text m_txtRebindTitle;
    [SerializeField] private Text m_txtRebindMessage;


    private enum Menu
    {
        None,
        Root,
        Settings,
        Controls,
        Bindings,
        Rebinding
    }

    private AudioSource m_audio;
    private Menu m_activeMenu;
    private List<ISettingPanel> m_settingPanels;
    private List<ISettingPanel> m_controlPanels;
    private Controls m_editControls;
    private IInputSource m_editSource;


    private bool IsMenuOpen
    {
        get { return m_activeMenu != Menu.None; }
    }

    private void Awake()
    {
        m_audio = GetComponent<AudioSource>();

        m_btnResume.onClick.AddListener(() =>           SetMenu(Menu.None));
        m_btnOpenSettings.onClick.AddListener(() =>     SetMenu(Menu.Settings));
        m_btnOpenControls.onClick.AddListener(() =>     SetMenu(Menu.Controls));
        m_btnRestart.onClick.AddListener(() =>          Main.Instance.LoadMainScene());
        m_btnQuit.onClick.AddListener(() =>             Application.Quit());

        m_btnBackSettings.onClick.AddListener(() =>         SetMenu(Menu.Root));
        m_btnApplySettings.onClick.AddListener(() =>        ApplySettings());
        m_btnLoadDefaultSettings.onClick.AddListener(() =>  LoadDefaultSettings());

        m_btnBackControls.onClick.AddListener(() =>         SetMenu(Menu.Root));
        m_btnApplyControls.onClick.AddListener(() =>        ApplyControls());
        m_btnUseDefaultControls.onClick.AddListener(() =>   UseDefaultControls());
        
        m_btnBackBindings.onClick.AddListener(() =>     SetMenu(Menu.Controls));
        m_btnNewBinding.onClick.AddListener(() =>       AddBinding());
    }

    private void Start()
    {
        CreateSettings();
        CreateControls();

        SetMenu(Menu.None);
    }

    private void Update()
    {
        // free the cursor if a menu is open
        Cursor.lockState = IsMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsMenuOpen;

        // prevent unwanted user input when menu is open
        ControlsManager.Instance.SetMuting(IsMenuOpen);

        // toggle the menu if the menu button was hit, or if the cancel button was hit go back a menu
        if (m_editControls == null || m_editControls.rebindState == Controls.RebindState.None)
        {
            if (ControlsManager.Instance.JustDown(GameButton.Menu))
            {
                SetMenu(IsMenuOpen ? Menu.None : Menu.Root);
                m_audio.PlayOneShot(IsMenuOpen ? m_menuOpenSound : m_cancelSound);
            }
            else if (IsMenuOpen && Input.GetButtonDown("Cancel"))
            {
                switch (m_activeMenu)
                {
                    case Menu.Root: SetMenu(Menu.None); break;
                    case Menu.Settings:
                    case Menu.Controls: SetMenu(Menu.Root); break;
                    case Menu.Bindings: SetMenu(Menu.Controls); break;
                }
                m_audio.PlayOneShot(m_cancelSound);
            }
        }

        // ensure there is always something selected so that controllers can always be used
        if (EventSystem.current.currentSelectedGameObject == null && (
            Mathf.Abs(Input.GetAxis("Horizontal")) > 0.3f ||
            Mathf.Abs(Input.GetAxis("Vertical")) > 0.3f
            ))
        {
            switch (m_activeMenu)
            {
                case Menu.Root: m_btnResume.Select(); break;
                case Menu.Settings: m_btnApplySettings.Select(); break;
                case Menu.Controls: m_btnApplyControls.Select(); break;
                case Menu.Bindings: m_btnBackBindings.Select(); break;
            }
        }

        // handle key rebinding
        if (m_editControls != null)
        {
            m_editControls.UpdateRebinding();
            switch (m_editControls.rebindState)
            {
                case Controls.RebindState.Button:   m_txtRebindMessage.text = "Press any key..."; break;
                case Controls.RebindState.Axis:     m_txtRebindMessage.text = "Use any axis, or press a key for the axis <b>positive</b> direction..."; break;
                case Controls.RebindState.ButtonAxis:
                case Controls.RebindState.KeyAxis:  m_txtRebindMessage.text = "Press a key for the axis <b>negative</b> direction..."; break;
            }
        }

        // size the menus to fit the screen best
        float edgeMargin = 50;
        float maxMenuHeight = 800;
        float height = Mathf.Min(Screen.height - 2 * (edgeMargin), maxMenuHeight);

        m_rectSettings.sizeDelta = new Vector2(m_rectSettings.sizeDelta.x, height);
        m_rectControls.sizeDelta = new Vector2(m_rectControls.sizeDelta.x, height);
    }

    private void SetMenu(Menu menu)
    {
        m_menuRoot.enabled = (menu == Menu.Root);
        m_menuSettings.enabled = (menu == Menu.Settings);
        m_menuControls.enabled = (menu == Menu.Controls);
        m_menuBindings.enabled = (menu == Menu.Bindings);
        m_menuRebinding.enabled = (menu == Menu.Rebinding);

        bool fromRoot = m_activeMenu == Menu.Root;
        m_activeMenu = menu;

        if (fromRoot)
        {
            StartCoroutine(RefreshScrollbar(m_scrollbarSettings));
            StartCoroutine(RefreshScrollbar(m_scrollbarControls));
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
            m_txtRebindTitle.text = "Rebinding \"" + controlName + "\"";
            m_txtBindingsTitle.text = "Bindings for \"" + controlName + "\"";
        }

        EventSystem.current.SetSelectedGameObject(null);
    }
    
    private void OpenBindings(IInputSource source)
    {
        m_editSource = source;
        SetMenu(Menu.Bindings);
    }

    private void OnRebindComplete()
    {
        SetMenu(Menu.Bindings);
    }

    private void ApplySettings()
    {
        m_settingPanels.ForEach(s => s.Apply());
        UpdateSettings();
    }

    private void LoadDefaultSettings()
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

    private void ApplyControls()
    {
        m_controlPanels.ForEach(s => s.Apply());
        UpdateControls();
    }

    private void UseDefaultControls()
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

    private void AddBinding()
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
        m_contentBindings.Cast<Transform>().ToList().ForEach(panel => Destroy(panel.gameObject));

        List<PanelRebind> bindingPanels = new List<PanelRebind>();

        int index = 0;
        foreach (SourceInfo info in m_editSource.SourceInfos)
        {
            bindingPanels.Add(UIHelper.Create(prefab_binding, m_contentBindings).Init(info, index++, Rebind, RemoveBinding));
        }

        if (bindingPanels.Count > 1)
        {
            for (int i = 0; i < bindingPanels.Count; i++)
            {
                PanelRebind current = bindingPanels[i];

                if (i == 0)
                {
                    PanelRebind down = bindingPanels[i + 1];
                    current.SetNav(null, null, down.buttonBinding, down.buttonRemove);
                }
                else if (i == bindingPanels.Count - 1)
                {
                    PanelRebind up = bindingPanels[i - 1];
                    current.SetNav(up.buttonBinding, up.buttonRemove, m_btnNewBinding, m_btnNewBinding);

                    Navigation nav = m_btnNewBinding.navigation;
                    nav.selectOnUp = current.buttonBinding;
                    m_btnNewBinding.navigation = nav;
                }
                else
                {
                    PanelRebind up = bindingPanels[i - 1];
                    PanelRebind down = bindingPanels[i + 1];
                    current.SetNav(up.buttonBinding, up.buttonRemove, down.buttonBinding, down.buttonRemove);
                }
            }
        }
        else if (bindingPanels.Count > 0)
        {
            bindingPanels[0].SetNav(null, null, m_btnNewBinding, m_btnNewBinding);

            Navigation nav = m_btnNewBinding.navigation;
            nav.selectOnUp = bindingPanels[0].buttonBinding;
            m_btnNewBinding.navigation = nav;
        }
    }

    private void CreateSettings()
    {
        m_settingPanels = new List<ISettingPanel>();

        float spacing = 10;

        UIHelper.AddSpacer(m_contentSettings, spacing);
        CreateSettingsPanels(m_contentSettings, m_settingPanels, spacing, () => SettingManager.Instance.Settings);

        Navigation explicitNav = new Navigation();
        explicitNav.mode = Navigation.Mode.Explicit;

        Navigation topNav = explicitNav;
        topNav.selectOnUp = m_btnApplySettings;

        Selectable firstSetting = UIHelper.SetNavigationVertical(m_contentSettings, topNav, explicitNav, explicitNav).FirstOrDefault();
        Navigation tempNav;

        tempNav = m_btnBackSettings.navigation;
        tempNav.selectOnDown = firstSetting;
        m_btnBackSettings.navigation = tempNav;

        tempNav = m_btnApplySettings.navigation;
        tempNav.selectOnDown = firstSetting;
        m_btnApplySettings.navigation = tempNav;

        tempNav = m_btnLoadDefaultSettings.navigation;
        tempNav.selectOnDown = firstSetting;
        m_btnLoadDefaultSettings.navigation = tempNav;
    }

    private void CreateControls()
    {
        m_controlPanels = new List<ISettingPanel>();
        m_editControls = ControlsManager.Instance.ControlsCopy();

        float spacing = 10;

        UIHelper.AddSpacer(m_contentControls, spacing);
        CreateSettingsPanels(m_contentControls, m_controlPanels, spacing, () => m_editControls.Settings);
        
        UIHelper.Create(prefab_headerBindings, m_contentControls);
        // TODO: Order by some value
        foreach (GameButton button in Enum.GetValues(typeof(GameButton)))
        {
            string name = button.ToString();
            if (m_editControls.NameToButton.ContainsKey(name) && m_editControls.NameToButton[name].CanRebind)
            {
                m_controlPanels.Add(UIHelper.Create(prefab_controlBindings, m_contentControls).Init(() => m_editControls.NameToButton[name], OpenBindings));
            }
        }
        foreach (GameAxis axis in Enum.GetValues(typeof(GameAxis)))
        {
            string name = axis.ToString();
            if (m_editControls.NameToAxis.ContainsKey(name) && m_editControls.NameToAxis[name].CanRebind)
            {
                m_controlPanels.Add(UIHelper.Create(prefab_controlBindings, m_contentControls).Init(() => m_editControls.NameToAxis[name], OpenBindings));
            }
        }
        UIHelper.AddSpacer(m_contentControls, spacing);

        Navigation explicitNav = new Navigation();
        explicitNav.mode = Navigation.Mode.Explicit;

        Navigation topNav = explicitNav;
        topNav.selectOnUp = m_btnApplyControls;
        UIHelper.SetNavigationVertical(m_contentControls, topNav, explicitNav, explicitNav);

        Selectable firstControl = UIHelper.SetNavigationVertical(m_contentControls, topNav, explicitNav, explicitNav).FirstOrDefault();
        Navigation tempNav;

        tempNav = m_btnBackControls.navigation;
        tempNav.selectOnDown = firstControl;
        m_btnBackControls.navigation = tempNav;

        tempNav = m_btnApplyControls.navigation;
        tempNav.selectOnDown = firstControl;
        m_btnApplyControls.navigation = tempNav;

        tempNav = m_btnUseDefaultControls.navigation;
        tempNav.selectOnDown = firstControl;
        m_btnUseDefaultControls.navigation = tempNav;
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

    public void PlaySound(AudioClip clip)
    {
        m_audio.PlayOneShot(clip);
    }

    public void PlaySelectSound()
    {
        m_audio.PlayOneShot(m_selectSound);
    }

    public void PlayDeselectSound()
    {
        m_audio.PlayOneShot(m_deselectSound);
    }

    public void PlaySubmitSound()
    {
        m_audio.PlayOneShot(m_submitSound);
    }

    public void PlayDragSound()
    {
        m_audio.PlayOneShot(m_sliderSound);
    }
}