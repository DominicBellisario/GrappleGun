using UnityEngine;

public class GrappleRangeRaycast : MonoBehaviour
{
    [SerializeField] Crosshair crosshair;
    [SerializeField] GrappleHead grapple;
    float raycastRange;
    LayerMask mask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        raycastRange = grapple.maxDistance;
        mask = GetComponent<Raycasts>().targetableLayers;
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, grapple.maxDistance, mask);
        crosshair.UpdateCrosshair(hit.collider);
    }
}
