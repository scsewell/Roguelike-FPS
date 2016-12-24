using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
	private CharacterMovement m_character;

    private float m_recoil = 0;
    public float Recoil
    {
        get { return m_recoil; }
        set { m_recoil = value; }
    }

	private void Start()
    {
		m_character = transform.root.GetComponent<CharacterMovement>();
	}

	private void Update()
    {
		if (Controls.Instance.IsDown(GameButton.Fire))
        {
			BroadcastMessage("Fire");
		}

		if (Controls.Instance.IsDown(GameButton.Reload) && !m_character.IsJumping())
        {
			BroadcastMessage("Reload");
		}
	}
}
