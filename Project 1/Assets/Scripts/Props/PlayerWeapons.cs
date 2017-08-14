using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Framework;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer m_arms;
    [SerializeField]
    private AudioClip[] m_holsterSounds;
    [SerializeField]
    private AudioClip[] m_drawSounds;

    private AudioSource m_audio;
    private PlayerInteract m_interact;
    private Prop[] m_props;
    private Prop m_targetProp;
    private Prop m_activeProp;

    public bool IsPropActive
    {
        get { return m_activeProp != null; }
    }

    private float m_recoil = 0;
    public float Recoil
    {
        get { return m_recoil; }
        set { m_recoil = value; }
    }

    private void Awake()
    {
        m_audio = GetComponent<AudioSource>();
        m_interact = GetComponentInParent<PlayerInteract>();

        m_interact.InteractOnce += OnInteractOnce;
        m_interact.InteractStart += OnInteractStart;

        Dictionary<string, int> boneNamesToIndex = new Dictionary<string, int>();
        for (int i = 0; i < m_arms.bones.Length; i++)
        {
            if (m_arms.bones[i] != null)
            {
                boneNamesToIndex.Add(m_arms.bones[i].name, i);
            }
        }

        m_props = GetComponentsInChildren<Prop>(true);
        foreach (Prop prop in m_props)
        {
            prop.Init(m_arms, boneNamesToIndex);
        }
    }

    private void OnInteractOnce()
    {
        if (IsPropActive)
        {
            m_activeProp.CancelActions();
        }
    }

    private void OnInteractStart()
    {
        HolsterActive();
    }

    private void Start()
    {
        m_targetProp = m_props.First();
        m_targetProp.DrawProp();
    }

    public void UpdateWeapons()
    {
        foreach (Prop prop in m_props)
        {
            if (ControlsManager.Instance.JustDown(prop.HolsterButton))
            {
                m_targetProp = (m_targetProp == prop) ? null : prop;
                HolsterActive();
            }
        }
        
        if (!IsPropActive && m_targetProp != null && !m_interact.IsInteracting)
        {
            m_activeProp = m_targetProp;
            m_activeProp.DrawProp();
            m_arms.bones = m_activeProp.ArmBones;
            m_audio.PlayOneShot(Utils.PickRandom(m_drawSounds));
        }

        m_arms.enabled = IsPropActive;

        if (IsPropActive)
        {
            m_activeProp.MainUpdate();
        }
        
        if (IsPropActive && !m_activeProp.Holster && !m_interact.IsInteracting)
        {
            if (ControlsManager.Instance.JustDown(GameButton.Fire))
            {
                m_activeProp.OnFireStart();
            }
            if (ControlsManager.Instance.IsDown(GameButton.Fire))
            {
                m_activeProp.OnFireHeld();
            }
            if (ControlsManager.Instance.JustDown(GameButton.Reload))
            {
                m_activeProp.OnReload();
            }
        }
    }

    public void UpdateVisuals(PlayerInput input, PlayerLook look, CharacterMovement movement)
    {
        if (IsPropActive)
        {
            m_activeProp.VisualUpdate(
               input.DesiredMove,
               look.GetLookDelta(),
               movement.IsJumping,
               movement.IsRunning,
               false
               );
        }
    }

    private void HolsterActive()
    {
        if (IsPropActive)
        {
            m_activeProp.CancelActions();
            m_activeProp.HolsterProp(OnHolsterComplete);
            m_audio.PlayOneShot(Utils.PickRandom(m_holsterSounds));
        }
    }

    private void OnHolsterComplete()
    {
        m_activeProp = null;
    }
}
