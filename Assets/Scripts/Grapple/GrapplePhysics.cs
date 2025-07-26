using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GrapplePhysics : MonoBehaviour
{
    CharacterJoint joint;
    [SerializeField] GameObject grappleHead;

    /// <summary>
    /// creates and sets up a character joint.
    /// Called when the grapple head hits an object
    /// </summary>
    public void CreateGrapple()
    {
        joint = gameObject.AddComponent<CharacterJoint>();

        //change the angular limits
        SoftJointLimit swing1Limit = joint.swing1Limit;
        swing1Limit.limit = 360f;
        joint.swing1Limit = swing1Limit;
        SoftJointLimit swing2Limit = joint.swing2Limit;
        swing2Limit.limit = 360f;
        joint.swing2Limit = swing2Limit;

        // Example: Change the low and high twist limits
        SoftJointLimit lowTwistLimit = joint.lowTwistLimit;
        lowTwistLimit.limit = -20f;
        joint.lowTwistLimit = lowTwistLimit;

        SoftJointLimit highTwistLimit = joint.highTwistLimit;
        highTwistLimit.limit = 20f;
        joint.highTwistLimit = highTwistLimit;

        StartCoroutine(UpdateGrapplePosition());
    }

    private IEnumerator UpdateGrapplePosition()
    {
        while (true)
        {
            joint.anchor = grappleHead.transform.position - transform.position;
            yield return null;
        }
    }

    public void DestroyGrapple()
    {
        StopAllCoroutines();
        Destroy(joint);
        joint = null;
    }
}
