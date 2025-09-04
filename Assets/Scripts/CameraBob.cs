using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody playerRb;
    [SerializeField] Raycasts playerRays;

    [Header("Settings")]
    [SerializeField] AnimationCurve bobCurve;   // the curve the bob follows
    [SerializeField] float bobFrequency = 6f;   // how fast the bob oscillates
    [SerializeField] float bobHeight = 0.05f;   // how high the camera moves
    [SerializeField] float smooth = 8f;         // smooths movement

    private float timer = 0f;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // check if player is moving on the ground
        if (playerRb.linearVelocity.magnitude > 0.1f && playerRays.DownRaycastHit.collider != null)
        {
            // increment timer based on movement speed
            timer += Time.deltaTime * bobFrequency * Mathf.Clamp(playerRb.linearVelocity.magnitude, 0, 2);

            // vertical bobbing
            float bobOffset = (bobCurve.Evaluate(timer % 1) - 0.5f) * 2f * bobHeight;

            // apply to position
            Vector3 targetPosition = initialPosition + Vector3.up * bobOffset;

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smooth);
        }
        else
        {
            // reset back when not moving
            timer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * smooth);
        }
    }
}
