using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Framework;

public class MainGun : Prop
{
    [SerializeField] private TextMesh m_bulletsMagText;
    [SerializeField] private TextMesh m_bulletsHeldText;
    [SerializeField] private Transform m_bulletEmitter;
    [SerializeField] private Light m_muzzleFlashLight;
    [SerializeField] private ParticleSystem m_muzzleFlash;
    [SerializeField] private BulletSettings m_bulletSettings;
    
	[SerializeField]
    private int m_bulletsPerClip = 32;
    [SerializeField]
    private int m_maxAmmo = 260;
	[SerializeField] [Range(0, 6)]
    private float m_reloadTime = 2f;
    
	[SerializeField] [Range(0.1f, 100)]
    private float m_fireRate = 8.0f;

    [SerializeField] [Range(1, 50)]
    float m_bulletsPerRecoilKey = 5.0f;
    [SerializeField] [Range(1, 40)]
    float m_horizontalRecoil = 5.0f;
    [SerializeField] [Range(1, 40)]
    float m_verticalRecoil = 5.0f;
    [SerializeField] [Range(0.01f, 1)]
    float m_recoilSmoothing = 0.2f;

    [SerializeField] [Range(0, 0.1f)]
    private float m_movementInaccuracy = 0.015f;
	[SerializeField] [Range(0, 1)]
    private float m_crouchInaccuracyMultiplier = 0.5f;

	private CharacterMovement m_character;
    private MainGunAnimations m_anim;
    private MainGunSounds m_sound;
    private GunBlocking m_blocking;

    private int m_bulletsMag = 0;
    private int m_bulletsHeld = 0;
    private bool m_bulletsMagDirty = true;
    private bool m_bulletsHeldDirty = true;
    private StringBuilder m_bulletsMagSb = new StringBuilder();
    private StringBuilder m_bulletsHeldSb = new StringBuilder();

    private int BulletsMag
    {
        get { return m_bulletsMag; }
        set
        {
            if (m_bulletsMag != value)
            {
                m_bulletsMag = value;
                m_bulletsMagDirty = true;
            }
        }
    }

    private int BulletsHeld
    {
        get { return m_bulletsHeld; }
        set
        {
            if (m_bulletsHeld != value)
            {
                m_bulletsHeld = value;
                m_bulletsHeldDirty = true;
            }
        }
    }

    private IEnumerator m_reload;
    private LinkedList<Recoil> m_recoil = new LinkedList<Recoil>();
    private float m_lastFireTime = 0;

    public override GameButton HolsterButton { get { return GameButton.Weapon1; } }
    protected override int MainAnimatorState { get { return 1; } }
    
    private bool IsReloading
    {
        get { return m_reload != null; }
    }


    private void Awake()
    {
		m_character = Utils.GetComponentInAnyParent<CharacterMovement>(gameObject);
        m_anim = GetComponent<MainGunAnimations>();
        m_sound = GetComponent<MainGunSounds>();
        m_blocking = GetComponentInChildren<GunBlocking>();

        BulletManager.Instance.InitBulletType(m_bulletSettings);

        BulletsMag = m_bulletsPerClip;
        BulletsHeld = m_maxAmmo;
        m_muzzleFlashLight.enabled = false;

        for (int i = 0; i < 5; i++)
        {
            m_recoil.AddLast(new Recoil());
        }
    }

    protected override void OnDraw()
    {
        ResetFire();
    }

    protected override void OnHolster() {}

    protected override void LogicUpdate(bool firing)
    {
        if (firing && BulletsMag == 0)
        {
            OnReload();
        }
        else if (firing && !IsReloading && BulletsMag > 0 && !m_blocking.IsBlocked())
        {
            float firePeriod = (1 / m_fireRate);

            bool hasFired = false;

            while (m_lastFireTime <= Time.time && BulletsMag > 0)
            {
                FireOneShot(Time.time - m_lastFireTime);
                m_lastFireTime += firePeriod;

                if (!hasFired)
                {
                    m_sound.PlayFireSound();
                    m_muzzleFlashLight.enabled = true;
                    StartCoroutine(FinishFire());
                    hasFired = true;
                }
            }
        }
        else
        {
            ResetFire();
        }
        
        m_anim.RecoilUpdate();
    }

