using UnityEngine;

public class MainGunAnimations : MonoBehaviour
{
	[SerializeField] private float m_recoilTime = 0.065f;
	[SerializeField] private Vector3 m_recoilPosition;
	[SerializeField] private float m_recoilSpeed = 10f;
	[SerializeField] private float m_recoilResetSpeed = 6f;
    [SerializeField] private float m_movementAdjustRate = 3.0f;
    [SerializeField] private float m_movementSmoothing = 3.5f;
    [SerializeField] private float m_rotateSmoothing = 9.0f;
    [SerializeField] private bool m_useBlocking = false;
    [SerializeField] private GunBlocking m_gunBlocking;

    private CharacterInput m_moveDirection;
    private CharacterMovement m_character;
    private CharacterLook m_look;
    private MainGun m_gun;
    private Animator m_anim;

    private Vector3 m_fireResetPosition;
    private float m_recoilTimeLeft = 0;
    private float m_h;
    private float m_v;
    private float m_hLook;
    private float m_vLook;

    private void Start() 
    {
		m_anim = GetComponent<Animator>();
        m_look = transform.root.GetComponent<CharacterLook>();
        m_moveDirection = transform.root.GetComponent<CharacterInput>();
        m_character = transform.root.GetComponent<CharacterMovement>();
        m_gun = GetComponent<MainGun>();

		m_fireResetPosition = transform.localPosition;
	}

    private void Update()
    {
        bool recoiling = (m_recoilTimeLeft > 0);
        
		transform.localPosition = Vector3.Lerp(transform.localPosition, recoiling ? m_recoilPosition : m_fireResetPosition, (recoiling ? m_recoilSpeed : m_recoilResetSpeed) * Time.deltaTime);
        
        m_h = Mathf.MoveTowards(m_h, m_moveDirection.GetMoveDirection().x, Time.deltaTime * m_movementAdjustRate);
        m_v = Mathf.MoveTowards(m_v, m_moveDirection.GetMoveDirection().z, Time.deltaTime * m_movementAdjustRate);
        m_h = Mathf.Lerp(m_h, m_moveDirection.GetMoveDirection().x, Time.deltaTime * m_movementSmoothing);
        m_v = Mathf.Lerp(m_v, m_moveDirection.GetMoveDirection().z, Time.deltaTime * m_movementSmoothing);
        
        m_hLook = Mathf.Lerp(m_hLook, m_look.GetDeltaX(), Time.deltaTime * m_rotateSmoothing);
        m_vLook = Mathf.Lerp(m_vLook, m_look.GetDeltaY(), Time.deltaTime * m_rotateSmoothing);
        
		m_anim.SetFloat("Speed", m_v);
        m_anim.SetFloat("Direction", m_h);
        m_anim.SetFloat("LookVertical", m_hLook);
        m_anim.SetFloat("LookHorizontal", m_vLook);
        m_anim.SetFloat("ReloadSpeed", 2.0f / m_gun.GetReloadSpeed());
        m_anim.SetBool("Aiming Change", Controls.JustUp(GameButton.Aim) || Controls.JustDown(GameButton.Aim));
		m_anim.SetBool("Aiming", Controls.IsDown(GameButton.Aim));
		m_anim.SetBool("Running", m_character.IsRunning());
		m_anim.SetBool("Jump", m_character.IsJumping());
		m_anim.SetBool("Reloading", m_gun.IsReloading());
        m_anim.SetBool("Blocked", m_useBlocking && m_gunBlocking.IsBlocked());

        m_recoilTimeLeft -= Time.deltaTime;
    }

	public void Recoil()
    {
        m_recoilTimeLeft = m_recoilTime;
	}
}
