using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterMovement m_movement;
    private PlayerInput m_input;
    private PlayerLook m_look;
    private PlayerHeadCamera m_headCamera;
    private PlayerInteract m_interact;
    private PlayerFootsteps m_footsteps;
    private PlayerWeapons m_weapons;

    private void Awake()
    {
        m_input = GetComponentInChildren<PlayerInput>();
        m_look = GetComponentInChildren<PlayerLook>();
        m_movement = GetComponentInChildren<CharacterMovement>();
        m_footsteps = GetComponentInChildren<PlayerFootsteps>();
        m_headCamera = GetComponentInChildren<PlayerHeadCamera>();
        m_interact = GetComponentInChildren<PlayerInteract>();
        m_weapons = GetComponentInChildren<PlayerWeapons>();
    }

    private void Start()
    {
        m_footsteps.Init(m_movement);
    }

    public void Spawn(Vector3 spawnPos, Quaternion facing)
    {
        m_movement.Teleport(spawnPos, facing);
    }

    private void Update()
    {
        m_interact.ProcessInteractions();
        m_weapons.UpdateVisuals(m_input, m_look, m_movement);
    }

    private void FixedUpdate()
    {
        m_weapons.UpdateWeapons();

        // Move player
        MoveInputs inputs = m_input.GetInput(m_interact);
        m_movement.UpdateMovement(inputs);

        m_footsteps.UpdateSounds(m_weapons, m_movement);
        m_look.UpdateLook(m_weapons);
        
        m_headCamera.UpdateCamera(m_movement);

        m_interact.MoveGrapTarget(m_movement);
    }
}
