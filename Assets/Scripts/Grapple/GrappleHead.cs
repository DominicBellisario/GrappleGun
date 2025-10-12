using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class GrappleHead : MonoBehaviour
{
    Rigidbody rb;
    GameObject grapplePoint;
    SphereCollider col;
    bool detectCollisions;

    [Header("Game Objects")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject grapplePointPrefab;
    [SerializeField] GameObject grappleStartPos;
    [SerializeField] GunLag grappleLag;
    [SerializeField] GameObject sparksBurstPrefab;
    [SerializeField] GameObject muzzleFlashPrefab;
    [SerializeField] AudioSource grappleReelSound;

    GVar gvar;
    float startColRadius;

    public bool IsAttached { get { return !detectCollisions; } }

    /// <summary>
    /// the current distance between the grapple head and the launcher
    /// </summary>
    public float CurrentRopeLength { get; set; }

    void Start()
    {
        gvar = GVar.Instance;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        col.enabled = false;
        detectCollisions = true;
        startColRadius = col.radius;
    }

    void Update()
    {
        // get the distance between the grapple head and the player ONCE so everything else can use it without recalculating
        CurrentRopeLength = Vector3.Distance(transform.position, player.transform.position);

        //if not attached and at max diatnce, return to gun
        if (!rb.isKinematic)
        {
            if (CurrentRopeLength > gvar.GrappleMaxDistance)
            {
                StartCoroutine(ReturnToGun());
            }
        }
    }
    public void Launch(RaycastHit hit)
    {
        // Stop any existing return coroutine
        StopAllCoroutines();

        //add recoil
        grappleLag.AddRecoil(1f);

        // spawn a burst of sparks
        GameObject flash = Instantiate(muzzleFlashPrefab, grappleStartPos.transform.position, Quaternion.identity);
        flash.transform.SetParent(grappleStartPos.transform);

        // detatch the grapple head from the grapple
        transform.SetParent(null);

        col.enabled = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Calculate the direction to the target
        Vector3 direction;
        Vector3 target = hit.point;
        // use the raycast point to calcaulte the direction normally
        if (target != Vector3.zero && Vector3.Distance(target, transform.position) > 1.5f)
        {
            direction = (hit.point - transform.position).normalized;
        }
        // if there is no target in range or the target is too close, use the camera forward
        else
        {
            direction = player.GetComponentInChildren<Camera>().gameObject.transform.forward;
        }

        // Set the velocity
        rb.linearVelocity = direction * gvar.GrappleLaunchSpeed;

        // play the reel sound
        grappleReelSound.Play();

        //ignore collisions if the grapple is launched while already touching something
        Collider headCollider = GetComponent<Collider>();
        Collider[] overlaps = Physics.OverlapSphere(headCollider.bounds.center, 0.35f);

        foreach (Collider col in overlaps)
        {
            // do not disable collision for self and the object the player is looking at
            if (col != headCollider && col != hit.collider)
            {
                Physics.IgnoreCollision(headCollider, col, true);
                StartCoroutine(ReenableCollision(headCollider, col));
            }
        }
    }

    public IEnumerator ReturnToGun()
    {
        //Debug.Log("Returning to gun");
        // cannot launch gun while returning
        player.GetComponent<PlayerController>().CanUseGrapple = false;

        // Disable physics while returning
        rb.isKinematic = true;
        // disable enemy collision
        rb.excludeLayers = LayerMask.GetMask("Enemy");
        // re-disable gravity
        rb.useGravity = false;
        col.radius = startColRadius;
        rb.freezeRotation = false;

        // remove any possible grapple joint
        player.GetComponent<GrapplePhysics>().DestroyGrapple();

        // destroy the grapple point if it exists
        if (grapplePoint != null) { Destroy(grapplePoint); grapplePoint = null; }

        // play the reel sound
        grappleReelSound.Play();

        float timer = 0f;
        // come back to the player until it gets there or until it takes too long
        while (CurrentRopeLength > gvar.GrappleReturnRadius + timer && timer < gvar.ReelAutoDetatchTime)
        {
            timer += Time.deltaTime;
            // get the direction the grapple should move in
            Vector3 direction = (grappleStartPos.transform.position - transform.position).normalized;
            // move the grapple in that direction.  it gets faster the longer it returns
            rb.MovePosition(transform.position + gvar.GrappleReturnSpeed * Time.deltaTime * direction * (1 + (timer * 0.2f)));
            rb.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 0, 0);
            yield return new WaitForFixedUpdate();
        }

        // Once close enough
        // reenable collisions
        col.enabled = false;
        detectCollisions = true;
        // disable enemy collision
        rb.excludeLayers = 0;

        // add recoil as long as the grapple head was actually coming back and not already back
        grappleLag.AddRecoil(Mathf.Round(Mathf.Clamp(timer + 0.45f, 0f, 1f)));
        transform.SetPositionAndRotation(grappleStartPos.transform.position, grappleStartPos.transform.rotation);
        
        // snap to the grapple start position
        transform.SetParent(grappleStartPos.transform);

        rb.interpolation = RigidbodyInterpolation.None;

        player.GetComponent<PlayerController>().CanUseGrapple = true;

        // spawn a burst of sparks
        if (timer != 0)
        {
            GameObject sparks = Instantiate(sparksBurstPrefab, transform.position, Quaternion.identity);
            sparks.transform.SetParent(grappleStartPos.transform);
        }

        // stop the reel sound
        grappleReelSound.Stop();
    }

    void OnCollisionEnter(Collision collision)
    {
        // can only act on one collision event
        if (!detectCollisions) return;
        detectCollisions = false;

        // stop the reel sound
        grappleReelSound.Stop();

        // If the grapple head collides with a standard surface, create a non elastic grapple
        if (collision.gameObject.CompareTag("Normal Grap Surface"))
        {
            CreateGrapplePoint(collision, 0);
            // spawn a burst of sparks
            Instantiate(sparksBurstPrefab, transform.position, Quaternion.identity);
        }
        // If it collides with a reel, create an elastic grapple that pulls the player toward it
        else if (collision.gameObject.CompareTag("Reel"))
        {
            if (Vector3.Distance(collision.GetContact(0).point, player.transform.position) < 2f)
            {
                // if the player is too close to the bird, return the grapple
                StartCoroutine(ReturnToGun());
                return;
            }
            CreateGrapplePoint(collision, 1);
            //player cannot use the grapple until they are pulled in fully
            player.GetComponent<PlayerController>().CanUseGrapple = false;
            player.GetComponent<PlayerController>().IsStuck = false;
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
        // If it collides with a bird, create an elastic grapple that pulls the player toward it
        else if (collision.gameObject.CompareTag("Bird"))
        {
            CreateGrapplePoint(collision, 2);
            //player cannot use the grapple until they are pulled in fully
            player.GetComponent<PlayerController>().CanUseGrapple = false;
            player.GetComponent<PlayerController>().IsStuck = false;
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
        // If it collides with anything else, send the grapple back
        else
        {
            col.radius = 0.1f;
            rb.freezeRotation = true;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce((-transform.up * 4f) + (-transform.forward * 3f), ForceMode.VelocityChange);
            // spawn a burst of sparks
            Instantiate(sparksBurstPrefab, transform.position, Quaternion.identity);
        }
    }

    private void CreateGrapplePoint(Collision collision, int grappleType)
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
        player.GetComponent<GrapplePhysics>().CreateGrapple(collision.gameObject, grappleType);
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

    /// <summary>
    /// once the head is not hitting anything, turn back on its collisions
    /// </summary>
    /// <param name="hookCol"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    IEnumerator ReenableCollision(Collider hookCol, Collider col)
    {
        // Wait until hook is no longer inside
        while (col != null && hookCol.bounds.Intersects(col.bounds))
        {
            yield return null;
        }
        if (col != null) Physics.IgnoreCollision(hookCol, col, false);
    }
}
