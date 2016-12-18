using UnityEngine;
using System.Collections;

public class DamageReceiver : MonoBehaviour {
	
	public float hitPoints = 100.0f;
	public Transform deadReplacement;

	private bool alive = true;

	public void ApplyDamage (float damage) {

		hitPoints -= damage;

		if (hitPoints <= 0.0 && alive) {
			alive = false;
			Kill();
		}
	}

	void Kill () {
		if (deadReplacement) {
			Transform replacement = (Transform)Instantiate(deadReplacement, transform.position, transform.rotation);

			Transform[] allReplacementChildren = replacement.GetComponentsInChildren<Transform>();
			
			foreach (Transform replacementChild in allReplacementChildren) {

				Transform child = Find(transform, replacementChild.name);

				if (child != null) {
					replacementChild.position = child.position;
					replacementChild.rotation = child.rotation;
				}
			}
		}
		Destroy(gameObject);
	}
	
	Transform Find (Transform parent, string searchName) {

		foreach (Transform child in parent.GetComponentsInChildren<Transform>()) {

			if (child.name == searchName) {
				return child;
			}
		}
		return null;
	}
}
