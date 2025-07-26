using System.Collections;
using UnityEngine;

public class GrappleLine : MonoBehaviour
{
    [SerializeField] GameObject grappleMuzzle;
    [SerializeField] GameObject grappleHead;

    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        line.SetPosition(0, grappleMuzzle.transform.position);
        line.SetPosition(1, grappleHead.transform.position);
    } 
}
