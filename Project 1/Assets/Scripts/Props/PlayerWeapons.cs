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
    private Dictionary<string, int> m_boneNamesToIndex;
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
        
        m_props = GetComponentsInChildren<Prop>(true);
        foreach (Prop prop in m_props)
        {
            prop.Init();
        }
        m_targetProp = m_props.First();
    }

    private void OnInteractOnce()
    {
        if (m_activeProp != null)
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
        m_boneNamesToIndex = new Dictionary<string, int>();
        for (int i = 0; i < m_arms.bones.Length; i++)
        {
            if (m_arms.bones[i] != null)
            {
                m_boneNamesToIndex.Add(m_arms.bones[i].name, i);
            }
        }

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

        foreach (Prop prop in m_props)
        {
            prop.MainUpdate();
        }
        
        if (m_activeProp == null && m_targetProp != null && !m_interact.IsInteracting)
        {
            m_activeProp = m_targetProp;
            m_activeProp.DrawProp();

            // make the arms follow the active prop rig
            m_arms.bones = m_arms.bones.Where(b => b == null).Concat(
                m_activeProp.ArmsRoot.GetComponentsInChildren<Transform>()
                .Where(t => m_boneNamesToIndex.ContainsKey(t.name))
                .OrderBy(t => m_boneNamesToIndex[t.name])
                ).ToArray();

            m_audio.PlayOneShot(Utils.PickRandom(m_drawSounds));
        }
        
        if (m_activeProp != null && !m_activeProp.Holster && !m_interact.IsInteracting)
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

        // only render the arms if they are raised
        m_arms.enabled = IsPropActive;
    }

    public void UpdateVisuals(PlayerInput input, PlayerLook look, CharacterMovement movement)
    {
        foreach (Prop prop in m_props)
        {
            prop.VisualUpdate(
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
        if (m_activeProp != null)
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
