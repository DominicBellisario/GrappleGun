using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class VelocityText : MonoBehaviour
{
    TextMeshProUGUI text;
    GVar gvar;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        gvar = GVar.Instance;
    }

    void Update()
    {
        //Display the player's velocity to one decimal point
        text.text = "Speed: " + Vector3.Magnitude(gvar.PlayerRb.linearVelocity).ToString("F1") + " m/s";
    }
}
