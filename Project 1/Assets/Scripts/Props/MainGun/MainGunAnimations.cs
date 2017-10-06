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

    [SerializeField]
    private GunBlocking m_gunBlocking;
    [SerializeField]
    private bool m_useBlocking = false;
    
    private Animator m_anim;
    private Vector3 m_fireResetPosition;
    private float m_recoilTimeLeft = 0;

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

    public void AnimUpdate(bool isReloading, float reloadSpeed)
    {
        m_anim.SetBool("Blocked", m_useBlocking && m_gunBlocking.IsBlocked());
		m_anim.SetBool("Reloading", isReloading);
        m_anim.SetFloat("ReloadSpeed", reloadSpeed);
    }

	public void Recoil()
    {
        m_recoilTimeLeft = m_recoilTime;
	}
}
