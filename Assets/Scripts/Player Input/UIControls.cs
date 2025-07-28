using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControls : MonoBehaviour
{
    TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void OnExpandControls(InputValue inputValue)
    {
        if (!inputValue.isPressed) return;
        text.enabled = !text.enabled;
    }
}
