using UnityEngine;

public class ParticleObjectKill : MonoBehaviour
{
	private void Update()
    {
		if (!GetComponent<ParticleSystem>().IsAlive())
        {
			Destroy(gameObject);
		}
	}
}
