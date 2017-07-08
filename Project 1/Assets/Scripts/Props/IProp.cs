using UnityEngine;

public interface IProp
{
    bool Holster { get; set; }
    bool IsHolstered { get; }
    bool IsReloading { get; }
    GameObject GameObject { get; }
    Transform ArmsRoot { get; }

    void MainUpdate();
    void VisualUpdate(Vector2 move, Vector2 look, bool jumping, bool running, bool interact);
    void Fire();
    void FireStart();
    void Reload();
    void CancelActions();
}
