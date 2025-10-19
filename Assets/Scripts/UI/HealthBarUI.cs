using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBarUI : MonoBehaviour
{
    Slider slider;
    GVar gvar;

    [Header("References")]
    [SerializeField] Image bar;
    [SerializeField] Image background;

    [Header("Visuals")]
    [SerializeField] float smoothSpeed;

    // the current player health
    float targetValue;


    void Start()
    {
        slider = GetComponent<Slider>();
        gvar = GVar.Instance;
        slider.maxValue = gvar.CurrentHealth * 100;
        targetValue = slider.maxValue;
    }

    void Update()
    {
        // lerp the bar to follow the current health
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);

        //update health if it is different
        if (targetValue != gvar.CurrentHealth * 100)
        {
            targetValue = gvar.CurrentHealth * 100;
        }

        // hide the bar if at full health
        if (targetValue == slider.maxValue)
        {
            bar.enabled = false;
            background.enabled = false;
        }
        else
        {
            bar.enabled = true;
            background.enabled = true;
        }
    }
}
