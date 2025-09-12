using UnityEngine;
using UnityEngine.InputSystem;

public class GunLag : MonoBehaviour
{
     [Header("References")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody playerRb;

    [Header("Sway Settings")]
    [SerializeField] float swayAmount = 2f;
    [SerializeField] float maxSway = 5f;
    [SerializeField] float swaySmooth = 6f;

    [Header("Lag Settings")]
    [SerializeField] float lagAmount = 0.05f;   // how far the gun shifts
    [SerializeField] float lagSmooth = 6f;     // how quickly it catches up
    [SerializeField] float lagClamp;

    [HideInInspector]
    public Vector2 lookInput;

    Quaternion originalRotation;
    Vector3 originalPosition;

    void Start()
    {
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Calculate target sway rotation
        Quaternion xQuat = Quaternion.AngleAxis(-swayAmount * lookInput.x, Vector3.up);
        Quaternion yQuat = Quaternion.AngleAxis(swayAmount * lookInput.y, Vector3.right);
        Quaternion targetRotation = originalRotation * xQuat * yQuat;

        // Clamp sway
        targetRotation = ClampRotation(targetRotation, maxSway);

        // Smoothly interpolate
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * swaySmooth);

        //reset look input
        lookInput = Vector2.zero;


        // calculate how far the gun should be from the player
        Vector3 localVel = cameraTransform.InverseTransformDirection(playerRb.linearVelocity);
        Vector3 lagOffset = new Vector3(-localVel.x, -localVel.y, -localVel.z) * lagAmount;
        // gun cannot go too far away from player
        lagOffset = Vector3.ClampMagnitude(lagOffset, lagClamp);

        // Smooth movement toward calculated lag position
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalPosition + lagOffset,
            Time.deltaTime * lagSmooth
        );
    }

    private Quaternion ClampRotation(Quaternion q, float maxAngle)
    {
        q.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180) angle -= 360;
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
        return Quaternion.AngleAxis(angle, axis);
    }
}
