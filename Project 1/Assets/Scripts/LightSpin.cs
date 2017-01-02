using UnityEngine;

public class LightSpin : MonoBehaviour
{
	public float spinSpeed = 100;

	private void Update()
    {
		transform.Rotate(spinSpeed * Time.deltaTime * Vector3.up);
	}
}
