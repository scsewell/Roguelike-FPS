using UnityEngine;
using System.Collections;

public class PlayerInteract : MonoBehaviour
{
    public LayerMask BlockingLayers;
	public float interactDistance = 0.6f;
	public string interactiveTag;

	void Update() 
	{
		if (Controls.JustDown(GameButton.Interact)) 
		{
			RaycastHit hit;
			
			if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, BlockingLayers) && hit.collider.transform.tag == interactiveTag) 
			{
				hit.collider.SendMessageUpwards("Interact");
			}
		}
	}
}
