using UnityEngine;
using Framework.Interpolation;

public class MainGunAnimations : MonoBehaviour
{
	[SerializeField] [Range(0, 0.5f)]
    private float m_recoilTime = 0.025f;
	[SerializeField]
    private Vector3 m_recoilPosition;
	[SerializeField] [Range(1, 20)]
    private float m_recoilSpeed = 10f;
	[SerializeField][Range(1, 20)]
    private float m_recoilResetSpeed = 6f;
    [SerializeField][Range(0, 10)]
    private float m_movementAdjustRate = 3.0f;
    [SerializeField][Range(0, 16)]
    private float m_movementSmoothing = 3.5f;
    [SerializeField][Range(0, 16)]
    private float m_rotateSmoothing = 9.0f;

    [SerializeField]
    private GunBlocking m_gunBlocking;
    [SerializeField]
    private bool m_useBlocking = false;
    
    private Animator m_anim;

    private Vector3 m_fireResetPosition;
    private float m_recoilTimeLeft = 0;
    private float m_h = 0;
    private float m_v = 0;
    private float m_hLook = 0;
    private float m_vLook = 0;

    private void Start() 
    {
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

    public void AnimUpdate(MainGun gun, Vector2 move, Vector2 look, bool jumping, bool running, bool interact)
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
		m_anim.SetBool("Reloading", gun.IsReloading);
        m_anim.SetFloat("ReloadSpeed", 2.0f / gun.ReloadTime);
        m_anim.SetBool("Aiming Change", ControlsManager.Instance.JustUp(GameButton.Aim) || ControlsManager.Instance.JustDown(GameButton.Aim));
		m_anim.SetBool("Aiming", ControlsManager.Instance.IsDown(GameButton.Aim));
        m_anim.SetBool("Blocked", m_useBlocking && m_gunBlocking.IsBlocked());
        m_anim.SetBool("Lowered", gun.Holster);
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
