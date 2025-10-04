using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrappleLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject grappleMuzzle;
    [SerializeField] GameObject grappleHead;

    LineRenderer line;
    GrappleHead grappleHeadScript;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        grappleHeadScript = grappleHead.GetComponent<GrappleHead>();
    }
    void Update()
    {
        // get positions
        Vector3 start = grappleMuzzle.transform.position;
        Vector3 end = grappleHead.transform.position;

        // set the line points to the positions
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        // scroll the texture based on distance to the PLAYER (not grapple head bc the joint is on player and it would scroll when it wasnt supposed to) (im a genius)
        line.material.mainTextureOffset = new Vector2(-grappleHeadScript.CurrentRopeLength * 2.0f, 0);
    }
}
