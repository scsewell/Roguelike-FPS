using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    private Animator anim;
    private bool doorUp = false;

    void Start ()
    {
        anim = GetComponent<Animator>();
    }
	
	void Update ()
    {
        anim.SetBool("Open", doorUp);
    }

    public void Interact()
    {
        doorUp = !doorUp;
    }
}
