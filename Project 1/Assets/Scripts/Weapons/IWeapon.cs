using UnityEngine;

public interface IWeapon
{
    void SetHolster(bool holster);
    void Fire();
    void Reload();
    void CancelReload();
    bool IsHolstered();
    bool IsReloading();
    GameObject GetGameObject();
}
