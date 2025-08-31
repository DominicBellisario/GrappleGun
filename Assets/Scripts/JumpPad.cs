using System.Collections;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float launchForce;
    [SerializeField] float coolDownTime;
    [SerializeField] BoxCollider jumpPadCollider;

    [Header("Gizmo Settings")]
    // how many segments to draw
    [SerializeField] int resolution = 30;
    // time between points      
    [SerializeField] float timeStep = 0.1f;    

    public Vector3 GetLaunchForceAndActivatePad()
    {
        StartCoroutine(DisableJumpPad());
        return transform.up * launchForce;
    }

    private IEnumerator DisableJumpPad()
    {
        jumpPadCollider.enabled = false;
        yield return new WaitForSeconds(coolDownTime);
        jumpPadCollider.enabled = true;
    }

    void OnDrawGizmos()
    {
        //uses kenumatic equasions to predict the player's trajectory
        Gizmos.color = Color.yellow;

        Vector3 startPos = transform.position;
        Vector3 velocity = transform.up * launchForce;

        Vector3 previousPoint = startPos;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i * timeStep;
            Vector3 newPoint = startPos + velocity * t + 0.5f * Physics.gravity * (t * t);

            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }
}
