using System;
using UnityEngine;
using Framework;
using Framework.InputManagement;
using Framework.SettingManagement;
using Framework.IO;

[DefaultExecutionOrder(100)]
public class ControlsManager : ComponentSingleton<ControlsManager>
{
    private string[] m_buttonToName;
    private string[] m_axisToName;

    private Controls m_controls;

    private Setting<float> m_lookSensitivity;
    public float LookSensitivity
    {
        get { return m_lookSensitivity.Value; }
    }

    public ControlsManager()
    {
        m_buttonToName = Enum.GetNames(typeof(GameButton));
        m_axisToName = Enum.GetNames(typeof(GameAxis));

        m_controls = new Controls();
        InitControls(m_controls);
    }

    public void FixedUpdate()
    {
        m_controls.LateFixedUpdate();
    }

    public void EarlyUpdate()
    {
        m_controls.EarlyUpdate();
    }

    public void SetMuting(bool isMuted)
    {
        m_controls.IsMuted = isMuted;
    }

    public bool IsDown(GameButton button)
    {
        return m_controls.IsDown(m_buttonToName[(int)button]);
    }
    
    public bool JustDown(GameButton button)
    {
        return m_controls.JustDown(m_buttonToName[(int)button]);
    }
    
    public bool JustUp(GameButton button)
    {
        return m_controls.JustUp(m_buttonToName[(int)button]);
    }
    
    public float AverageValue(GameAxis axis)
    {
        return m_controls.AverageValue(m_axisToName[(int)axis]);
    }
    
    public float CumulativeValue(GameAxis axis)
    {
        return m_controls.CumulativeValue(m_axisToName[(int)axis]);
    }

    public void Save()
    {
        FileIO.WriteFile(JsonConverter.ToJson(m_controls.Serialize()), FileIO.GetInstallDirectory(), "Controls.ini");
    }

    public void Load()
    {
        string str = FileIO.ReadFile(FileIO.GetInstallDirectory(), "Controls.ini");
        if (str == null || !m_controls.Deserialize(JsonConverter.FromJson<SerializableControls>(str)))
        {
            m_controls.UseDefaults();
            Save();
        }
    }

    public Controls ControlsCopy()
    {
        Controls controls = new Controls();
        InitControls(controls);
        controls.Deserialize(m_controls.Serialize());
        return controls;
    }

    public void SetControls(Controls controls)
    {
        m_controls.Deserialize(controls.Serialize());
        m_controls.Settings.Apply();
    }

    private void InitControls(Controls controls)
    {
        AddButton(controls,
            GameButton.Menu, "Toggle Menu", false, false,
            new KeyButton(KeyCode.Escape),
            new JoystickButton(GamepadButton.Start)
            );

        AddButton(controls,
            GameButton.Jump, "Jump", true, true,
            new KeyButton(KeyCode.Space),
            new JoystickButton(GamepadButton.A)
            );

        AddButton(controls,
            GameButton.Crouch, "Crouch", true, true,
            new KeyButton(KeyCode.C),
            new JoystickButton(GamepadButton.B)
            );

        AddButton(controls,
            GameButton.RunHold, "Run", true, true,
            new KeyButton(KeyCode.LeftShift)
            );

        AddButton(controls,
            GameButton.RunTap, "Tap to Run", true, true,
            new JoystickButton(GamepadButton.LStick)
            );

        AddButton(controls,
            GameButton.Fire, "Fire", true, true,
            new KeyButton(KeyCode.Mouse0),
            new JoystickButton(GamepadButton.RTrigger)
            );

        AddButton(controls,
            GameButton.Aim, "Aim", true, true,
            new KeyButton(KeyCode.Mouse1),
            new JoystickButton(GamepadButton.LTrigger)
            );

        AddButton(controls,
            GameButton.AimToggle, "Aim Toggle", true, true
            );

        AddButton(controls,
            GameButton.Reload, "Reload", true, true,
            new KeyButton(KeyCode.R),
            new JoystickButton(GamepadButton.Y)
            );

        AddButton(controls,
            GameButton.Interact, "Interact", true, true,
            new KeyButton(KeyCode.F),
            new JoystickButton(GamepadButton.X)
            );
        
        AddButton(controls, GameButton.Weapon1, "Weapon 1", true, true,
            new KeyButton(KeyCode.Alpha1),
            new JoystickButton(GamepadButton.DpadLeft)
            );

        AddButton(controls,
            GameButton.Flashlight, "Flashlight", true, true,
            new KeyButton(KeyCode.L),
            new JoystickButton(GamepadButton.DpadUp)
            );
        

        AddAxis(controls,
            GameAxis.MoveY, "Strafe", true, true, 1f,
            new KeyAxis(KeyCode.S, KeyCode.W),
            new JoystickAxis(GamepadAxis.LStickY)
            );

        AddAxis(controls,
            GameAxis.MoveX, "Move", true, true, 1f,
            new KeyAxis(KeyCode.A, KeyCode.D),
            new JoystickAxis(GamepadAxis.LStickX)
            );

        AddAxis(controls,
            GameAxis.LookX, "Look Horizontal", true, true, 2f,
            new MouseAxis(MouseAxis.Axis.MouseX),
            new JoystickAxis(GamepadAxis.RStickX)
            );

        AddAxis(controls,
            GameAxis.LookY, "Look Vertical", true, true, 2f,
            new MouseAxis(MouseAxis.Axis.MouseY),
            new JoystickAxis(GamepadAxis.RStickY)
            );

        m_lookSensitivity = controls.Settings.Add("General", "Look Sensitivity", 1, 0, 2, false);
    }

    private void AddButton(Controls controls, GameButton button, string displayName, bool canRebind, bool canBeMuted, params ISource<bool>[] defaultBindings)
    {
        controls.AddButton(button.ToString(), displayName, canRebind, canBeMuted, defaultBindings);
    }

    private void AddAxis(Controls controls, GameAxis axis, string displayName, bool canRebind, bool canBeMuted, float exponent, params ISource<float>[] defaultBindings)
    {
        controls.AddAxis(axis.ToString(), displayName, canRebind, canBeMuted, exponent, defaultBindings);
    }
}