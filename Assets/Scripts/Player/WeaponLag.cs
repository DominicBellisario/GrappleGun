using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class WeaponLag : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] protected Rigidbody playerRb;

    [Header("Sway Settings")]
    [SerializeField] protected float swayAmount;
    [SerializeField] protected float maxSway;
    [SerializeField] protected float swaySmooth;

    [Header("Lag Settings")]
    [SerializeField] protected float lagAmount;
    [SerializeField] protected float lagSmooth;
    [SerializeField] protected float lagClamp;

    [Header("Recoil Settings")]
    [SerializeField] protected float recoilShootKickback;   // backward position offset
    [SerializeField] protected float recoilShootRotation;     // upward rotation angle
    [SerializeField] protected float recoilDashKickback;
    [SerializeField] protected float recoilReturnSpeed; // how fast it returns

    [Header("Vibration Settings")]
    [SerializeField] protected float vibrationMagnitude;
    Coroutine vibrationCoroutine;

    [HideInInspector] public Vector2 lookInput;

    Quaternion originalRotation;
    Vector3 originalPosition;

    protected Vector3 recoilOffset;       // positional recoil offset
    protected Quaternion recoilRotationOffset; // rotational recoil offset

    void Start()
    {
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
        recoilRotationOffset = Quaternion.identity;
        vibrationCoroutine = null;
    }

    protected virtual void OnEnable()
    {
        PlayerController.OnDashEvent += AddDashRecoil;
        PlayerController.OnBoostStartEvent += () => ToggleVibration(true);
        PlayerController.OnBoostStopEvent += () => ToggleVibration(false);
    }

    protected virtual void OnDisable()
    {
        PlayerController.OnDashEvent -= AddDashRecoil;
        PlayerController.OnBoostStartEvent -= () => ToggleVibration(true);
        PlayerController.OnBoostStopEvent -= () => ToggleVibration(false);
    }

    void Update()
    {
        // --- Sway ---
        Quaternion xQuat = Quaternion.AngleAxis(-swayAmount * lookInput.x * Time.deltaTime, Vector3.up);
        Quaternion yQuat = Quaternion.AngleAxis(swayAmount * lookInput.y * Time.deltaTime, Vector3.right);
        Quaternion targetRotation = originalRotation * xQuat * yQuat;

        targetRotation = ClampRotation(targetRotation, maxSway);

        // --- Recoil Rotation ---
        recoilRotationOffset = Quaternion.Slerp(recoilRotationOffset, Quaternion.identity, Time.deltaTime * recoilReturnSpeed);
        targetRotation *= recoilRotationOffset;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * swaySmooth);

        lookInput = Vector2.Lerp(lookInput, Vector2.zero, Time.deltaTime * swaySmooth);

        // --- Lag ---
        Vector3 localVel = cameraTransform.InverseTransformDirection(playerRb.linearVelocity);
        Vector3 lagOffset = new Vector3(localVel.x, -localVel.y * 2, localVel.z) * lagAmount;
        lagOffset = Vector3.ClampMagnitude(lagOffset, lagClamp);

        // --- Recoil Position ---
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalPosition + lagOffset + recoilOffset,
            Time.deltaTime * lagSmooth
        );
    }

    protected virtual void AddShootRecoil()
    {
        // Kick gun backwards
        recoilOffset += recoilShootKickback * Vector3.back;

        // Rotate gun upwards (slight random side sway can be added)
        recoilRotationOffset *= Quaternion.Euler(-recoilShootRotation, 0f, 0f);
    }

    private void AddDashRecoil()
    {
        // Kick gun backwards
        recoilOffset += recoilDashKickback * Vector3.back;
    }

    private Quaternion ClampRotation(Quaternion q, float maxAngle)
    {
        q.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180) angle -= 360;
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
        return Quaternion.AngleAxis(angle, axis);
    }

    private void ToggleVibration(bool on)
    {
        if (vibrationCoroutine != null) StopCoroutine(vibrationCoroutine);
        if (on) vibrationCoroutine = StartCoroutine(VibrateGun());
    }

    private IEnumerator VibrateGun()
    {
        Vector3 originalPos = transform.localPosition;

        while (true)
        {
            float x = Random.Range(-1f, 1f) * vibrationMagnitude;
            float y = Random.Range(-1f, 1f) * vibrationMagnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            yield return null;
        }
    }
}
