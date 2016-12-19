using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    private Animator m_anim;
    private bool m_doorUp = false;

    private void Start()
    {
        m_anim = GetComponent<Animator>();
    }

    private void Update()
    {
        m_anim.SetBool("Open", m_doorUp);
    }

    public void Interact()
    {
        m_doorUp = !m_doorUp;
    }
}
