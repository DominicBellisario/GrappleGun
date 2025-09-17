using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// once the bullet is not hitting anything, turn back on its collisions
    /// </summary>
    /// <param name="hookCol"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public IEnumerator ReenableCollision(Collider otherCol)
    {
        Collider bulletCol = GetComponent<Collider>();
        // Wait until hook is no longer inside
        while (bulletCol.bounds.Intersects(otherCol.bounds))
        {
            yield return null;
        }
        Physics.IgnoreCollision(bulletCol, otherCol, false);
    }
}
