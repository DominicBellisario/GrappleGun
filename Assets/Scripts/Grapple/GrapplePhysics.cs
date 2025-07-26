using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GrapplePhysics : MonoBehaviour
{
    ConfigurableJoint joint;
    [SerializeField] GameObject grappleHead;
    private float currentRopeLength;

    /// <summary>
    /// creates and sets up a configurable joint
    /// Called when the grapple head hits an object
    /// </summary>
    public void CreateGrapple()
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

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = currentRopeLength;
        joint.linearLimit = limit;

        // Allow rotation (free swinging)
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;
    }

    void Update()
    {
        //reel in the grapple if the player goes towards the grapple point
        if (joint != null)
        {
            float distanceToGrapple = Vector3.Distance(transform.position, joint.connectedAnchor);

            // Only shrink the rope if player is closer
            if (distanceToGrapple < currentRopeLength)
            {
                currentRopeLength = distanceToGrapple;

                SoftJointLimit limit = new SoftJointLimit();
                limit.limit = currentRopeLength;
                joint.linearLimit = limit;
            }

            joint.connectedAnchor = grappleHead.transform.position;
        }
    }

    public void DestroyGrapple()
    {
        Destroy(joint);
        joint = null;
    }
}
