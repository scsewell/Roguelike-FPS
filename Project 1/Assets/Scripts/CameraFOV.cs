using UnityEngine;
using System.Collections;

public class CameraFOV : MonoBehaviour
{
	public float aimFieldOfViewRatio = 0.5f;
	public float changeRate = 5;
    public float nearClip = 0.0065f;
    public float farClip = 80.0f;
    
	private Settings m_settings;
	private CharacterMovement m_character;
	private Camera m_cam;
	
	void Start ()
    {
		m_settings = GameObject.FindGameObjectWithTag("GameController").GetComponent<Settings>();
		m_character = transform.root.GetComponent<CharacterMovement>();
		m_cam = GetComponent<Camera>();

        m_cam.fieldOfView = m_settings.GetFieldOfView();
        m_cam.nearClipPlane = nearClip;
        m_cam.farClipPlane = farClip;
    }

	void Update ()
    {
		float targetFieldOfView = m_settings.GetFieldOfView();

		if (Controls.IsDown(GameButton.Aim) && !m_character.IsRunning())
        {
			targetFieldOfView = m_settings.GetFieldOfView() * aimFieldOfViewRatio;
		}

		m_cam.fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, targetFieldOfView, Time.deltaTime * changeRate);
	}
}
