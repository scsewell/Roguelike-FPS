using UnityEngine;
using Framework.Interpolation;

public class PlayerHeadCamera : MonoBehaviour
{
	[SerializeField] [Range(0, 0.3f)]
    private float m_headOffset = 0.05f;
	[SerializeField] [Range(0.1f, 1)]
    private float m_aimFieldOfViewRatio = 0.5f;
	[SerializeField] [Range(1, 16)]
    private float m_changeRate = 6;
    [SerializeField]
    private float m_nearClip = 0.0065f;
    [SerializeField]
    private float m_farClip = 80.0f;
    
	private Camera m_cam;
    public Camera Cam
    {
        get { return m_cam; }
    }
    
    private bool m_aiming = false;
    public bool Aiming
    {
        get { return m_aiming; }
        private set { m_aiming = value; }
    }

    public float AimFactor
    {
        get
        {
            if (m_aimFieldOfViewRatio == 1)
            {
                return 0;
            }
            return Mathf.Clamp01(1 + (((m_cam.fieldOfView / SettingManager.Instance.FieldOfView) - 1) / (1 - m_aimFieldOfViewRatio)));
        }
    }

	private void Start()
    {
		m_cam = GetComponent<Camera>();

        m_cam.fieldOfView = SettingManager.Instance.FieldOfView;
        InterpolatedFloat fov = new InterpolatedFloat(() => (m_cam.fieldOfView), val => { m_cam.fieldOfView = val; });
        gameObject.AddComponent<FloatInterpolator>().Initialize(fov);

        m_cam.gameObject.AddComponent<TransformInterpolator>();
    }

    public void UpdateCamera(CharacterMovement movement)
    {
        transform.position = movement.ColliderTop + m_headOffset * Vector3.down;

        m_cam.nearClipPlane = m_nearClip;
        m_cam.farClipPlane = m_farClip;
        
        if (ControlsManager.Instance.JustDown(GameButton.AimToggle))
        {
            Aiming = !Aiming;
        }
        
        if (ControlsManager.Instance.IsDown(GameButton.Aim))
        {
            Aiming = true;
        }

        if (ControlsManager.Instance.JustUp(GameButton.Aim))
        {
            Aiming = false;
        }

        if (movement.IsRunning)
        {
            Aiming = false;
        }

        float targetFieldOfView = SettingManager.Instance.FieldOfView;

		if (Aiming)
        {
			targetFieldOfView *= m_aimFieldOfViewRatio;
		}

		m_cam.fieldOfView = Mathf.Lerp(m_cam.fieldOfView, targetFieldOfView, Time.deltaTime * m_changeRate);
	}
}
