using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputController;


// Actions needing a key binding.
public enum GameButton
{
    Menu,
    Fire,
    Aim,
    Reload,
    Jump,
    Run,
    Crouch,
    Interact,
}

// Actions needing an axis binding.
public enum GameAxis
{
    LookX,
    LookY,
    MoveX,
    MoveY,
}


/*
 * Stores and maintains user constrols.
 */
[RequireComponent(typeof(ControlsEarlyUpdate))]
public class Controls : MonoBehaviour
{
    private static Dictionary<GameButton, BufferedButton> m_buttons;
    public static Dictionary<GameButton, BufferedButton> Buttons
    {
        get { return m_buttons; }
    }

    private static Dictionary<GameAxis, BufferedAxis> m_axis;
    public static Dictionary<GameAxis, BufferedAxis> Axis
    {
        get { return m_axis; }
    }

    private static bool m_isMuted = false;
    public static bool IsMuted
    {
        get { return m_isMuted; }
        set { m_isMuted = value; }
    }

    private void Awake()
    {
        loadDefaultControls();
    }

    /*
     * Needs to run at the end of every FixedUpdate frame to handle the input buffers.
     */
    private void FixedUpdate()
    {
        foreach (BufferedButton button in m_buttons.Values)
        {
            button.RecordFixedUpdateState();
        }
        foreach (BufferedAxis axis in m_axis.Values)
        {
            axis.RecordFixedUpdateState();
        }
    }

    /*
     * Needs to run at the start of every Update frame to buffer new inputs.
     */
    public void EarlyUpdate()
    {
        foreach (BufferedButton button in m_buttons.Values)
        {
            button.RecordUpdateState();
        }
        foreach (BufferedAxis axis in m_axis.Values)
        {
            axis.RecordUpdateState();
        }
    }

    /*
     * Clears the current controls and replaces them with the default set.
     */
    public static void loadDefaultControls()
    {
        m_buttons = new Dictionary<GameButton, BufferedButton>();
        
        m_buttons.Add(GameButton.Menu, new BufferedButton(false, new List<ISource<bool>>
        {
            new KeyButton(KeyCode.Escape),
            new JoystickButton(GamepadButton.Start),
        }));
        m_buttons.Add(GameButton.Jump, new BufferedButton(true, new List<ISource<bool>>
        {
            new KeyButton(KeyCode.Space),
            new JoystickButton(GamepadButton.A),
        }));
        m_buttons.Add(GameButton.Run, new BufferedButton(true, new List<ISource<bool>>
        {
            new KeyButton(KeyCode.LeftShift),
            new JoystickButton(GamepadButton.LStick),
        }));
        m_buttons.Add(GameButton.Crouch, new BufferedButton(true, new List<ISource<bool>>
        {
            new KeyButton(KeyCode.C),
            new JoystickButton(GamepadButton.B),
        }));
        m_buttons.Add(GameButton.Fire, new BufferedButton(true, new List<ISource<bool>>
        {
            new KeyButton(KeyCode.Mouse0),
            new JoystickButton(GamepadButton.RTrigger),
        }));
        m_buttons.Add(GameButton.Aim, new BufferedButton(true, new List<ISource<bool>>
        {
            new KeyButton(KeyCode.Mouse1),
            new JoystickButton(GamepadButton.LTrigger),
        }));
        m_buttons.Add(GameButton.Reload, new BufferedButton(true, new List<ISource<bool>>
        {
            new KeyButton(KeyCode.R),
            new JoystickButton(GamepadButton.Y),
        }));
        m_buttons.Add(GameButton.Interact, new BufferedButton(true, new List<ISource<bool>>
        {
            new KeyButton(KeyCode.F),
            new JoystickButton(GamepadButton.X),
        }));


        m_axis = new Dictionary<GameAxis, BufferedAxis>();

        m_axis.Add(GameAxis.MoveY, new BufferedAxis(true, 1f, new List<ISource<float>>
        {
            new KeyAxis(KeyCode.S, KeyCode.W),
            new JoystickAxis(GamepadAxis.LStickY),
        }));
        m_axis.Add(GameAxis.MoveX, new BufferedAxis(true, 1f, new List<ISource<float>>
        {
            new KeyAxis(KeyCode.A, KeyCode.D),
            new JoystickAxis(GamepadAxis.LStickX),
        }));
        m_axis.Add(GameAxis.LookX, new BufferedAxis(true, 2f, new List<ISource<float>>
        {
            new MouseAxis(MouseAxis.Axis.MouseX),
            new JoystickAxis(GamepadAxis.RStickX),
        }));
        m_axis.Add(GameAxis.LookY, new BufferedAxis(true, 2f, new List<ISource<float>>
        {
            new MouseAxis(MouseAxis.Axis.MouseY),
            new JoystickAxis(GamepadAxis.RStickY),
        }));
    }

