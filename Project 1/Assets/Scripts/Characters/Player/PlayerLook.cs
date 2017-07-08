using UnityEngine;

public class PlayerLook : MonoBehaviour
{
	[SerializeField] [Range(0, 10)]
    private float m_sensitivityX = 5.0f;
    [SerializeField] [Range(0, 10)]
    private float m_sensitivityY = 3.5f;
    [SerializeField] [Range(-90, 0)]
    private float m_minimumY = -80.0f;
    [SerializeField] [Range(0, 90)]
    private float m_maximumY = 80.0f;
    
    private Camera m_cam;
    
    private float m_deltaX = 0;
    private float m_deltaY = 0;
    private float m_rotationY = 0;

    private void Awake() 
    {
        m_cam = GetComponentInChildren<Camera>();
    }

    public void UpdateLook(PlayerWeapons weapons)
    {
        float sensitivity = 60 * Mathf.Pow((ControlsManager.Instance.LookSensitivity / 2) + 0.5f, 3) * (m_cam.fieldOfView / 60);
        m_deltaX = ControlsManager.Instance.AverageValue(GameAxis.LookX) * m_sensitivityX * sensitivity * Time.fixedDeltaTime;
        m_deltaY = ControlsManager.Instance.AverageValue(GameAxis.LookY) * m_sensitivityY * sensitivity * Time.fixedDeltaTime;

        transform.Rotate(0, m_deltaX, 0);

        m_rotationY += m_deltaY + (weapons.Recoil * Time.fixedDeltaTime);
        m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

        m_cam.transform.localEulerAngles = new Vector3(-m_rotationY, 0, 0);
	}

    public Vector2 GetLookDelta()
    {
        return new Vector2(m_deltaX, m_deltaY);
    }
}