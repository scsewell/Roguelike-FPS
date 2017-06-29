using UnityEngine;
using Framework.Interpolation;

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
    
    private MainGun m_gun;
    private Animator m_anim;

    private Vector3 m_fireResetPosition;
    private float m_recoilTimeLeft = 0;
    private float m_h = 0;
    private float m_v = 0;
    private float m_hLook = 0;
    private float m_vLook = 0;

    private void Start() 
    {
        m_gun = GetComponent<MainGun>();
		m_anim = GetComponent<Animator>();

        gameObject.AddComponent<TransformInterpolator>();

		m_fireResetPosition = transform.localPosition;
	}

    public void RecoilUpdate()
    {
        bool recoiling = (m_recoilTimeLeft > 0);
        Vector3 targetPos = recoiling ? m_recoilPosition : m_fireResetPosition;
        float targetSpeed = recoiling ? m_recoilSpeed : m_recoilResetSpeed;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, targetSpeed * Time.deltaTime);
        m_recoilTimeLeft -= Time.deltaTime;
    }

    public void AnimUpdate(Vector2 move, Vector2 look, bool jumping, bool running, bool interact)
    {
        m_h = Mathf.MoveTowards(m_h, move.x, Time.deltaTime * m_movementAdjustRate);
        m_v = Mathf.MoveTowards(m_v, move.y, Time.deltaTime * m_movementAdjustRate);
        m_h = Mathf.Lerp(m_h, move.x, Time.deltaTime * m_movementSmoothing);
        m_v = Mathf.Lerp(m_v, move.y, Time.deltaTime * m_movementSmoothing);
        
        m_hLook = Mathf.Lerp(m_hLook, look.x, Time.deltaTime * m_rotateSmoothing);
        m_vLook = Mathf.Lerp(m_vLook, look.y, Time.deltaTime * m_rotateSmoothing);
        
		m_anim.SetFloat("Speed", m_v);
        m_anim.SetFloat("Direction", m_h);
        m_anim.SetFloat("LookVertical", m_hLook);
        m_anim.SetFloat("LookHorizontal", m_vLook);
		m_anim.SetBool("Running", running);
		m_anim.SetBool("Jump", jumping);
        m_anim.SetBool("Interact", interact);
		m_anim.SetBool("Reloading", m_gun.IsReloading());
        m_anim.SetFloat("ReloadSpeed", 2.0f / m_gun.GetReloadSpeed());
        m_anim.SetBool("Aiming Change", Controls.Instance.JustUp(GameButton.Aim) || Controls.Instance.JustDown(GameButton.Aim));
		m_anim.SetBool("Aiming", Controls.Instance.IsDown(GameButton.Aim));
        m_anim.SetBool("Blocked", m_useBlocking && m_gunBlocking.IsBlocked());
        m_anim.SetBool("Lowered", m_gun.Holster);
    }

	public void Recoil()
    {
        m_recoilTimeLeft = m_recoilTime;
	}

    public bool IsHostered()
    {
        return m_anim.GetCurrentAnimatorStateInfo(1).IsTag("Holstered");
    }
}
