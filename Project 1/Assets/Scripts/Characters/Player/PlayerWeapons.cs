using UnityEngine;
using System.Linq;

public class PlayerWeapons : MonoBehaviour
{
	private CharacterMovement m_character;
    private PlayerInteract m_interact;
    private IWeapon[] m_weapons;

    private float m_recoil = 0;
    public float Recoil
    {
        get { return m_recoil; }
        set { m_recoil = value; }
    }

    private IWeapon m_currentWeapon;

    private void Awake()
    {
        m_character = GetComponentInParent<CharacterMovement>();
        m_interact = GetComponentInParent<PlayerInteract>();
        m_weapons = GetComponentsInChildren<IWeapon>();

        m_currentWeapon = m_weapons.First();
    }

	private void Update()
    {
		if (Controls.Instance.IsDown(GameButton.Fire) && !m_interact.Interacting)
        {
            m_currentWeapon.Fire();
		}
		if (Controls.Instance.IsDown(GameButton.Reload) && !m_character.IsJumping() && !m_interact.Interacting && !m_interact.Interact)
        {
            m_currentWeapon.Reload();
		}
        if (m_currentWeapon.IsReloading() && (m_interact.Interacting || m_interact.Interact))
        {
            m_currentWeapon.CancelReload();
        }
	}
}
