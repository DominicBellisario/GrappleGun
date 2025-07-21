using UnityEngine;

public class Raycasts : MonoBehaviour
{
    /// <summary>
    /// distance to check for a raycast hit downwards
    /// </summary>
    [SerializeField] float downRaycastDistance;

    Rigidbody rb;

    /// <summary>
    /// Checks if there is a raycast hit downwards
    /// </summary>
    public bool DownRaycastHit
    {
        get { return Physics.Raycast(transform.position, Vector3.down, downRaycastDistance, LayerMask.GetMask("Surface")); }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnDrawGizmos()
    {
        // Draw a ray in the editor to visualize the raycast
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * downRaycastDistance);
    }
}
