using UnityEngine;

public class Raycasts : MonoBehaviour
{
    /// <summary>
    /// distance to check for a raycast hit downwards
    /// </summary>
    [SerializeField] float downRaycastDistance;
    [SerializeField] float forwardRaycastDistance;

    /// <summary>
    /// sends a raycast downwards from the player
    /// </summary>
    public RaycastHit DownRaycastHit
    {
        get
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, downRaycastDistance, LayerMask.GetMask("Surface")))
            {
                return hit;
            }
            return new RaycastHit(); // return an empty RaycastHit if no hit
        }
    }

    /// <summary>
    /// sends a raycast forward from the player
    /// </summary>
    public RaycastHit ForwardRaycastHit
    {
        get
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, forwardRaycastDistance, LayerMask.GetMask("Surface")))
            {
                return hit;
            }
            return new RaycastHit(); // return an empty RaycastHit if no hit
        }
    }

    void OnDrawGizmos()
    {
        // Draw a ray in the editor to visualize the raycast
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * downRaycastDistance);
        Gizmos.DrawRay(transform.position, transform.forward * forwardRaycastDistance);
    }
}
