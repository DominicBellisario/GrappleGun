using UnityEngine;

public class FalseWall : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Bullet")) return;

        Destroy(gameObject);
    }
}
