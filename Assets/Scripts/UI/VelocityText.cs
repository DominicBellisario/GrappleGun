using TMPro;
using UnityEngine;

public class VelocityText : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] Rigidbody playerRb;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //Display the player's velocity to one decimal point
        text.text = "Speed: " + Vector3.Magnitude(playerRb.linearVelocity).ToString("F1") + " m/s";
    }
}
