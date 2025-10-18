using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] float resultTextActiveTime;
    [SerializeField] float resultTextFadeOutTime;
    TextMeshProUGUI timerText;
    float timer;
    GVar gvar;
    Coroutine timerCoroutine;

    void Start()
    {
        gvar = GVar.Instance;
        timerText = GetComponent<TextMeshProUGUI>();
        timer = 0f;
    }

    void OnEnable()
    {
        PlayerEvents.OnPlayerOutOfBounds += HandlePlayerOutOfBounds;
        PlayerTriggers.OnPlayerReachedTarget += HandlePlayerReachingTarget;
    }
    void OnDisable()
    {
        PlayerEvents.OnPlayerOutOfBounds -= HandlePlayerOutOfBounds;
        PlayerTriggers.OnPlayerReachedTarget -= HandlePlayerReachingTarget;
    }

    private void HandlePlayerOutOfBounds() { gvar.LastRecordedTime = timer; }
    private void HandlePlayerReachingTarget()
    {
        gvar.LastRecordedTime = timer;
        TimerEnd();
    }

    public void TimerStart(float startTime = 0f)
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timer = startTime;
        timerCoroutine = StartCoroutine(UpdateTimer());
    }

    public void TimerEnd()
    {
        StartCoroutine(DisplayResults());
        timer = 0f;
    }

    private IEnumerator UpdateTimer()
    {
        while (true)
        {
            timer += Time.deltaTime;
            timerText.text = "Time: " + string.Format("{0:F2}", timer);
            yield return null;
        }
    }

    private IEnumerator DisplayResults()
    {
        // set the text and make it visible
        resultText.enabled = true;
        resultText.text = "Your time: " + string.Format("{0:F2}", gvar.LastRecordedTime);

        // wait for a bit
        yield return new WaitForSeconds(resultTextActiveTime);

        // fade the text
        float fadeOutTimer = 0f;
        while (fadeOutTimer < resultTextFadeOutTime)
        {

            fadeOutTimer += Time.deltaTime;
            resultText.alpha = Mathf.Lerp(1, 0, fadeOutTimer / resultTextFadeOutTime);
            yield return null;
        }
        resultText.enabled = false;
        resultText.alpha = 1f;
    }
}