    public override Vector2 GetRecoil()
    {
        Vector2 recoilDelta = Vector2.zero;

        foreach (Recoil recoil in m_recoil)
        {
            recoilDelta += recoil.Progress(m_recoilSmoothing, m_horizontalRecoil, m_verticalRecoil, m_fireRate);
        }
        return recoilDelta;
    }

    protected override void AnimUpdate()
    {
        m_anim.AnimUpdate(IsReloading, 2.0f / m_reloadTime);

        if (m_bulletsMagDirty)
        {
            m_bulletsMagSb.Clear();
            m_bulletsMagSb.Append(m_bulletsMag);
            m_bulletsMagText.text = m_bulletsMagSb.ToString();
            m_bulletsMagDirty = false;
        }

        if (m_bulletsHeldDirty)
        {
            m_bulletsHeldSb.Clear();
            m_bulletsHeldSb.Append(m_bulletsHeld);
            m_bulletsHeldText.text = m_bulletsHeldSb.ToString();
            m_bulletsHeldDirty = false;
        }
    }

    protected override void OnFireStart()
    {
        Recoil recoil = m_recoil.Last.Value;
        m_recoil.RemoveLast();
        m_recoil.AddFirst(recoil);

        recoil.GeneratePattern(m_bulletsPerClip, m_bulletsPerRecoilKey, m_fireRate);
        
        ResetFire();
    }

    protected override void OnFireEnd()
    {
        ResetFire();
    }

    public override void OnReload()
    {
        if (!IsReloading && BulletsHeld > 0 && BulletsMag < m_bulletsPerClip)
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

        BulletsMag--;
        m_recoil.First.Value.BulletFired();

        if (m_muzzleFlash != null)
        {
            m_muzzleFlash.Emit(1);
        }
        m_anim.Recoil();
    }

    private IEnumerator FinishFire()
    {
        yield return null;
        m_muzzleFlashLight.enabled = false;
    }
    
    private IEnumerator FinishReload()
    {
        yield return Utils.Wait(m_reloadTime - 0.2f);

        int addedBullets = Mathf.Min(m_bulletsHeld, m_bulletsPerClip - m_bulletsMag);
        BulletsMag += addedBullets;
        BulletsHeld -= addedBullets;
        m_bulletsHeldDirty = true;
        m_sound.PlayReloadEnd();

        yield return Utils.Wait(0.2f);

        m_reload = null;
        ResetFire();
    }

    private void ResetFire()
    {
        if (Time.time - m_lastFireTime > (1 / m_fireRate))
        {
            m_lastFireTime = Time.time;
        }
    }

    private class Recoil
    {
        private AnimationCurve m_pattern = new AnimationCurve();
        private int m_burstCount = 0;
        private float m_recoilTime = 0;
        private Vector2 m_lastRecoil = Vector2.zero;

        public void GeneratePattern(float bulletsPerMag, float bulletsPerKey, float fireRate)
        {
            while (m_pattern.length > 0)
            {
                m_pattern.RemoveKey(0);
            }

            float hRecoilValue = 0;
            int keyCount = Mathf.CeilToInt(bulletsPerMag / bulletsPerKey);
            for (int i = 0; i < keyCount; i++)
            {
                m_pattern.AddKey(new Keyframe(i * (bulletsPerKey / fireRate), hRecoilValue));
                hRecoilValue += (Random.value - 0.5f);
            }

            m_burstCount = 0;
            m_recoilTime = 0;
            m_lastRecoil = Vector2.zero;
        }

        public Vector2 Progress(float recoilSmoothing, float hRecoil, float vRecoil, float fireRate)
        {
            m_recoilTime = Mathf.Lerp(m_recoilTime, (m_burstCount / fireRate), Time.deltaTime / recoilSmoothing);
            Vector2 recoil = new Vector2(hRecoil * m_pattern.Evaluate(m_recoilTime), vRecoil * m_recoilTime);
            Vector2 recoilDelta = recoil - m_lastRecoil;
            m_lastRecoil = recoil;
            return recoilDelta;
        }

        public void BulletFired()
        {
            m_burstCount++;
        }
    }
}
