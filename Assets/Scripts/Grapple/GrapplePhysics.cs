using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GrapplePhysics : MonoBehaviour
{
    ConfigurableJoint joint;
    [SerializeField] GameObject grappleHead;

    /// <summary>
    /// creates and sets up a character joint.
    /// Called when the grapple head hits an object
    /// </summary>
    public void CreateGrapple()
    {
        //create joint
        joint = gameObject.AddComponent<ConfigurableJoint>();

        // Set connectedAnchor to where the grapple hit
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grappleHead.transform.position;

        //set anchor to player position
        joint.anchor = Vector3.zero;

        // Calculate current rope length (distance from player to grapple point)
        float currentDistance = Vector3.Distance(transform.position, grappleHead.transform.position);

        // limit position movement
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        // Set linear limit (the max rope length)
        SoftJointLimit limit = new();
        limit.limit = currentDistance;
        joint.linearLimit = limit;


        // Allow rotation (free swinging)
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        joint.connectedAnchor = grappleHead.transform.position;

        StartCoroutine(UpdateGrapplePosition());
    }

    private IEnumerator UpdateGrapplePosition()
    {
        yield return null;
    }

    public void DestroyGrapple()
    {
        StopAllCoroutines();
        Destroy(joint);
        joint = null;
    }
}
