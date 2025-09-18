using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject bulletExplosion;

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

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(bulletExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
