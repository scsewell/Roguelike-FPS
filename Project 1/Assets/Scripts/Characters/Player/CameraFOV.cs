using UnityEngine;

public class CameraFOV : MonoBehaviour
{
	[SerializeField] private float m_aimFieldOfViewRatio = 0.5f;
	[SerializeField] private float m_changeRate = 5;
    [SerializeField] private float m_nearClip = 0.0065f;
    [SerializeField] private float m_farClip = 80.0f;
    
	private CharacterMovement m_character;
	private Camera m_cam;
	
	private void Start()
    {
		m_character = transform.root.GetComponent<CharacterMovement>();
		m_cam = GetComponent<Camera>();

        m_cam.fieldOfView = Settings.Instance.GetFieldOfView();
        m_cam.nearClipPlane = m_nearClip;
        m_cam.farClipPlane = m_farClip;
    }

    private void Update()
    {
		float targetFieldOfView = Settings.Instance.GetFieldOfView();

		if (Controls.Instance.IsDown(GameButton.Aim) && !m_character.IsRunning())
        {
			targetFieldOfView = Settings.Instance.GetFieldOfView() * m_aimFieldOfViewRatio;
		}

		m_cam.fieldOfView = Mathf.Lerp(m_cam.fieldOfView, targetFieldOfView, Time.deltaTime * m_changeRate);
	}
}
