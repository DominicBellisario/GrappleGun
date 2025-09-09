using UnityEngine;

public class GrappleRangeRaycast : MonoBehaviour
{
    [SerializeField] Crosshair crosshair;
    static float raycastRange;
    LayerMask mask;
    GVar gvar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvar = GVar.Instance;
        raycastRange = gvar.GrappleMaxDistance;
        mask = GetComponent<Raycasts>().targetableLayers;
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, raycastRange, mask);
        crosshair.UpdateCrosshair(hit.collider);
    }
}
