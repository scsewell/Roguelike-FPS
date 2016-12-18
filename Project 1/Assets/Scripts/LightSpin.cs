using UnityEngine;
using System.Collections;

public class LightSpin : MonoBehaviour
{
	public float spinSpeed = 1;

	void Update ()
    {
		transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime * 100, 0));
	}
}
