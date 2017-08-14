using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Prop : MonoBehaviour
{
    [SerializeField]
    private Transform m_armsRoot;
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
    private bool m_lastHolstered = true;

    private Animator m_animator;
    private float m_x = 0;
    private float m_y = 0;
    private float m_xLook = 0;
    private float m_yLook = 0;

    private bool m_holster = true;
    public bool Holster { get { return m_holster; } }

    public bool IsHolstered
    {
        get { return !gameObject.activeSelf; }
    }

    private Transform[] m_armBones;
    public Transform[] ArmBones { get { return m_armBones; } }

    public void Init(SkinnedMeshRenderer arms, Dictionary<string, int> boneNamesToIndex)
    {
        m_animator = GetComponentInChildren<Animator>(true);
        m_renderers = GetComponentsInChildren<Renderer>(true);

        m_armBones = arms.bones.Where(b => b == null).Concat(
            m_armsRoot.GetComponentsInChildren<Transform>(true)
            .Where(t => boneNamesToIndex.ContainsKey(t.name))
            .OrderBy(t => boneNamesToIndex[t.name])
            ).ToArray();
        
        gameObject.SetActive(false);
        OnHolster();
    }

    public void MainUpdate()
    {
        bool holstered = m_animator.GetCurrentAnimatorStateInfo(MainAnimatorState).IsTag("Holstered");

        if (holstered != m_lastHolstered && holstered)
        {
            OnHolster();
            m_onHolsterComplete();
            gameObject.SetActive(false);
        }
        m_lastHolstered = holstered;

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

        m_animator.SetBool("Holster", m_holster);
        m_animator.SetFloat("SpeedX", m_x);
        m_animator.SetFloat("SpeedY", m_y);
        m_animator.SetFloat("LookX", -m_xLook * m_lookSensitivity);
        m_animator.SetFloat("LookY", -m_yLook * m_lookSensitivity);
        m_animator.SetBool("Interact", interact);
        m_animator.SetBool("Jumping", jumping);
        m_animator.SetBool("Running", running);

        AnimUpdate();
    }

    public void DrawProp()
    {
        if (m_holster)
        {
            m_holster = false;
            gameObject.SetActive(true);
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