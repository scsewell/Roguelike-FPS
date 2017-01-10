using UnityEngine;

public class FootstepReciever : MonoBehaviour
{
    private FootstepSounds m_footsteps;

    private void Start()
    {
        m_footsteps = transform.root.GetComponent<FootstepSounds>();
    }

    public void FootstepL()
    {
        m_footsteps.FootstepL();
    }

    public void FootstepR()
    {
        m_footsteps.FootstepR();
    }
}
