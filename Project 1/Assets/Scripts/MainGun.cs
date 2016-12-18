using UnityEngine;
using System.Collections;

public class MainGun : MonoBehaviour
{
	[SerializeField] private float m_range = 100.0f;
	[SerializeField] private float m_fireRate = 0.05f;
	[SerializeField] private float m_force = 10.0f;
	[SerializeField] private float m_damage = 5.0f;
	[SerializeField] private int m_bulletsPerClip = 32;
	[SerializeField] private int m_clips = 20;
	[SerializeField] private float m_reloadTime = 0.5f;
	[SerializeField] private float m_baseInaccuracy = 0.0f;
	[SerializeField] private float m_movementInaccuracyMultiplier = 2.0f;
	[SerializeField] private float m_crouchInaccuracyMultiplier = 0.5f;
	[SerializeField] private float m_shotRecoilAmount = 2f;
	[SerializeField] private float m_recoilStabilizeSpeed = 2f;

    [SerializeField] private TextMesh m_bulletGUI;
    [SerializeField] private TextMesh m_clipsGUI;
    [SerializeField] private Transform m_bulletEmitter;
	[SerializeField] private GameObject m_bulletHoles;
	[SerializeField] private GameObject m_muzzleFlashPosition;
	[SerializeField] private ParticleSystem m_muzzleFlash;
	[SerializeField] private Light m_muzzleFlashLight;
	[SerializeField] private ParticleSystem m_sparksParticles;
    [SerializeField] private string m_bleedObjects;
    [SerializeField] private ParticleSystem m_bloodParticles;
    [SerializeField] private LayerMask m_hitLayers;

	private CharacterMovement m_character;
    private MainGunAnimations m_anim;
    private MainGunSounds m_sound;

    private bool m_reloading = false;
	private float m_recoilIncrease = 0;
	private int m_bulletsLeft = 0;
	private float m_nextFireTime = 0;

	private void Start()
    {
		m_character = transform.root.GetComponent<CharacterMovement>();
        m_anim = GetComponent<MainGunAnimations>();
        m_sound = GetComponent<MainGunSounds>();

		m_bulletsLeft = m_bulletsPerClip;
	}

    private void Update()
    {
		if (m_character.IsJumping() && m_reloading)
        {
            m_reloading = false;
            StopCoroutine(FinishReload());
            m_sound.ReloadCanceled();
		}

		m_muzzleFlashLight.enabled = false;
	}

    private void FixedUpdate()
    {
        m_recoilIncrease /= m_recoilStabilizeSpeed;
    }

    private void LateUpdate()
    {
        m_bulletGUI.text = m_bulletsLeft.ToString();
        m_clipsGUI.text = m_clips.ToString();
    }

    public void Fire()
    {
		if (!m_reloading)
        {
			if (Time.time - m_fireRate > m_nextFireTime)
            {
				m_nextFireTime = Time.time - Time.deltaTime;
			}
		
			while (m_nextFireTime < Time.time && m_bulletsLeft > 0)
            {
                m_recoilIncrease += m_shotRecoilAmount;
                m_muzzleFlashLight.enabled = true;
				FireOneShot();
				m_nextFireTime += m_fireRate;
			}
        }

        if (m_bulletsLeft == 0 && !m_character.IsJumping())
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (!m_reloading && m_bulletsLeft < m_bulletsPerClip && m_clips > 0)
        {
            m_reloading = true;
            m_sound.Reload();
            StartCoroutine(FinishReload());
        }
    }

    private void FireOneShot()
    {
		float inaccuracy = (m_baseInaccuracy * m_movementInaccuracyMultiplier * m_character.movement.velocity.magnitude) + m_baseInaccuracy;	

		if (m_character.IsCrouching())
        {
			inaccuracy *= m_crouchInaccuracyMultiplier;
		}

		Vector3 direction = m_bulletEmitter.TransformDirection(new Vector3(Random.value * inaccuracy - (inaccuracy / 2), Random.value * inaccuracy - (inaccuracy / 2), 1));
		RaycastHit hit;
		
		if (Physics.Raycast(m_bulletEmitter.position, direction, out hit, m_range, m_hitLayers))
        {
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
                    GameObject hole = Instantiate(m_bulletHoles, hit.point + (hit.normal / 1000f), rotation) as GameObject;
                    hole.GetComponent<BulletHoles>().SetParent(hit.collider.transform);
				}
			}

			hit.collider.SendMessage("RecieveDamage", m_damage, SendMessageOptions.DontRequireReceiver);
        }
        m_bulletsLeft--;

        ParticleSystem flash = Instantiate(m_muzzleFlash, m_muzzleFlashPosition.transform.position, transform.rotation);
        flash.transform.parent = m_muzzleFlashPosition.transform;

        m_sound.Recoil();
        m_anim.Recoil();
	}
	
	private IEnumerator FinishReload()
    {
		yield return new WaitForSeconds(m_reloadTime);
		m_reloading = false;
		m_clips--;
		m_bulletsLeft = m_bulletsPerClip;
	}
	
	public float GetRecoil()
    {
		return m_recoilIncrease;
	}
	
	public bool IsReloading()
    {
		return m_reloading;
	}
}
