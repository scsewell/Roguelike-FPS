using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer m_arms;
    [SerializeField] private Transform m_weapon1;
    [SerializeField] private Transform m_flashlight;

    private PlayerInteract m_interact;
    private List<IProp> m_props;
    private IProp m_currentProp;

    private float m_recoil = 0;
    public float Recoil
    {
        get { return m_recoil; }
        set { m_recoil = value; }
    }

    private void Awake()
    {
        m_interact = GetComponentInParent<PlayerInteract>();

        m_props = new List<IProp>();
        m_props.Add(m_weapon1.GetComponent<IProp>());
        m_props.Add(m_flashlight.GetComponent<IProp>());

        m_currentProp = m_props.First();

        if (m_arms.bones != null)
        {
            string str = "";
            foreach (Transform t in m_arms.bones)
            {
                str += ((t != null) ? t.name : "null") + "\n";
            }
            Debug.Log(str);
        }
        m_arms.bones = m_arms.bones.Where(b => b == null).Concat(m_arms.bones.Where(b => b != null).OrderBy(b => b.name)).ToArray();
        if (m_arms.bones != null)
        {
            string str = "";
            foreach (Transform t in m_arms.bones)
            {
                str += ((t != null) ? t.name : "null") + "\n";
            }
            Debug.Log(str);
        }
    }

    private void Update()
    {
        if (m_arms.bones != null)
        {
            string str = "";
            foreach (Transform t in m_arms.bones)
            {
                if (t)
                {
                    str += t.name + "\n";
                }
            }
            Debug.Log(str);
        }

        // draw or holster specific items
        DrawCheck(GameButton.Weapon1, m_weapon1.GetComponent<IProp>());
        DrawCheck(GameButton.Flashlight, m_flashlight.GetComponent<IProp>());

        // holster any props the player does not want drawn
        m_props.Where(w => w != m_currentProp).ToList().ForEach(w => w.SetHolster(true));
        // hide any props that are completely holstered
        m_props.ForEach(w => w.GetGameObject().GetComponentsInChildren<Renderer>().ToList().ForEach(r => r.enabled = !w.IsHolstered()));
        // only render the arms if they are raised
        m_arms.enabled = AnyWeaponDrawn();
        //m_arms.bones = m_currentProp.GetArmsRoot().GetComponentsInChildren<Transform>();

        if (m_currentProp != null)
        {
            if (Controls.Instance.IsDown(GameButton.Fire) && !m_interact.IsInteracting)
            {
                m_currentProp.Fire();
            }

            if (Controls.Instance.JustDown(GameButton.Reload) && !m_interact.IsInteracting && !m_interact.Interacted)
            {
                m_currentProp.Reload();
            }

            if (m_currentProp.IsReloading() && (m_interact.IsInteracting || m_interact.Interacted))
            {
                m_currentProp.CancelReload();
            }
            
            if (!AnyWeaponDrawn() && !m_interact.IsInteracting)
            {
                m_currentProp.SetHolster(false);
            }
            else if (m_interact.IsInteracting)
            {
                m_currentProp.SetHolster(true);
            }
        }

        // execute other specific updates
        m_props.ForEach(w => w.StateUpdate());
    }

    private void DrawCheck(GameButton propButton, IProp prop)
    {
        if (Controls.Instance.JustDown(propButton))
        {
            if (m_currentProp != prop)
            {
                m_currentProp = prop;
            }
            else
            {
                m_currentProp.CancelReload();
                m_currentProp = null;
            }
        }
    }

    private bool AnyWeaponDrawn()
    {
        return m_props.Any(w => !w.IsHolstered());
    }
}
