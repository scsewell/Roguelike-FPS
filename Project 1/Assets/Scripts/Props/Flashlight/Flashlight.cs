using UnityEngine;

public class Flashlight : MonoBehaviour, IProp
{
    [SerializeField] private Transform m_armsRoot;
    [SerializeField] private Transform m_flashlightRig;
    [SerializeField] private Transform m_flashlight;
    [SerializeField] private float m_movementAdjustRate = 3.0f;
    [SerializeField] private float m_movementSmoothing = 3.5f;
    [SerializeField] private float m_lookSmoothing = 9.0f;
    [SerializeField] private float m_lookSensitivity = 0.2f;
    [SerializeField] private AudioClip m_turnOn;
    [SerializeField] private AudioClip m_turnOff;
    
    private Light m_light;
    private Animator m_anim;
    private AudioSource m_audio;
    private bool m_on = true;
    private float m_y = 0;
    private float m_x = 0;
    private float m_lookX = 0;
    private float m_lookY = 0;

    private bool m_holster = true;
    public bool Holster
    {
        get { return m_holster; }
        set { m_holster = value; }
    }

    private void Awake()
    {
        m_light = GetComponentInChildren<Light>();
        m_anim = GetComponentInChildren<Animator>();
        m_audio = GetComponent<AudioSource>();
    }

    public void MainUpdate()
    {
    }

    public void VisualUpdate(Vector2 move, Vector2 look, bool jumping, bool running, bool interact)
    {
        AnimatorStateInfo state = m_anim.GetCurrentAnimatorStateInfo(2);
        m_light.enabled = !IsHolstered() && state.IsTag("On") && state.normalizedTime >= 0.5f;

        float ySpeed = move.y * (running ? 2 : 1);
        m_x = Mathf.MoveTowards(m_x, move.x, Time.deltaTime * m_movementAdjustRate);
        m_y = Mathf.MoveTowards(m_y, ySpeed, Time.deltaTime * m_movementAdjustRate);
        m_x = Mathf.Lerp(m_x, move.x, Time.deltaTime * m_movementSmoothing);
        m_y = Mathf.Lerp(m_y, ySpeed, Time.deltaTime * m_movementSmoothing);

        m_lookX = Mathf.Lerp(m_lookX, look.x, Time.deltaTime * m_lookSmoothing);
        m_lookY = Mathf.Lerp(m_lookY, look.y, Time.deltaTime * m_lookSmoothing);

        m_anim.SetFloat("SpeedX", m_x);
        m_anim.SetFloat("SpeedY", m_y);
        m_anim.SetFloat("LookX", -m_lookX * m_lookSensitivity);
        m_anim.SetFloat("LookY", -m_lookY * m_lookSensitivity);
        m_anim.SetBool("Jumping", jumping);
        m_anim.SetBool("Holstered", m_holster);
        m_anim.SetBool("On", m_on);
    }

    private void LateUpdate()
    {
        m_flashlightRig.position = m_flashlight.position;
    }

    public void FireStart()
    {
        if (!m_holster)
        {
            m_on = !m_on;
            m_audio.clip = m_on ? m_turnOn : m_turnOff;
            m_audio.Play();
        }
    }

    public void Fire() {}
    public void Reload() {}
    public void CancelActions() {}

    public bool IsHolstered()
    {
        return m_anim.GetCurrentAnimatorStateInfo(0).IsTag("Holstered");
    }

    public bool IsReloading()
    {
        return false;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Transform GetArmsRoot()
    {
        return m_armsRoot;
    }
}
