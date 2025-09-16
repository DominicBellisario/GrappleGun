using UnityEngine;
using UnityEngine.UI;

public class BoostBarUI : MonoBehaviour
{
    Slider slider;
    [SerializeField] Image bar;
    [SerializeField] PlayerController player;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        // update the length of the bar to match the current boost fuel
        slider.value = player.CurrentBoostFuel;
    }
}
