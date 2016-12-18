using UnityEngine;
using System.Collections;

public class ExoskeletonEnemyAnimation : MonoBehaviour {
	
	private Animator anim;

	void Start () {
		anim = GetComponent<Animator>();
	}

	void FixedUpdate () {
		anim.SetFloat("forwardSpeed", 0);
		anim.SetFloat("sidewaysSpeed", 0);
	}
}
