using UnityEngine;

public class FootstepReciever : MonoBehaviour
{
    private FootstepSounds m_footsteps;

    private void Start()
    {
        m_footsteps = GetComponentInParent<FootstepSounds>();
    }

    public void FootstepL()
    {
        if (m_footsteps)
        {
            m_footsteps.FootstepL();
        }
    }

    public void FootstepR()
    {
        if (m_footsteps)
        {
            m_footsteps.FootstepR();
        }
    }
}
