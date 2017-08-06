using System;
using UnityEngine;

public abstract class Prop : MonoBehaviour
{
    [SerializeField]
    private Transform m_armsRoot;
    public Transform ArmsRoot { get { return m_armsRoot; } }

    [SerializeField] [Range(0, 10)]
    private float m_movementAdjustRate = 2.0f;
    [SerializeField] [Range(0, 16)]
    private float m_movementSmoothing = 3.5f;
    [SerializeField] [Range(1, 16)]
    private float m_lookSmoothing = 9.0f;
    [SerializeField] [Range(0, 2)]
    private float m_lookSensitivity = 1.0f;

    private Renderer[] m_renderers;
    private Action m_onHolsterComplete;
    private bool m_lastIsHolstered = true;

    private Animator m_anim;
    private float m_x = 0;
    private float m_y = 0;
    private float m_xLook = 0;
    private float m_yLook = 0;

    private bool m_holster = true;
    public bool Holster { get { return m_holster; } }

    public bool IsHolstered
    {
        get { return m_anim.GetCurrentAnimatorStateInfo(MainAnimatorState).IsTag("Holstered"); }
    }

    public void Init()
    {
        m_anim = GetComponentInChildren<Animator>(true);
        m_renderers = GetComponentsInChildren<Renderer>(true);
        
        SetRenderersVisible(false);
        OnHolster();
    }

    public void MainUpdate()
    {
        if (IsHolstered != m_lastIsHolstered && IsHolstered)
        {
            SetRenderersVisible(false);
            OnHolster();
            m_onHolsterComplete();
        }

        m_lastIsHolstered = IsHolstered;

        LogicUpdate();
    }

    public void VisualUpdate(Vector2 move, Vector2 look, bool jumping, bool running, bool interact)
    {
        float dt = Time.deltaTime;

        float ySpeed = move.y * (running ? 2 : 1);
        m_x = Mathf.MoveTowards(m_x, move.x, dt * m_movementAdjustRate);
        m_y = Mathf.MoveTowards(m_y, ySpeed, dt * m_movementAdjustRate);
        m_x = Mathf.Lerp(m_x, move.x, dt * m_movementSmoothing);
        m_y = Mathf.Lerp(m_y, ySpeed, dt * m_movementSmoothing);

        m_xLook = Mathf.Lerp(m_xLook, look.x, dt * m_lookSmoothing);
        m_yLook = Mathf.Lerp(m_yLook, look.y, dt * m_lookSmoothing);

        m_anim.SetBool("Holster", m_holster);
        m_anim.SetFloat("SpeedX", m_x);
        m_anim.SetFloat("SpeedY", m_y);
        m_anim.SetFloat("LookX", -m_xLook * m_lookSensitivity);
        m_anim.SetFloat("LookY", -m_yLook * m_lookSensitivity);
        m_anim.SetBool("Interact", interact);
        m_anim.SetBool("Jumping", jumping);
        m_anim.SetBool("Running", running);

        AnimUpdate();
    }

    private void SetRenderersVisible(bool isVisible)
    {
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].enabled = isVisible;
        }
    }

    public void DrawProp()
    {
        if (m_holster)
        {
            m_holster = false;
            SetRenderersVisible(true);
            OnDraw();
        }
    }

    public void HolsterProp(Action onComplete)
    {
        if (!m_holster)
        {
            m_holster = true;
            m_onHolsterComplete = onComplete;
        }
    }

    public abstract GameButton HolsterButton { get; }
    protected abstract int MainAnimatorState { get; }

    protected abstract void LogicUpdate();
    protected abstract void AnimUpdate();
    protected abstract void OnDraw();
    protected abstract void OnHolster();

    public abstract void OnFireStart();
    public abstract void OnFireHeld();
    public abstract void OnReload();
    public abstract void CancelActions();
}