using UnityEngine;

public class GrappleRangeRaycast : MonoBehaviour
{
    [SerializeField] Crosshair crosshair;
    LayerMask mask;
    static float raycastRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        raycastRange = GVar.Instance.GrappleMaxDistance;
        mask = GetComponent<Raycasts>().targetableLayers;
    }

    // Update is called once per frame
    void Update()
    {
        //shoot a raycast that shows the max range of the grapple
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, raycastRange, mask);
        crosshair.UpdateCrosshair(hit.collider);
    }
}
