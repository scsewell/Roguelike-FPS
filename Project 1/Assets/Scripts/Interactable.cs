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
    public delegate void StartInteractHandler(Transform grabTarget, Transform interacted, Vector3 interactPoint);

    public event InteractHandler Interacted;
    public event StartInteractHandler InteractStart;
    public event Action InteractEnd;

    private Outline[] m_outlines;
    private Action m_endInteraction;

    private void Awake()
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

    public void EndInteraction()
    {
        if (m_endInteraction != null)
        {
            m_endInteraction();
        }
    }

    public void OnInteractOnce(Transform interacted, Vector3 interactPoint)
    {
        if (Interacted != null)
        {
            Interacted(interacted, interactPoint);
        }
    }

    public void OnStartInteract(Transform grabTarget, Transform interacted, Vector3 interactPoint, Action endInteraction)
    {
        if (InteractStart != null)
        {
            InteractStart(grabTarget, interacted, interactPoint);
            m_endInteraction = endInteraction;
        }
    }

    public void OnEndInteract()
    {
        if (InteractEnd != null)
        {
            InteractEnd();
            m_endInteraction = null;
        }
    }
}
