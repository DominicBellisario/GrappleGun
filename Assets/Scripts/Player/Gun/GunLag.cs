using UnityEngine;

public class GunLag : WeaponLag
{
    protected override void OnEnable()
    {
        base.OnEnable();
        PlayerController.OnShootGunEvent += (raycastHit) => AddShootRecoil();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerController.OnShootGunEvent -= (raycastHit) => AddShootRecoil();
    }
}
