using UnityEngine;

public class GrapplePhysics : MonoBehaviour
{
    ConfigurableJoint joint;
    float currentRopeLength;
    Rigidbody rb;
    [SerializeField] GameObject playerCam;
    [SerializeField] GameObject grappleHead;
    /// <summary>
    /// the force applied to the player when detatching from a bird
    /// </summary>
    [SerializeField] float launchForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// creates and sets up a configurable joint
    /// Called when the grapple head hits an object
    /// </summary>
    public void CreateGrapple(float elasticity, float damper)
    {
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
    }

    void Update()
    {

        if (joint != null)
        {
            //reel in the grapple if the player goes towards the grapple point
            float distanceToGrapple = Vector3.Distance(transform.position, joint.connectedAnchor);

            // Only shrink the rope if player is closer
            if (distanceToGrapple < currentRopeLength)
            {
                currentRopeLength = distanceToGrapple;

                SetSoftJointLimit(currentRopeLength);
            }

            joint.connectedAnchor = grappleHead.transform.position;

            // if the grapple is elastic wait until the player is close
            if (joint.xDrive.positionSpring == 0) return;
            if (distanceToGrapple <= 10f)
            {
                //detatch the grapple
                grappleHead.GetComponent<GrappleHead>().StartCoroutine(grappleHead.GetComponent<GrappleHead>().ReturnToGun());
                //reset velocity
                rb.linearVelocity = Vector3.zero;
                //apply a force to the player in the direction the player is looking
                rb.AddForce(playerCam.transform.forward * launchForce);
            }
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
