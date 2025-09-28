using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject playerCam;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gunMuzzle;

    public void FireGun(RaycastHit hit)
    {
        //spawn a new bullet
        GameObject newBullet = Instantiate(bullet, gunMuzzle.position, playerCam.transform.rotation);

        // Calculate the direction to the target
        Vector3 direction;
        Vector3 target = hit.point;
        // normally use the raycast point to calcaulte the direction
        if (target != Vector3.zero && Vector3.Distance(target, transform.position) > 1.5f)
        {
            direction = (hit.point - transform.position).normalized;
        }
        // if there is no target in range or the target is too close, use the camera forward
        else
        {
            direction = playerCam.transform.forward;
        }

        // Set the velocity
        newBullet.GetComponent<Rigidbody>().linearVelocity = direction * newBullet.GetComponent<Bullet>().BulletSpeed;

        //ignore collisions if the grapple is launched while already touching something
        Collider bulletCol = newBullet.GetComponent<Collider>();
        Collider[] overlaps = Physics.OverlapSphere(bulletCol.bounds.center, 0.1f);

        foreach (Collider col in overlaps)
        {
            // do not disable collision for self and the object the player is looking at
            if (col != bulletCol && col != hit.collider)
            {
                Physics.IgnoreCollision(bulletCol, col, true);
                newBullet.GetComponent<Bullet>().StartCoroutine(newBullet.GetComponent<Bullet>().ReenableCollision(col));
            }
        }
    }
}
