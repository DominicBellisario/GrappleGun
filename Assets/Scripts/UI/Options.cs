using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] Slider senseSlider;
    [SerializeField] Slider volumeSlider;

    GVar gvar;

    void Start()
    {
        gvar = GVar.Instance;
    }

    // match the options stuff with the actual values
    public void AssignValues()
    {
        senseSlider.value = gvar.MouseSensitivity;
        volumeSlider.value = gvar.MasterVolume;
    }

    // apply the options stuff to the actual values
    public void ApplyValues()
    {
        gvar.MouseSensitivity = senseSlider.value;
        gvar.MasterVolume = volumeSlider.value;
    }
}
