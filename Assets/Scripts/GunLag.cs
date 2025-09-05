using UnityEngine;
using UnityEngine.InputSystem;

public class GunLag : MonoBehaviour
{
     [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody playerRb;

    [Header("Sway Settings")]
    [SerializeField] private float swayAmount = 2f;
    [SerializeField] private float maxSway = 5f;
    [SerializeField] private float swaySmooth = 6f;

    [Header("Lag Settings")]
    [SerializeField] private float lagAmount = 0.05f;   // how far the gun shifts
    [SerializeField] private float lagSmooth = 6f;     // how quickly it catches up

    [HideInInspector]
    public Vector2 lookInput;

    private Quaternion originalRotation;
    private Vector3 originalPosition;

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


        // Offset based on player velocity in local space
        Vector3 localVel = cameraTransform.InverseTransformDirection(playerRb.linearVelocity);
        Vector3 lagOffset = new Vector3(-localVel.x, -localVel.y, -localVel.z) * lagAmount;

        // Smooth movement toward offset position
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
