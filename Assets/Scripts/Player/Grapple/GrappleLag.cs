using UnityEngine;

public class GrappleLag : WeaponLag
{
    protected override void OnEnable()
    {
        base.OnEnable();
        PlayerController.OnShootGrappleEvent += AddShootRecoil;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerController.OnShootGrappleEvent -= AddShootRecoil;
    }

    public void AddReturnRecoil(float multiplier)
    {
        // Kick gun backwards
        recoilOffset += multiplier * recoilShootKickback * Vector3.back;

        // Rotate gun upwards (slight random side sway can be added)
        recoilRotationOffset *= Quaternion.Euler(-recoilShootRotation, 0f, 0f);
    }
}
