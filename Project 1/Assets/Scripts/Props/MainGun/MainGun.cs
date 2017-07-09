using UnityEngine;
using System.Collections;

public class MainGun : MonoBehaviour, IProp
{
    [SerializeField]
    private Transform m_armsRoot;
    public Transform ArmsRoot { get { return m_armsRoot; } }

    [SerializeField] private TextMesh m_bulletGUI;
    [SerializeField] private TextMesh m_clipsGUI;
    [SerializeField] private Transform m_bulletEmitter;
    [SerializeField] private GameObject m_muzzleFlashPosition;
    [SerializeField] private Light m_muzzleFlashLight;
    [SerializeField] private ParticleSystem m_muzzleFlash;

    [SerializeField]
    private BulletSettings m_bulletSettings;
    
	[SerializeField] [Range(0.1f, 100)]
    private float m_fireRate = 8.0f;
	[SerializeField] [Range(0, 100)]
    private float m_shotRecoilAmount = 20f;
	[SerializeField] [Range(1, 1.5f)]
    private float m_recoilStabilizeSpeed = 1.1f;

	[SerializeField]
    private int m_bulletsPerClip = 32;
	[SerializeField]
    private int m_clips = 20;

	[SerializeField] [Range(0, 6)]
    private float m_reloadTime = 2f;
    public float ReloadTime { get { return m_reloadTime; } }
    
	[SerializeField] [Range(0, 0.1f)]
    private float m_movementInaccuracy = 0.015f;
	[SerializeField] [Range(0, 1)]
    private float m_crouchInaccuracyMultiplier = 0.5f;

	private CharacterMovement m_character;
    private PlayerWeapons m_weapons;
    private MainGunAnimations m_anim;
    private MainGunSounds m_sound;
    private GunBlocking m_blocking;

    private IEnumerator m_reload;
	private int m_bulletsLeft = 0;
	private float m_recoilIncrease = 0;
	private float m_lastFireTime = 0;
    
    public bool Holster { get; set; }

    public bool IsHolstered
    {
        get { return m_anim.IsHostered(); }
    }
    
    public bool IsReloading
    {
        get { return m_reload != null; }
    }

    public GameObject GameObject
    {
        get { return gameObject; }
    }

    private void Start()
    {
		m_character = transform.root.GetComponent<CharacterMovement>();
        m_weapons = transform.GetComponentInParent<PlayerWeapons>();
        m_anim = GetComponent<MainGunAnimations>();
        m_sound = GetComponent<MainGunSounds>();
        m_blocking = GetComponentInChildren<GunBlocking>();

        m_bulletsLeft = m_bulletsPerClip;
        m_muzzleFlashLight.enabled = false;
    }

    public void MainUpdate()
    {
        m_recoilIncrease /= m_recoilStabilizeSpeed;
        m_weapons.Recoil = m_recoilIncrease;
        m_anim.RecoilUpdate();
    }

    public void VisualUpdate(Vector2 move, Vector2 look, bool jumping, bool running, bool interact)
    {
        m_anim.AnimUpdate(this, move, look, jumping, running, interact);
        m_bulletGUI.text = m_bulletsLeft.ToString();
        m_clipsGUI.text = m_clips.ToString();
    }

    public void FireStart()
    {
        m_lastFireTime = Time.time;
    }

    public void Fire()
    {
        if (m_bulletsLeft == 0)
        {
            Reload();
        }
        else if (!IsReloading && m_bulletsLeft > 0 && !m_blocking.IsBlocked())
        {
            float firePeriod = (1 / m_fireRate);
            
            while (m_lastFireTime < Time.time && m_bulletsLeft > 0)
            {
                FireOneShot(0);
                m_recoilIncrease += m_shotRecoilAmount;
                m_lastFireTime += firePeriod;
                m_muzzleFlashLight.enabled = true;
                StartCoroutine(FinishFire());
            }
        }
    }

    public void Reload()
    {
        if (!IsReloading && m_clips > 0 && m_bulletsLeft < m_bulletsPerClip)
        {
            m_sound.PlayReloadStart();
            m_reload = FinishReload();
            StartCoroutine(m_reload);
        }
    }

    public void CancelActions()
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

        m_bulletsLeft--;

        ParticleSystem flash = Instantiate(m_muzzleFlash, m_muzzleFlashPosition.transform.position, transform.rotation);
        flash.transform.parent = m_muzzleFlashPosition.transform;

        m_sound.PlayFireSound();
        m_anim.Recoil();
    }

    private IEnumerator FinishFire()
    {
        yield return 0;
        m_muzzleFlashLight.enabled = false;
    }
    
    private IEnumerator FinishReload()
    {
        yield return new WaitForSeconds(m_reloadTime - 0.2f);
        m_bulletsLeft = m_bulletsPerClip;
        m_clips--;
        m_sound.PlayReloadEnd();
        yield return new WaitForSeconds(0.2f);
        m_reload = null;
	}
}
