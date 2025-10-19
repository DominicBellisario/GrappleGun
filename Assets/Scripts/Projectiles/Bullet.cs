using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject bulletExplosion;
    
    [SerializeField] float lifeSpan;

    [SerializeField] float bulletSpeed;
    public float BulletSpeed { get { return bulletSpeed; } }

    [SerializeField] int damage;
    public int Damage { get { return damage; } }

    bool hasCollided;

    [SerializeField] AudioClip bulletExplosionClip;

    GVar gvar;

   

    void Start()
    {
        gvar = GVar.Instance;
        hasCollided = false;
        StartCoroutine(Helper.DoThisAfterDelay(lifeSpan, () => Die()));
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
        while (otherCol != null && bulletCol.bounds.Intersects(otherCol.bounds))
        {
            yield return null;
        }
        if (otherCol != null) Physics.IgnoreCollision(bulletCol, otherCol, false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;
        Die();
    }

    private void Die()
    {
        // spawn an explosion
        Instantiate(bulletExplosion, transform.position, Quaternion.identity);

        // play the bullet explode sound at a random pitch
        GameObject newSource = Instantiate(gvar.AudioSourcePrefab, transform.position, Quaternion.identity);
        newSource.GetComponent<AudioSourceLogic>().Constructor(bulletExplosionClip, Random.Range(0.9f, 1.1f));

        Destroy(gameObject);
    }
}
