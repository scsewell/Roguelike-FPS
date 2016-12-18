using UnityEngine;
using System.Collections;

public class HitBoxDamage : MonoBehaviour
{
	public float damageMultiplier = 1.0F;
	private DamageReceiver damageReceiver;
	
	void Start ()
    {
		damageReceiver = transform.root.GetComponent<DamageReceiver>();
	}
	
	void RecieveDamage (float damage)
    {
		damageReceiver.ApplyDamage(damage * damageMultiplier);
	}
}
