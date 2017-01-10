using UnityEngine;

public interface IProp
{
    bool Holster { get; set; }

    void MainUpdate();
    void VisualUpdate(Vector2 move, Vector2 look, bool jumping, bool running, bool interact);
    void Fire();
    void FireStart();
    void Reload();
    void CancelActions();
    bool IsHolstered();
    bool IsReloading();
    GameObject GetGameObject();
    Transform GetArmsRoot();
}
