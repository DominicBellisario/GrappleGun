using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunLag : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody playerRb;

    [Header("Sway Settings")]
    [SerializeField] float swayAmount;
    [SerializeField] float maxSway;
    [SerializeField] float swaySmooth;

    [Header("Lag Settings")]
    [SerializeField] float lagAmount;
    [SerializeField] float lagSmooth;
    [SerializeField] float lagClamp;

    [Header("Recoil Settings")]
    [SerializeField] float recoilKickback = 0.1f;   // backward position offset
    [SerializeField] float recoilRotation = 5f;     // upward rotation angle
    [SerializeField] float recoilReturnSpeed = 10f; // how fast it returns

    [Header("Vibration Settings")]
    [SerializeField] float vibrationMagnitude;
    Coroutine vibrationCoroutine;

    [HideInInspector] public Vector2 lookInput;

    Quaternion originalRotation;
    Vector3 originalPosition;

    Vector3 recoilOffset;       // positional recoil offset
    Quaternion recoilRotationOffset; // rotational recoil offset

    void Start()
    {
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
        recoilRotationOffset = Quaternion.identity;
        vibrationCoroutine = null;
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

    public void AddRecoil(float multiplier)
    {
        // Kick gun backwards
        recoilOffset += multiplier * recoilKickback * Vector3.back;

        // Rotate gun upwards (slight random side sway can be added)
        recoilRotationOffset *= Quaternion.Euler(-recoilRotation, Random.Range(-recoilRotation * 0.2f, recoilRotation * 0.2f), 0f);
    }

    private Quaternion ClampRotation(Quaternion q, float maxAngle)
    {
        q.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180) angle -= 360;
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
        return Quaternion.AngleAxis(angle, axis);
    }

    public void ToggleVibration(bool on)
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
