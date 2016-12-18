using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
	private CharacterMovement m_character;

	private void Start()
    {
		m_character = transform.root.GetComponent<CharacterMovement>();
	}

	private void Update()
    {
		if (Controls.IsDown(GameButton.Fire))
        {
			BroadcastMessage("Fire");
		}

		if (Controls.IsDown(GameButton.Reload) && !m_character.IsJumping())
        {
			BroadcastMessage("Reload");
		}
	}
}