    /*
     * Returns true if any of the relevant keyboard or joystick buttons are held down.
     */
    public static bool IsDown(GameButton button)
    {
        BufferedButton bufferedButton = m_buttons[button];
        return !(m_isMuted && bufferedButton.CanBeMuted) && bufferedButton.IsDown();
    }

    /*
     * Returns true if a relevant keyboard or joystick key was pressed since the last appropriate update.
     */
    public static bool JustDown(GameButton button)
    {
        BufferedButton bufferedButton = m_buttons[button];
        bool isFixed = (Time.deltaTime == Time.fixedDeltaTime);
        return !(m_isMuted && bufferedButton.CanBeMuted) && (isFixed ? bufferedButton.JustDown() : bufferedButton.VisualJustDown());
    }

    /*
     * Returns true if a relevant keyboard or joystick key was released since the last appropriate update.
     */
    public static bool JustUp(GameButton button)
    {
        BufferedButton bufferedButton = m_buttons[button];
        bool isFixed = (Time.deltaTime == Time.fixedDeltaTime);
        return !(m_isMuted && bufferedButton.CanBeMuted) && (isFixed ? bufferedButton.JustUp() : bufferedButton.VisualJustUp());
    }

    /*
     * Returns the average value of an axis from all Update frames since the last FixedUpdate.
     */
    public static float AverageValue(GameAxis axis)
    {
        BufferedAxis bufferedAxis = m_axis[axis];
        return (m_isMuted && bufferedAxis.CanBeMuted) ? 0 : bufferedAxis.AverageValue();
    }

    /*
     * Returns the cumulative value of an axis from all Update frames since the last FixedUpdate.
     */
    public static float CumulativeValue(GameAxis axis)
    {
        BufferedAxis bufferedAxis = m_axis[axis];
        return (m_isMuted && bufferedAxis.CanBeMuted) ? 0 : bufferedAxis.CumulativeValue();
    }


    public enum RebindState { None, Button, Axis, KeyAxis, ButtonAxis }
    private static RebindState m_rebindState = RebindState.None;
    public static RebindState rebindState
    {
        get { return m_rebindState; }
    }

    private static BufferedButton m_rebindingButton = null;
    private static BufferedAxis m_rebindingAxis = null;
    private static List<KeyCode> m_rebindingPreviousKeys = new List<KeyCode>();
    private static List<GamepadButton> m_rebindingPreviousButtons = new List<GamepadButton>();
    private static KeyCode m_rebindingAxisKey;
    private static GamepadButton m_rebindingAxisButton;
    private static Action m_onRebindComplete;

