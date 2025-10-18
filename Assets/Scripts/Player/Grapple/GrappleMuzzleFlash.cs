using UnityEngine;

public class GrappleMuzzleFlash : MonoBehaviour
{
    [SerializeField] GameObject muzzleFlashPrefab;

    void OnEnable()
    {
        PlayerController.OnShootGrappleEvent += SpawnFlash;
    }

    void OnDisable()
    {
        PlayerController.OnShootGrappleEvent -= SpawnFlash;
    }

    void SpawnFlash(RaycastHit unusedHit = default)
    {
        Instantiate(muzzleFlashPrefab, transform.position, Quaternion.identity).transform.SetParent(transform);
    }
}
