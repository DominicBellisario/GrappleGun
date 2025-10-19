using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrapplePhysics : MonoBehaviour
{
    // --- EVENTS --- 
    /// <summary>
    /// invoked whenever a grapple configurable joint is made
    /// </summary>
    public static event Action OnGrappleAttatched;
    /// <summary>
    /// invoked whenever a grapple configurable joint is destroyed
    /// </summary>
    public static event Action OnGrappleReleased;
    public static event Action OnReelStick;
    public static event Action OnBirdLaunch;
    public static event Action OnFailsafeBirdReelDetatch;

    ConfigurableJoint joint;
    float currentRopeLength;
    Rigidbody rb;
    [SerializeField] GameObject playerCam;
    [SerializeField] GameObject grappleHead;
    [Header("Grapple Settings")]
    [SerializeField] float normalElasticity = 0;
    [SerializeField] float normalDamper = 0;
    [SerializeField] float reelElasticity = 20f;
    [SerializeField] float reelDamper = 10f;

    [Header("Sounds")]
    [SerializeField] GameObject audioSourcePrefab;
    [SerializeField] AudioClip birdLaunchClip;

    GVar gvar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gvar = GVar.Instance;
    }

    void OnEnable()
    {
        GrappleHead.OnGrappleHitNormalEvent += CreateGrapple;
        GrappleHead.OnGrappleHitReelEvent += CreateGrapple;
        GrappleHead.OnGrappleHitBirdEvent += CreateGrapple;
        GrappleHead.OnStartGrappleReturnEvent += DestroyGrapple;
    }
    void OnDisable()
    {
        GrappleHead.OnGrappleHitNormalEvent -= CreateGrapple;
        GrappleHead.OnGrappleHitReelEvent -= CreateGrapple;
        GrappleHead.OnGrappleHitBirdEvent -= CreateGrapple;
        GrappleHead.OnStartGrappleReturnEvent -= DestroyGrapple;
    }

    /// <summary>
    /// creates and sets up a configurable joint
    /// Called when the grapple head hits an object
    /// </summary>
    public void CreateGrapple(Collision collision, int grappleType)
    {
        GameObject hitObject = collision.gameObject;
        float elasticity;
        float damper;

        if (grappleType == 0)
        {
            elasticity = normalElasticity;
            damper = normalDamper;
        }
        else
        {
            elasticity = reelElasticity;
            damper = reelDamper;
        }

        //get the current distance between the player and the grapple head
        //this will be the starting rope length

        currentRopeLength = Vector3.Distance(transform.position, grappleHead.transform.position);

        //create joint
        joint = gameObject.AddComponent<ConfigurableJoint>();

        // Set connectedAnchor to where the grapple hit
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grappleHead.transform.position;

        //set anchor to player position
        joint.anchor = Vector3.zero;

        // limit position movement
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SetSoftJointLimit(currentRopeLength);

        // Allow rotation (free swinging)
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        // set elasticity
        JointDrive drive = new()
        {
            positionSpring = elasticity, // How elastic it is
            positionDamper = damper, // How much it resists movement
            maximumForce = Mathf.Infinity
        };

        // Apply to axes you want to be elastic
        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;

        if (grappleType == 0)
        {
            StartCoroutine(ClampDistance());
        }
        else if (grappleType == 1)
        {
            StartCoroutine(ClampDistance());
            StartCoroutine(ReelLogic());

        }
        else
        {
            StartCoroutine(ClampDistance());
            StartCoroutine(BirdLogic(hitObject));
        }

        // change the grapple bar UI color
        OnGrappleAttatched?.Invoke();
    }

    void Update()
    {

        if (joint != null)
        {
            // get the distance between the grapple head and the player
            currentRopeLength = Vector3.Distance(transform.position, joint.connectedAnchor);

            // update the connected anchor in case the grapple point moves
            joint.connectedAnchor = grappleHead.transform.position;
        }
    }

    // reel in the grapple if the player goes towards the grapple point
    private IEnumerator ClampDistance()
    {
        while (joint != null)
        {
            SetSoftJointLimit(currentRopeLength);
            yield return null;
        }
    }

    private IEnumerator ReelLogic()
    {
        float timer = 0;
        while (joint != null)
        {
            timer += Time.deltaTime;
            // increase the elasticicty over time to bring the player in faster
            if (currentRopeLength > gvar.ReelLaunchRadius)
            {
                float newElasticity = joint.xDrive.positionSpring + Time.deltaTime * gvar.BirdElasticityIncreaseSpeed * timer;
                JointDrive drive = joint.xDrive;
                drive.positionSpring = newElasticity;
                joint.xDrive = drive;
                joint.yDrive = drive;
                joint.zDrive = drive;
            }
            else
            {
                // stick the player to the reel
                // detatch the grapple
                OnReelStick?.Invoke();
            }

            // if the player is grappling for too long, they are stuck. detatch them
            if (timer >= gvar.ReelAutoDetatchTime)
            {
                // detatch the grapple
                // reset FOV
                OnFailsafeBirdReelDetatch?.Invoke();
            }
            yield return null;
        }
    }

    private IEnumerator BirdLogic(GameObject bird)
    {
        float timer = 0;
        while (joint != null)
        {
            timer += Time.deltaTime;
            // increase the elasticicty over time to bring the player in faster
            if (currentRopeLength > gvar.ReelLaunchRadius)
            {
                float newElasticity = joint.xDrive.positionSpring + Time.deltaTime * gvar.BirdElasticityIncreaseSpeed;
                JointDrive drive = joint.xDrive;
                drive.positionSpring = newElasticity;
                joint.xDrive = drive;
                joint.yDrive = drive;
                joint.zDrive = drive;
            }
            // launch the player in the direction they are facing and kill the bird
            else
            {
                rb.linearVelocity = playerCam.transform.forward * gvar.BirdLaunchSpeed;
                bird.GetComponent<BirdEffects>().Hit(playerCam.transform);

                // play the bird launch sound at a random pitch
                GameObject newSource = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
                newSource.GetComponent<AudioSourceLogic>().Constructor(birdLaunchClip, UnityEngine.Random.Range(0.9f, 1.1f));

                // detatch the grapple
                // reset the FOV
                OnBirdLaunch?.Invoke();
            }

            // if the player is grappling for too long, they are stuck. detatch them
            if (timer >= gvar.ReelAutoDetatchTime)
            {
                //detatch the grapple
                // reset FOV
                OnFailsafeBirdReelDetatch?.Invoke();
            }
            yield return null;
        }
    }

    private void SetSoftJointLimit(float ropeLength)
    {
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = ropeLength;
        joint.linearLimit = limit;
    }

    public void DestroyGrapple()
    {
        if (joint == null) return;

        Destroy(joint);
        joint = null;
        
        // change the grapple bar UI color
        OnGrappleReleased?.Invoke();
    }
}