    /*
     * When rebinding detects any appropriate inputs.
     */
    private void Update()
    {
        if (m_rebindState == RebindState.Axis)
        {
            ISource<float> source = GetAxisSource();
            if (source != null)
            {
                m_rebindState = RebindState.None;
                m_rebindingAxis.AddSource(source);
                m_onRebindComplete();
            }
            else
            {
                List<KeyCode> activeKeys = FindActiveKeys(true);
                List<GamepadButton> activeButtons = FindActiveButtons(true);
                if (activeButtons.Count > 0)
                {
                    m_rebindState = RebindState.ButtonAxis;
                    m_rebindingAxisButton = activeButtons.First();
                }
                else if (activeKeys.Count > 0)
                {
                    m_rebindState = RebindState.KeyAxis;
                    m_rebindingAxisKey = activeKeys.First();
                }
            }
        }
        else if (m_rebindState == RebindState.Button)
        {
            ISource<bool> source = GetButtonSource();
            if (source != null)
            {
                m_rebindState = RebindState.None;
                m_rebindingButton.AddSource(source);
                m_onRebindComplete();
            }
        }
        else if (m_rebindState == RebindState.ButtonAxis)
        {
            List<GamepadButton> activeButtons = FindActiveButtons(true);
            if (activeButtons.Count > 0)
            {
                m_rebindState = RebindState.None;
                m_rebindingAxis.AddSource(new JoystickButtonAxis(activeButtons.First(), m_rebindingAxisButton));
                m_onRebindComplete();
            }
        }
        else if (m_rebindState == RebindState.KeyAxis)
        {
            List<KeyCode> activeKeys = FindActiveKeys(true);
            if (activeKeys.Count > 0)
            {
                m_rebindState = RebindState.None;
                m_rebindingAxis.AddSource(new KeyAxis(activeKeys.First(), m_rebindingAxisKey));
                m_onRebindComplete();
            }
        }

        m_rebindingPreviousKeys = FindActiveKeys(false);
        m_rebindingPreviousButtons = FindActiveButtons(false);
    }
    
    public static void AddBinding(BufferedButton button, Action onRebindComplete)
    {
        if (m_rebindState == RebindState.None)
        {
            m_rebindState = RebindState.Button;
            m_rebindingButton = button;

            m_onRebindComplete = onRebindComplete;
            m_rebindingPreviousKeys = FindActiveKeys(false);
            m_rebindingPreviousButtons = FindActiveButtons(false);
        }
    }

    public static void AddBinding(BufferedAxis axis, Action onRebindComplete)
    {
        if (m_rebindState == RebindState.None)
        {
            m_rebindState = RebindState.Axis;
            m_rebindingAxis = axis;

            m_onRebindComplete = onRebindComplete;
            m_rebindingPreviousKeys = FindActiveKeys(false);
            m_rebindingPreviousButtons = FindActiveButtons(false);
        }
    }

    private static ISource<bool> GetButtonSource()
    {
        List<GamepadButton> activeButtons = FindActiveButtons(true);
        if (activeButtons.Count > 0)
        {
            return new JoystickButton(activeButtons.First());
        }
        List<KeyCode> activeKeys = FindActiveKeys(true);
        if (activeKeys.Count > 0)
        {
            return new KeyButton(activeKeys.First());
        }
        return null;
    }
    
    private static ISource<float> GetAxisSource()
    {
        foreach (GamepadAxis axis in Enum.GetValues(typeof(GamepadAxis)))
        {
            if (JoystickAxis.GetAxisValue(axis) > 0.5f)
            {
                return new JoystickAxis(axis);
            }
        }
        foreach (MouseAxis.Axis axis in Enum.GetValues(typeof(MouseAxis.Axis)))
        {
            if (MouseAxis.GetAxisValue(axis) > 0.5f)
            {
                return new MouseAxis(axis);
            }
        }
        return null;
    }

    private static List<KeyCode> FindActiveKeys(bool ignorePrevious)
    {
        return Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().Where(
            button => KeyButton.GetButtonValue(button) && (!ignorePrevious || !m_rebindingPreviousKeys.Contains(button))
            ).ToList();
    }

    private static List<GamepadButton> FindActiveButtons(bool ignorePrevious)
    {
        return Enum.GetValues(typeof(GamepadButton)).Cast<GamepadButton>().Where(
            button => JoystickButton.GetButtonValue(button) && (!ignorePrevious || !m_rebindingPreviousButtons.Contains(button))
            ).ToList();
    }
}