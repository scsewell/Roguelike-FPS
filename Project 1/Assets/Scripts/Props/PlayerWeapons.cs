using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Framework;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer m_arms;
    [SerializeField]
    private AudioSource m_holsterAudio;
    [SerializeField]
    private AudioClip[] m_holsterSounds;
    [SerializeField]
    private AudioSource m_drawAudio;
    [SerializeField]
    private AudioClip[] m_drawSounds;

    private MainGun m_weapon1;
    private Flashlight m_flashlight;
    private PlayerInteract m_interact;
    private Dictionary<string, int> m_boneNamesToIndex;
    private Dictionary<IProp, List<Renderer>> m_propToRenderers;
    private List<IProp> m_props;
    private IProp m_targetProp;

    private IProp m_activeProp;
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
        m_interact = GetComponentInParent<PlayerInteract>();

        m_interact.InteractOnce += OnInteractOnce;
        m_interact.InteractStart += OnInteractStart;

        m_weapon1 = GetComponentInChildren<MainGun>();
        m_flashlight = GetComponentInChildren<Flashlight>();

        m_props = new List<IProp>();
        m_props.Add(m_weapon1);
        m_props.Add(m_flashlight);

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

        m_propToRenderers = new Dictionary<IProp, List<Renderer>>();
        foreach (IProp prop in m_props)
        {
            prop.Holster = true;
            List<Renderer> rendrers = prop.GameObject.GetComponentsInChildren<Renderer>().ToList();
            m_propToRenderers.Add(prop, rendrers);
            rendrers.ForEach(r => r.enabled = false);
        }

        m_targetProp = m_props.First();
        m_targetProp.Holster = false;
    }

    public void UpdateWeapons()
    {
        // draw or holster specific items
        DrawProp(GameButton.Weapon1, m_weapon1);
        DrawProp(GameButton.Flashlight, m_flashlight);
        
        if (m_props.Any(p => !p.IsHolstered))
        {
            IProp currentlyActive = m_props.First(p => !p.IsHolstered);
            if (currentlyActive != m_activeProp)
            {
                m_activeProp = currentlyActive;
                // make the arms follow the active prop rig
                m_arms.bones = m_arms.bones.Where(b => b == null).Concat(
                    m_activeProp.ArmsRoot.GetComponentsInChildren<Transform>()
                    .Where(t => m_boneNamesToIndex.ContainsKey(t.name))
                    .OrderBy(t => m_boneNamesToIndex[t.name])
                    ).ToArray();
                
                m_propToRenderers[m_activeProp].ForEach(r => r.enabled = true);
                m_drawAudio.clip = Utils.PickRandom(m_drawSounds);
                m_drawAudio.Play();
            }
        }
        else
        {
            if (IsPropActive)
            {
                m_propToRenderers[m_activeProp].ForEach(r => r.enabled = false);
                m_activeProp = null;
            }
            if (m_targetProp != null && m_props.All(p => p.Holster) && !m_interact.IsInteracting)
            {
                m_targetProp.Holster = false;
            }
        }

        if (m_activeProp != null && !m_activeProp.Holster && !m_interact.IsInteracting)
        {
            if (ControlsManager.Instance.JustDown(GameButton.Fire))
            {
                m_activeProp.FireStart();
            }
            if (ControlsManager.Instance.IsDown(GameButton.Fire))
            {
                m_activeProp.Fire();
            }
            if (ControlsManager.Instance.JustDown(GameButton.Reload))
            {
                m_activeProp.Reload();
            }
        }

        // only render the arms if they are raised
        m_arms.enabled = IsPropActive;

        m_props.ForEach(w => w.MainUpdate());
    }

    public void UpdateVisuals(PlayerInput input, PlayerLook look, CharacterMovement movement)
    {
        foreach (IProp prop in m_props)
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

    private void DrawProp(GameButton propButton, IProp prop)
    {
        if (ControlsManager.Instance.JustDown(propButton))
        {
            m_targetProp = (m_targetProp == prop) ? null : prop;
            HolsterActive();
        }
    }

    private void HolsterActive()
    {
        if (m_activeProp != null && !m_activeProp.Holster)
        {
            m_activeProp.Holster = true;
            m_activeProp.CancelActions();
            m_holsterAudio.clip = Utils.PickRandom(m_holsterSounds);
            m_holsterAudio.Play();
        }
    }
}
