using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class VisualEffects : MonoBehaviour
{
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Fade(float startAlpha, float endAlpha, float time)
    {
        // set the alpha instantly if time is 0
        if (time == 0f) { canvasGroup.alpha = endAlpha; }
        // otherwise, proceed as normal
        else { StartCoroutine(_Fade(startAlpha, endAlpha, time)); }
        
    }

    private IEnumerator _Fade(float startAlpha, float endAlpha, float time)
    {
        float t = 0;
        // fade to black
        while (t < time)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t / time);
            yield return null;
        }
    }
}
