using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Door : MonoBehaviour
{
    private Interactable m_interact;
    private Animator m_anim;
    private bool m_doorUp = false;

    private void Start()
    {
        m_interact = GetComponent<Interactable>();
        m_anim = GetComponent<Animator>();

        m_interact.Interacted += Interacted;
    }

    private void OnDestroy()
    {
        m_interact.Interacted -= Interacted;
    }

    private void Update()
    {
        m_anim.SetBool("Open", m_doorUp);
    }

    private void Interacted(Transform interacted, Vector3 interactPoint)
    {
        m_doorUp = !m_doorUp;
    }
}
