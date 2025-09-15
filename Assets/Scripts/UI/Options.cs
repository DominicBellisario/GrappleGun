using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] Slider senseSlider;

    GVar gvar;

    void Start()
    {
        gvar = GVar.Instance;
    }

    // match the options stuff with the actual values
    public void AssignValues()
    {
        senseSlider.value = gvar.MouseSensitivity;
    }

    // apply the options stuff to the actual values
    public void ApplyValues()
    {
        gvar.MouseSensitivity = senseSlider.value;
    }
}
