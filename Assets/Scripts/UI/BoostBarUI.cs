using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BoostBarUI : MonoBehaviour
{
    Slider slider;
    [SerializeField] Image bar;

    [SerializeField] Color noBoostColor;
    [SerializeField] Color boostColor;
    
    GVar gvar;

    void Start()
    {
        slider = GetComponent<Slider>();
        gvar = GVar.Instance;
    }

    void OnEnable()
    {
        PlayerController.OnBoostStartEvent += HandleOnBoostStart;
        PlayerController.OnBoostStopEvent += HandleOnBoostStop;
    }
    void OnDisable()
    {
        PlayerController.OnBoostStartEvent -= HandleOnBoostStart;
        PlayerController.OnBoostStopEvent -= HandleOnBoostStop;
    }
    void HandleOnBoostStart() { bar.color = boostColor; }
    void HandleOnBoostStop() { bar.color = noBoostColor; }

    void Update()
    {
        // update the length of the bar to match the current boost fuel
        slider.value = gvar.CurrentBoostFuel;
    }
}
