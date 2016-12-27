public interface IWeapon
{
    void Fire();
    void Reload();
    void CancelReload();
    bool IsReloading();
}
