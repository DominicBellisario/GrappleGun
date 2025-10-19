using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DamageBorder : MonoBehaviour
{
    CanvasGroup canvasGroup;
    [SerializeField] float damageEffectTime;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        PlayerEvents.OnPlayerDecreaseHealth += PlayDamageEffect;
    }
    void OnDisable()
    {
        PlayerEvents.OnPlayerDecreaseHealth -= PlayDamageEffect;
    }

    private void PlayDamageEffect()
    {
        StartCoroutine(_PlayDamageEffect());
    }

    private IEnumerator _PlayDamageEffect()
    {
        float t = 0;
        canvasGroup.alpha = 1f;
        // fade to black
        while (t < damageEffectTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / damageEffectTime);
            yield return null;
        }
    }
}
