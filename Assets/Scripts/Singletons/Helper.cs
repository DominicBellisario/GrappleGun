using System;
using System.Collections;
using UnityEngine;

public static class Helper
{
    public static IEnumerator DoThisAfterDelay(float delay, Action onReset)
    {
        yield return new WaitForSeconds(delay);
        onReset?.Invoke();
    }
}
