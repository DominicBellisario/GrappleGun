using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GrappleHead : MonoBehaviour
{
    Rigidbody rb;
    GameObject grapplePoint;
    [SerializeField] GameObject grapplePointPrefab;
    [SerializeField] GameObject grappleStartPos;

    /// <summary>
    /// The speed at which the grapple head is launched from the gun
    /// </summary>
    [SerializeField] float launchSpeed;
    /// <summary>
    /// The speed at which the grapple head returns to the gun.
    /// </summary>
    [SerializeField] float returnSpeed;

    public void Launch(Vector3 target)
    {   // Stop any existing return coroutine
        StopAllCoroutines();

        //detatch the grapple head from the grapple
        transform.SetParent(null);

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        // Calculate the direction to the target
        Vector3 direction = (target - transform.position).normalized;
        // Set the velocity
        rb.linearVelocity = direction * launchSpeed;
    }

    public IEnumerator ReturnToGun()
    {
        // Disable physics while returning
        rb.isKinematic = true;

        //destroy the grapple point if it exists
        if (grapplePoint != null) { Destroy(grapplePoint); grapplePoint = null; }

        while (Vector3.Distance(transform.position, grappleStartPos.transform.position) > 0.25f)
        {
            // Move towards the grapple start position
            Vector3 direction = (grappleStartPos.transform.position - transform.position).normalized;
            rb.MovePosition(transform.position + launchSpeed * 3f * Time.deltaTime * direction);
            rb.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 0, 0);
            yield return null;
        }

        // Once close enough, snap to the grapple start position
        transform.SetPositionAndRotation(grappleStartPos.transform.position, grappleStartPos.transform.rotation);
        transform.SetParent(grappleStartPos.transform);
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the grapple head collides with a surface, stop it
        if (collision.gameObject.layer == LayerMask.NameToLayer("Surface"))
        {
            rb.linearVelocity = Vector3.zero; // Stop movement
            rb.isKinematic = true; // Disable physics

            //create a new game object to represent the grapple point
            grapplePoint = Instantiate(grapplePointPrefab, transform.position, Quaternion.identity);
            // Set the grapple point's parent to the collided object
            grapplePoint.transform.SetParent(collision.transform);
            StartCoroutine(FollowTarget());
        }
    }

    /// <summary>
    /// attach the head to the target
    /// stops running when the grapple point is destroyed
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowTarget()
    {
        Quaternion initialRotation = transform.rotation;
        while (grapplePoint != null)
        {
            // match the rotation and position of the target
            transform.SetPositionAndRotation(grapplePoint.transform.position, grapplePoint.transform.rotation * initialRotation);
            yield return null;
        }
    }
}
