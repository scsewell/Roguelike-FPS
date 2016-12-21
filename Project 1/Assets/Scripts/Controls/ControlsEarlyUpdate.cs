using UnityEngine;

public class ControlsEarlyUpdate : MonoBehaviour
{
    Controls m_controls;

	private void Awake()
    {
        m_controls = GetComponent<Controls>();
    }
	
	private void Update()
    {
        m_controls.EarlyUpdate();
    }
}
