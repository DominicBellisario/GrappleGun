using UnityEngine;

public class GrappleLag : WeaponLag
{
    protected override void OnEnable()
    {
        base.OnEnable();
        PlayerController.OnShootGrappleEvent += (raycastHit) => AddShootRecoil();
        GrappleHead.OnEndGrappleReturnEvent += AddReturnRecoil;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerController.OnShootGrappleEvent -= (raycastHit) => AddShootRecoil();
        GrappleHead.OnEndGrappleReturnEvent -= AddReturnRecoil;
    }

    public void AddReturnRecoil(float timeGrappleWasFlying)
    {
        float multiplier = Mathf.Round(Mathf.Clamp(timeGrappleWasFlying + 0.45f, 0f, 1f));

        // Kick gun backwards
        recoilOffset += multiplier * recoilShootKickback * Vector3.back;

        // Rotate gun upwards (slight random side sway can be added)
        recoilRotationOffset *= Quaternion.Euler(-recoilShootRotation, 0f, 0f);
    }
}
