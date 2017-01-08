using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer m_arms;
    
    private PlayerInteract m_interact;
    private List<IWeapon> m_weapons;
    private IWeapon m_currentWeapon;

    private float m_recoil = 0;
    public float Recoil
    {
        get { return m_recoil; }
        set { m_recoil = value; }
    }

    private void Awake()
    {
        m_interact = GetComponentInParent<PlayerInteract>();
        m_weapons = GetComponentsInChildren<IWeapon>().ToList();

        m_currentWeapon = m_weapons.First();
    }

	private void Update()
    {
        // draw or holster specific items
        if (Controls.Instance.JustDown(GameButton.Weapon1))
        {
            if (m_currentWeapon != m_weapons[0])
            {
                m_currentWeapon = m_weapons[0];
            }
            else
            {
                m_currentWeapon.CancelReload();
                m_currentWeapon = null;
            }
        }

        // holster any weapons the player does not want drawn
        m_weapons.Where(w => w != m_currentWeapon).ToList().ForEach(w => w.SetHolster(true));
        // hide any weapons that are completely holstered
        m_weapons.ForEach(w => w.GetGameObject().GetComponentsInChildren<Renderer>().ToList().ForEach(r => r.enabled = !w.IsHolstered()));
        // only draw the arms if they are raised
        m_arms.enabled = AnyWeaponDrawn();
        
        if (m_currentWeapon != null)
        {
            if (Controls.Instance.IsDown(GameButton.Fire) && !m_interact.IsInteracting)
            {
                m_currentWeapon.Fire();
            }

            if (Controls.Instance.JustDown(GameButton.Reload) && !m_interact.IsInteracting && !m_interact.Interacted)
            {
                m_currentWeapon.Reload();
            }

            if (m_currentWeapon.IsReloading() && (m_interact.IsInteracting || m_interact.Interacted))
            {
                m_currentWeapon.CancelReload();
            }
            
            if (!AnyWeaponDrawn() && !m_interact.IsInteracting)
            {
                m_currentWeapon.SetHolster(false);
            }
            else if (m_interact.IsInteracting)
            {
                m_currentWeapon.SetHolster(true);
            }
        }

        // execute other specific updates
        m_weapons.ForEach(w => w.StateUpdate());
    }

    private bool AnyWeaponDrawn()
    {
        return m_weapons.Any(w => !w.IsHolstered());
    }
}
