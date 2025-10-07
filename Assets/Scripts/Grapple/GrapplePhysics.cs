using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrapplePhysics : MonoBehaviour
{
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
    GVar gvar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gvar = GVar.Instance;
    }

    /// <summary>
    /// creates and sets up a configurable joint
    /// Called when the grapple head hits an object
    /// </summary>
    public void CreateGrapple(int grappleType)
    {
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
        JointDrive drive = new();
        drive.positionSpring = elasticity; // How elastic it is
        drive.positionDamper = damper; // How much it resists movement
        drive.maximumForce = Mathf.Infinity;

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
            StartCoroutine(BirdLogic());
        }
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
                // stick the player to the wall
                playerCam.GetComponentInParent<PlayerController>().IsStuck = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                // detatch the grapple
                grappleHead.GetComponent<GrappleHead>().StartCoroutine(grappleHead.GetComponent<GrappleHead>().ReturnToGun());
            }

            // if the player is grappling for too long, they are stuck. detatch them
            if (timer >= gvar.ReelAutoDetatchTime)
            {
                //detatch the grapple
                grappleHead.GetComponent<GrappleHead>().StartCoroutine(grappleHead.GetComponent<GrappleHead>().ReturnToGun());
            }
            yield return null;
        }
    }

    private IEnumerator BirdLogic()
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
            else
            {
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, gvar.BirdMaxVelocity);
                grappleHead.GetComponent<GrappleHead>().StartCoroutine(grappleHead.GetComponent<GrappleHead>().ReturnToGun());
            }

            // if the player is grappling for too long, they are stuck. detatch them
            if (timer >= gvar.ReelAutoDetatchTime)
            {
                //detatch the grapple
                grappleHead.GetComponent<GrappleHead>().StartCoroutine(grappleHead.GetComponent<GrappleHead>().ReturnToGun());
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
        Destroy(joint);
        joint = null;
    }
}
