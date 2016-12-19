using UnityEngine;

public class HitBoxDamage : MonoBehaviour
{
	public float damageMultiplier = 1f;

	private DamageReceiver m_damageReciever;
	
	private void Start()
    {
		m_damageReciever = transform.root.GetComponent<DamageReceiver>();
	}

    private void RecieveDamage(float damage)
    {
		m_damageReciever.ApplyDamage(damage * damageMultiplier);
	}
}
