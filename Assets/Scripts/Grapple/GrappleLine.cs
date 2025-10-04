using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrappleLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject grappleMuzzle;
    [SerializeField] GameObject grappleHead;

    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }
    void Update()
    {
        Vector3 start = grappleMuzzle.transform.position;
        Vector3 end = grappleHead.transform.position;

        line.SetPosition(0, grappleMuzzle.transform.position);
        line.SetPosition(1, grappleHead.transform.position);

        float distance = Vector3.Distance(start, end);
        line.material.mainTextureOffset = new Vector2(-distance * 2.0f, 0);
    }
}
