using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Interactable : MonoBehaviour
{
    private Renderer[] m_renderers;
    
	private void Start()
    {
        /*
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            GameObject go = new GameObject;
            go.transform.SetParent(renderer.transform, false);
            go.add
        }
        m_renderers = GetComponentsInChildren<Renderer>(true);
        */
    }

    public void ShowOutline()
    {
    }
}
