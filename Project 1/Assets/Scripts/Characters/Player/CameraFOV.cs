using UnityEngine;
using Framework.Interpolation;

public class CameraFOV : MonoBehaviour
{
	[SerializeField] private float m_aimFieldOfViewRatio = 0.5f;
	[SerializeField] private float m_changeRate = 5;
    [SerializeField] private float m_nearClip = 0.0065f;
    [SerializeField] private float m_farClip = 80.0f;

    private CharacterMovement m_character;
    private CharacterController m_controller;
	private Camera m_cam;
	
	private void Start()
    {
        m_character = transform.root.GetComponent<CharacterMovement>();
        m_controller = transform.root.GetComponent<CharacterController>();
		m_cam = GetComponent<Camera>();

        m_cam.fieldOfView = Settings.Instance.GetFieldOfView();
        m_cam.nearClipPlane = m_nearClip;
        m_cam.farClipPlane = m_farClip;

        InterpolatedFloat fov = new InterpolatedFloat(() => (m_cam.fieldOfView), val => { m_cam.fieldOfView = val; });
        gameObject.AddComponent<FloatInterpolator>().Initialize(fov);
    }

    private void FixedUpdate()
    {
        Vector3 controllerTop = m_controller.transform.TransformPoint(m_controller.center + ((m_controller.height / 2)) * Vector3.up);
        transform.position = controllerTop + 0.05f * Vector3.down;

		float targetFieldOfView = Settings.Instance.GetFieldOfView();

		if (Controls.Instance.IsDown(GameButton.Aim) && !m_character.IsRunning())
        {
			targetFieldOfView *= m_aimFieldOfViewRatio;
		}

		m_cam.fieldOfView = Mathf.Lerp(m_cam.fieldOfView, targetFieldOfView, Time.deltaTime * m_changeRate);
	}
}
