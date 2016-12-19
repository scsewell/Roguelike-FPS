using UnityEngine;

public class ExoskeletonEnemyAnimation : MonoBehaviour
{
	private Animator m_anim;

    private void Start()
    {
		m_anim = GetComponent<Animator>();
	}

    private void FixedUpdate()
    {
		m_anim.SetFloat("forwardSpeed", 0);
		m_anim.SetFloat("sidewaysSpeed", 0);
	}
}
