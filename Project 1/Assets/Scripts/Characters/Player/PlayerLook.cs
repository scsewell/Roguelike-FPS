using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] [Range(-90, 0)]
    private float m_minimumY = -80.0f;
    [SerializeField] [Range(0, 90)]
    private float m_maximumY = 80.0f;
	[SerializeField] [Range(0, 10)]
    private float m_sensitivityX = 5.0f;
    [SerializeField] [Range(0, 10)]
    private float m_sensitivityY = 3.5f;
	[SerializeField] [Range(0, 0.1f)]
    private float m_lookSmoothing = 0.015f;
	[SerializeField] [Range(0, 1)]
    private float m_zoomTurnRateFac = 0.5f;
    
    private float m_deltaX = 0;
    private float m_deltaY = 0;
    private float m_rotationY = 0;
    private Vector2 m_recoil = Vector2.zero;
    
    public void UpdateLook(PlayerHeadCamera headCamera)
    {
        float baseSensitivity = Mathf.Pow((ControlsManager.Instance.LookSensitivity / 2) + 0.5f, 3);
        float aimFac = (1 - m_zoomTurnRateFac) + m_zoomTurnRateFac * headCamera.AimFactor;

        float sensitivity = 60 * baseSensitivity * aimFac;
        float deltaX = ControlsManager.Instance.AverageValue(GameAxis.LookX) * m_sensitivityX * sensitivity * Time.fixedDeltaTime;
        float deltaY = ControlsManager.Instance.AverageValue(GameAxis.LookY) * m_sensitivityY * sensitivity * Time.fixedDeltaTime;
        
        float lookSensitivity = (m_lookSmoothing > 0) ? Time.fixedDeltaTime / m_lookSmoothing : 1;
        m_deltaX = Mathf.Lerp(m_deltaX, deltaX, lookSensitivity);
        m_deltaY = Mathf.Lerp(m_deltaY, deltaY, lookSensitivity);

        transform.Rotate(0, m_deltaX + m_recoil.x, 0);

        m_rotationY += m_deltaY + m_recoil.y;
        m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

        headCamera.Cam.transform.localEulerAngles = new Vector3(-m_rotationY, 0, 0);

        m_recoil = Vector2.zero;
    }

    public void AddRecoil(Vector2 recoilDelta)
    {
        m_recoil += recoilDelta;
    }

    public Vector2 GetLookDelta()
    {
        return new Vector2(m_deltaX, m_deltaY);
    }
}