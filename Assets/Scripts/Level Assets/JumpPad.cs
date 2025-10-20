using System.Collections;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float launchForce;
    [SerializeField] float coolDownTime;
    [SerializeField] BoxCollider jumpPadCollider;

    [Header("Gizmo Settings")]
    // how many segments to draw
    [SerializeField] int segments = 30;
    // time between points      
    [SerializeField] float timeStep = 0.1f;
    [SerializeField] LayerMask gizmoMask;

    [SerializeField] AudioSource audioSource;

    public Vector3 GetLaunchForceAndActivatePad()
    {
        StartCoroutine(DisableJumpPad());
        // play sound
        audioSource.Play();
        return transform.up * launchForce;
    }

    private IEnumerator DisableJumpPad()
    {
        jumpPadCollider.enabled = false;
        yield return new WaitForSeconds(coolDownTime);
        jumpPadCollider.enabled = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 velocity = transform.up * launchForce;

        Vector3 previousPoint = transform.position;

        for (int i = 1; i <= segments; i++)
        {
            float t = i * timeStep;
            Vector3 newPoint = transform.position + velocity * t + t * t * 0.5f * Physics.gravity;

            // Check for collision between previousPoint and newPoint
            if (Physics.Raycast(previousPoint, (newPoint - previousPoint).normalized, out RaycastHit hit, (newPoint - previousPoint).magnitude, gizmoMask))
            {
                Gizmos.DrawLine(previousPoint, hit.point);
                // Stop drawing once collision is detected
                break;
            }
            else
            {
                Gizmos.DrawLine(previousPoint, newPoint);
                previousPoint = newPoint;
            }
        }
    }
}
