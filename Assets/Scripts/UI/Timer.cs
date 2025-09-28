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
    Coroutine timerCoroutine;

    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        timer = 0f;
    }

    public float GetTime() { return timer; }

    public void TimerSequence(bool startTimer, float startTime = 0f)
    {
        if (startTimer && timer == 0f)
        {
            timer = startTime;
            timerCoroutine = StartCoroutine(UpdateTimer());
        }
        else if (!startTimer)
        {
            StopCoroutine(timerCoroutine);
            StartCoroutine(DisplayResults());
            timer = 0f;
        }
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
        resultText.text = "Your time: " + string.Format("{0:F2}", timer);

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
