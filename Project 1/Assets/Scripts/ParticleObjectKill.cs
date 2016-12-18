using UnityEngine;
using System.Collections;

public class ParticleObjectKill : MonoBehaviour {
	
	void Update () {
		if (!GetComponent<ParticleSystem>().IsAlive()) {
			Object.Destroy (this.gameObject);
		}
	}
}
