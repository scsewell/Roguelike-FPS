using UnityEngine;

public class LightSpin : MonoBehaviour
{
	public float spinSpeed = 1;

	private void Update()
    {
		transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime * 100, 0));
	}
}
