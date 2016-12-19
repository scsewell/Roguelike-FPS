using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Outline[] m_outlines;

    public delegate void InteractHandler();
    public event InteractHandler Interacted;

	private void Start()
    {
        m_outlines = GetComponentsInChildren<Outline>(true);
        SetOutline(false);
    }

    public void SetOutline(bool showOutline)
    {
        foreach (Outline outline in m_outlines)
        {
            outline.enabled = showOutline;
        }
    }

    public void Interact()
    {
        if (Interacted != null)
        {
            Interacted();
        }
    }
}
