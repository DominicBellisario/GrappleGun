using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GrappleHead : MonoBehaviour
{
    Rigidbody rb;

    /// <summary>
    /// The speed at which the grapple head moves.
    /// </summary>
    [SerializeField] float speed;

    public void Launch(Vector3 target)
    {
        StartCoroutine(Die());
        rb = GetComponent<Rigidbody>();
        // Calculate the direction to the target
        Vector3 direction = (target - transform.position).normalized;
        // Set the velocity
        rb.linearVelocity = direction * speed;
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
