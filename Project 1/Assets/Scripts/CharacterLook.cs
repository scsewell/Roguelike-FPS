using UnityEngine;
using System.Collections;

public class CharacterLook : MonoBehaviour
{
	[SerializeField] private float m_sensitivityX = 7.0f;
    [SerializeField] private float m_sensitivityY = 5.0f;
    [SerializeField] private float m_minimumY = -80.0f;
    [SerializeField] private float m_maximumY = 80.0f;
    
	private Settings m_settings;
    private PlayerWeapons m_weapons;
    private Camera m_cam;

    private float m_deltaX = 0;
    private float m_deltaY = 0;
    private float m_rotationY = 0.0f;

    private void Start() 
    {
		m_settings = GameObject.FindGameObjectWithTag("GameController").GetComponent<Settings>();
        m_weapons = transform.GetComponentInChildren<PlayerWeapons>();
        m_cam = transform.GetComponentInChildren<Camera>();
    }

	private void FixedUpdate()
    {
        float sensitivity = 60 * Mathf.Pow((m_settings.GetLookSensitivity() / 2) + 0.5f, 3) * (m_cam.fieldOfView / m_settings.GetFieldOfView());
        m_deltaX = Controls.Instance.AverageValue(GameAxis.LookX) * m_sensitivityX * sensitivity * Time.fixedDeltaTime;
        m_deltaY = Controls.Instance.AverageValue(GameAxis.LookY) * m_sensitivityY * sensitivity * Time.fixedDeltaTime;

        transform.Rotate(0, m_deltaX, 0);

        m_rotationY += m_deltaY + m_weapons.Recoil * Time.fixedDeltaTime;
        m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

        m_cam.transform.localEulerAngles = new Vector3(-m_rotationY, 0, 0);
	}

    public float GetDeltaX()
    {
        return m_deltaX;
    }

    public float GetDeltaY()
    {
        return m_deltaY;
    }
}