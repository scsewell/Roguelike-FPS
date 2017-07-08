using UnityEngine;
using System.Collections;

public class MainGun : MonoBehaviour, IProp
{
    [SerializeField]
    private Transform m_armsRoot;
    public Transform ArmsRoot { get { return m_armsRoot; } }

	[SerializeField] private float m_range = 100.0f;
	[SerializeField] private float m_fireRate = 0.05f;
	[SerializeField] private float m_force = 10.0f;
	[SerializeField] private float m_damage = 5.0f;
	[SerializeField] private int m_bulletsPerClip = 32;
	[SerializeField] private int m_clips = 20;

	[SerializeField]
    private float m_reloadTime = 2f;
    public float ReloadTime { get { return m_reloadTime; } }

	[SerializeField] private float m_baseInaccuracy = 0.0135f;
	[SerializeField] private float m_movementInaccuracyMultiplier = 1.5f;
	[SerializeField] private float m_crouchInaccuracyMultiplier = 0.5f;
	[SerializeField] private float m_shotRecoilAmount = 20f;
	[SerializeField] private float m_recoilStabilizeSpeed =1.25f;
    [SerializeField] private LayerMask m_hitLayers;
    [SerializeField] private string m_bleedObjects;
    
    [SerializeField] private TextMesh m_bulletGUI;
    [SerializeField] private TextMesh m_clipsGUI;
    [SerializeField] private Transform m_bulletEmitter;
	[SerializeField] private GameObject m_muzzleFlashPosition;
	[SerializeField] private Light m_muzzleFlashLight;
	[SerializeField] private Decal m_bulletHoles;
	[SerializeField] private ParticleSystem m_muzzleFlash;
	[SerializeField] private ParticleSystem m_sparksParticles;
    [SerializeField] private ParticleSystem m_bloodParticles;

	private CharacterMovement m_character;
    private PlayerWeapons m_weapons;
    private MainGunAnimations m_anim;
    private MainGunSounds m_sound;
    private GunBlocking m_blocking;

    private IEnumerator m_reload;
	private int m_bulletsLeft = 0;
	private float m_recoilIncrease = 0;
	private float m_nextFireTime = 0;
    
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

    public void FireStart() {}

    public void Fire()
    {
        if (m_bulletsLeft == 0)
        {
            Reload();
        }

        if (!IsReloading && m_bulletsLeft > 0 && !m_blocking.IsBlocked())
        {
			if (Time.time > m_nextFireTime + m_fireRate)
            {
				m_nextFireTime = Time.time - Time.deltaTime;
			}
		
			while (Time.time > m_nextFireTime && m_bulletsLeft > 0)
            {
				FireOneShot();
                m_recoilIncrease += m_shotRecoilAmount;
				m_nextFireTime += m_fireRate;
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

    private void FireOneShot()
    {
		float inaccuracy = (m_baseInaccuracy * m_movementInaccuracyMultiplier * m_character.Velocity.magnitude) + m_baseInaccuracy;	

		if (m_character.IsCrouching)
        {
			inaccuracy *= m_crouchInaccuracyMultiplier;
		}

		Vector3 direction = m_bulletEmitter.rotation * new Vector3(Random.value * inaccuracy - (inaccuracy / 2), Random.value * inaccuracy - (inaccuracy / 2), 1);
		RaycastHit hit;
		if (Physics.Raycast(m_bulletEmitter.position, direction, out hit, m_range, m_hitLayers))
        {
            HitboxCollider hitbox = hit.transform.GetComponentInParent<HitboxCollider>();
            if (hitbox != null)
            {
                hitbox.Damage(m_damage);
            }

            if (hit.rigidbody)
            {
                hit.rigidbody.AddForceAtPosition(m_force * direction, hit.point);
            }

			if (hit.transform.root.tag == m_bleedObjects)
            {	
				if (m_bloodParticles)
                {
					ParticleSystem blood = Instantiate(m_bloodParticles, hit.point, Quaternion.LookRotation(hit.normal)) as ParticleSystem;
					blood.transform.parent = hit.collider.transform;
				}
			}
            else
            {
				if (m_sparksParticles)
                {
					Instantiate(m_sparksParticles, hit.point, Quaternion.LookRotation(hit.normal));
				}
				if (m_bulletHoles)
                {
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.back, hit.normal) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
                    Decal hole = Instantiate(m_bulletHoles, hit.point, rotation).GetComponent<Decal>();
                    hole.GetComponent<BulletHoles>().SetParent(hit.collider.transform);
                    hole.BuildDecal(hit.transform.gameObject);
				}
			}
        }
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
