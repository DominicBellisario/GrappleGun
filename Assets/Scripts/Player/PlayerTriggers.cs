using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerTriggers : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Jump Pad"))
        {
            //reset velocity and launch in the direction the pad is facing
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(trigger.gameObject.GetComponent<JumpPad>().GetLaunchForceAndActivatePad(), ForceMode.VelocityChange);
            StartCoroutine(Test());
        }
    } 

    IEnumerator Test()
    {
        yield return new WaitForSeconds(0.05f);
        Debug.Log(rb.linearVelocity);
    }
}
