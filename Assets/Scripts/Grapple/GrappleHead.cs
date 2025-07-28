using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GrappleHead : MonoBehaviour
{
    Rigidbody rb;
    GameObject grapplePoint;
    bool detectCollisions;

    [Header("Game Objects")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject grapplePointPrefab;
    [SerializeField] GameObject grappleStartPos;

    [Header("Grapple Settings")]
    /// <summary>
    /// The speed at which the grapple head is launched from the gun
    /// </summary>
    [SerializeField] float launchSpeed;
    /// <summary>
    /// The speed at which the grapple head returns to the gun.
    /// </summary>
    [SerializeField] float returnSpeed;
    /// <summary>
    /// the maximum range of the grapple
    /// </summary>
    public float maxDistance;

    public bool IsAttached { get { return !detectCollisions; } }

    /// <summary>
    /// the current distance between the grapple head and the launcher
    /// </summary>
    public float CurrentRopeLength { get { return Vector3.Distance(transform.position, grappleStartPos.transform.position); } }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        detectCollisions = true;
    }

    void Update()
    {
        if (!rb.isKinematic)
        {
            if (CurrentRopeLength > maxDistance)
            {
                StartCoroutine(ReturnToGun());
            }
        }
    }
    public void Launch(Vector3 target)
    {
        // Stop any existing return coroutine
        StopAllCoroutines();

        // detatch the grapple head from the grapple
        transform.SetParent(null);

        // rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Calculate the direction to the target
        Vector3 direction;
        // use the raycast point to calcaulte the direction
        if (target != Vector3.zero) { direction = (target - transform.position).normalized; }
        // use the camera forward
        else { direction = player.GetComponentInChildren<Camera>().gameObject.transform.forward; }

        // Set the velocity
        rb.linearVelocity = direction * launchSpeed;
    }

    public IEnumerator ReturnToGun()
    {
        // cannot launch gun while returning
        player.GetComponent<PlayerController>().CanUseGrapple = false;

        // Disable physics while returning
        rb.isKinematic = true;
        detectCollisions = true;

        // remove any possible grapple joint
        player.GetComponent<GrapplePhysics>().DestroyGrapple();

        // destroy the grapple point if it exists
        if (grapplePoint != null) { Destroy(grapplePoint); grapplePoint = null; }

        float i = 0f;
        while (CurrentRopeLength > 0.25f)
        {
            i++;
            // Move towards the grapple start position
            Vector3 direction = (grappleStartPos.transform.position - transform.position).normalized;
            rb.MovePosition(transform.position + returnSpeed * Time.deltaTime * direction * (1 + (i * 0.02f)));
            rb.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 0, 0);
            yield return null;
        }

        // Once close enough, snap to the grapple start position
        transform.SetPositionAndRotation(grappleStartPos.transform.position, grappleStartPos.transform.rotation);
        transform.SetParent(grappleStartPos.transform);
        rb.interpolation = RigidbodyInterpolation.None;

        player.GetComponent<PlayerController>().CanUseGrapple = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // can only act on one collision event
        if (!detectCollisions) return;
        detectCollisions = false;

        // If the grapple head collides with a surface, create a normal, non elastic grapple
        if (collision.gameObject.layer == LayerMask.NameToLayer("Surface"))
        {
            CreateGrapplePoint(collision, 0f, 0f);
        }
        // If it collides with a bird, create an elastic grapple that pulls the player toward it
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Bird"))
        {
            CreateGrapplePoint(collision, 20f, 10f);
            //player cannot use the grapple until they are pulled in fully
            player.GetComponent<PlayerController>().CanUseGrapple = false;
        }
        // If it collides with a non grappleable surface, send the grapple back
        else if (collision.gameObject.layer == LayerMask.NameToLayer("No Grapple"))
        {
            StartCoroutine(ReturnToGun());
        }
    }

    private void CreateGrapplePoint(Collision collision, float elasticity, float damper)
    {

        rb.linearVelocity = Vector3.zero; // Stop movement
        rb.isKinematic = true; // Disable physics

        // create a new game object to represent the grapple point
        grapplePoint = Instantiate(grapplePointPrefab, transform.position, Quaternion.identity);
        // Set the grapple point's parent to the collided object
        grapplePoint.transform.SetParent(collision.transform);
        //make the grapple head match the point's transform
        StartCoroutine(FollowTarget());

        //create a configurable joint
        player.GetComponent<GrapplePhysics>().CreateGrapple(elasticity, damper);
    }

    /// <summary>
    /// attach the head to the grapple point
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
