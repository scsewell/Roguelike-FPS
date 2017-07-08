using UnityEngine;

public struct MoveInputs
{
    public Vector3 MoveDirection { get; set; }
    public bool Jump { get; set; }
    public bool Run { get; set; }
    public bool Crouch { get; set; }
    public bool Burdened { get; set; }

    public MoveInputs(Vector3 moveDirection, bool jump, bool run, bool crouch, bool burdened)
    {
        MoveDirection = moveDirection;
        Jump = jump;
        Run = run;
        Crouch = crouch;
        Burdened = burdened;
    }
}
