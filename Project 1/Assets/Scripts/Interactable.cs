using UnityEngine;
using System;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private bool m_fireOnce = true;
    public bool FireOnce
    {
        get { return m_fireOnce; }
    }

    [SerializeField]
    private bool m_heavy = false;
    public bool Heavy
    {
        get { return m_heavy; }
    }

    public delegate void InteractHandler(Transform interacted, Vector3 interactPoint);
    public delegate void StartInteractHandler(Transform interacted, Vector3 interactPoint, Action<Interactable> endInteraction);

    public event InteractHandler Interacted;
    public event StartInteractHandler InteractStart;
    public event Action InteractEnd;

    private Outline[] m_outlines;

    private void Awake()
    {
        m_outlines = GetComponentsInChildren<Outline>(true);
        SetOutline(false);
    }

    public void SetOutline(bool showOutline)
    {
        if (enabled || (!enabled && !showOutline))
        {
            foreach (Outline outline in m_outlines)
            {
                outline.enabled = showOutline;
            }
        }
    }

    public void InteractOnce(Transform interacted, Vector3 interactPoint)
    {
        if (enabled && Interacted != null)
        {
            Interacted(interacted, interactPoint);
        }
    }

    public void StartInteract(Transform interacted, Vector3 interactPoint, Action<Interactable> endInteraction)
    {
        if (enabled && InteractStart != null)
        {
            InteractStart(interacted, interactPoint, endInteraction);
        }
    }

    public void EndInteract()
    {
        if (enabled && InteractEnd != null)
        {
            InteractEnd();
        }
    }
}
