using System.Collections;
using System.Text;
using UnityEngine;
using Framework;

public class MainGun : Prop
{
    [SerializeField] private TextMesh m_bulletsMagText;
    [SerializeField] private TextMesh m_bulletsHeldText;
    [SerializeField] private Transform m_bulletEmitter;
    [SerializeField] private GameObject m_muzzleFlashPosition;
    [SerializeField] private Light m_muzzleFlashLight;
    [SerializeField] private ParticleSystem m_muzzleFlash;
    [SerializeField] private BulletSettings m_bulletSettings;
    
	[SerializeField] [Range(0.1f, 100)]
    private float m_fireRate = 8.0f;
	[SerializeField] [Range(0, 100)]
    private float m_shotRecoilAmount = 20f;
	[SerializeField] [Range(1, 1.5f)]
    private float m_recoilStabilizeSpeed = 1.1f;

	[SerializeField]
    private int m_bulletsPerClip = 32;
    [SerializeField]
    private int m_maxAmmo = 260;

	[SerializeField] [Range(0, 6)]
    private float m_reloadTime = 2f;
    
	[SerializeField] [Range(0, 0.1f)]
    private float m_movementInaccuracy = 0.015f;
	[SerializeField] [Range(0, 1)]
    private float m_crouchInaccuracyMultiplier = 0.5f;

	private CharacterMovement m_character;
    private PlayerWeapons m_weapons;
    private MainGunAnimations m_anim;
    private MainGunSounds m_sound;
    private GunBlocking m_blocking;

    private StringBuilder m_bulletsMagSb = new StringBuilder(10, 10);
    private StringBuilder m_bulletsHeldSb = new StringBuilder(10, 10);
    private string m_bulletsMagStr;
    private string m_bulletsHeldStr;
    private int m_bulletsMag = 0;
    private int m_bulletsHeld = 0;

    private IEnumerator m_reload;
    private WaitForSeconds m_reloadStartWait;
    private WaitForSeconds m_reloadEndWait;
    private float m_recoilIncrease = 0;
	private float m_lastFireTime = 0;


    public override GameButton HolsterButton { get { return GameButton.Weapon1; } }
    protected override int MainAnimatorState { get { return 1; } }

    
    private bool IsReloading
    {
        get { return m_reload != null; }
    }

    private void Start()
    {
		m_character = GetComponentInParent<CharacterMovement>();
        m_weapons = GetComponentInParent<PlayerWeapons>();
        m_anim = GetComponent<MainGunAnimations>();
        m_sound = GetComponent<MainGunSounds>();
        m_blocking = GetComponentInChildren<GunBlocking>();

        BulletManager.Instance.InitBulletType(m_bulletSettings);

        m_bulletsMagStr = m_bulletsMagSb.GarbageFreeString();
        m_bulletsHeldStr = m_bulletsHeldSb.GarbageFreeString();

        m_reloadStartWait = new WaitForSeconds(m_reloadTime - 0.2f);
        m_reloadEndWait = new WaitForSeconds(0.2f);

        m_bulletsHeld = m_maxAmmo;
        m_bulletsMag = m_bulletsPerClip;
        m_muzzleFlashLight.enabled = false;
    }

    protected override void OnDraw()
    {
        ResetFireTime();
    }

    protected override void OnHolster() {}

    protected override void LogicUpdate()
    {
        m_recoilIncrease /= m_recoilStabilizeSpeed;
        m_weapons.Recoil = m_recoilIncrease;
        m_anim.RecoilUpdate();
    }

    protected override void AnimUpdate()
    {
        m_anim.AnimUpdate(IsReloading, 2.0f / m_reloadTime);

        m_bulletsMagSb.Clear();
        m_bulletsMagSb.Concat(m_bulletsMag);
        m_bulletsMagText.SetGarbateFreeText(m_bulletsMagStr);

        m_bulletsHeldSb.Clear();
        m_bulletsHeldSb.Concat(m_bulletsHeld);
        m_bulletsHeldText.SetGarbateFreeText(m_bulletsHeldStr);
    }

    public override void OnFireStart()
    {
        ResetFireTime();
    }

    public override void OnFireHeld()
    {
        if (m_bulletsMag == 0)
        {
            OnReload();
        }
        else if (!IsReloading && m_bulletsMag > 0 && !m_blocking.IsBlocked())
        {
            float firePeriod = (1 / m_fireRate);
            
            while (m_lastFireTime <= Time.time && m_bulletsMag > 0)
            {
                FireOneShot(Time.time - m_lastFireTime);
                m_recoilIncrease += m_shotRecoilAmount;
                m_lastFireTime += firePeriod;
                m_muzzleFlashLight.enabled = true;
                StartCoroutine(FinishFire());
            }
        }
    }

    public override void OnReload()
    {
        if (!IsReloading && m_bulletsHeld > 0 && m_bulletsMag < m_bulletsPerClip)
        {
            m_sound.PlayReloadStart();
            m_reload = FinishReload();
            StartCoroutine(m_reload);
        }
    }

    public override void CancelActions()
    {
        if (m_reload != null)
        {
            StopCoroutine(m_reload);
            m_reload = null;
        }
    }

    private void FireOneShot(float timeUntilNextUpdate)
    {
        float additionalInaccuracy = (m_movementInaccuracy * m_character.Velocity.magnitude);
        float inaccuracyMultiplier = 1;

        if (m_character.IsCrouching)
        {
            inaccuracyMultiplier *= m_crouchInaccuracyMultiplier;
        }

        BulletManager.Instance.FireBullet(
            m_bulletEmitter.position,
            m_bulletEmitter.rotation,
            m_bulletSettings,
            timeUntilNextUpdate,
            additionalInaccuracy,
            inaccuracyMultiplier);

        m_bulletsMag--;
        
        m_muzzleFlash.Emit(1);
        m_sound.PlayFireSound();
        m_anim.Recoil();
    }

    private IEnumerator FinishFire()
    {
        yield return null;
        m_muzzleFlashLight.enabled = false;
    }
    
    private IEnumerator FinishReload()
    {
        yield return m_reloadStartWait;

        int requiredBullets = Mathf.Min(m_bulletsHeld, m_bulletsPerClip - m_bulletsMag);
        m_bulletsMag += requiredBullets;
        m_bulletsHeld -= requiredBullets;
        m_sound.PlayReloadEnd();

        yield return m_reloadEndWait;

        m_reload = null;
        ResetFireTime();
    }

    private void ResetFireTime()
    {
        if (Time.time - m_lastFireTime > (1 / m_fireRate))
        {
            m_lastFireTime = Time.time;
        }
    }
}
