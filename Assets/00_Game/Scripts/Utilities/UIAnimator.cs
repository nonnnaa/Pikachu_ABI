using System;
using System.Collections;
using UnityEngine;

public static class UIAnimator
{
    public enum SlideDirection { Up, Down }

    public static void SlideAndFade(MonoBehaviour caller, RectTransform rect, CanvasGroup canvasGroup,
        bool isShow, SlideDirection direction, float distance = 800f,
        float duration = 0.4f, Action onComplete = null)
    {
        caller.StartCoroutine(SlideCoroutine(rect, canvasGroup, isShow, direction, distance, duration, onComplete));
    }

    private static IEnumerator SlideCoroutine(RectTransform rect, CanvasGroup canvasGroup,
        bool isShow, SlideDirection direction,
        float distance, float duration, Action onComplete)
    {
        float time = 0f;

        float sign = direction == SlideDirection.Down ? 1 : -1;
        Vector2 offset = new Vector2(0, distance * sign);
        Vector2 originalPos = Vector2.zero;

        Vector2 startPos = isShow ? originalPos + offset : originalPos;
        Vector2 endPos = isShow ? originalPos : originalPos + offset;

        float startAlpha = isShow ? 0f : 1f;
        float endAlpha = isShow ? 1f : 0f;

        rect.anchoredPosition = startPos;
        canvasGroup.alpha = startAlpha;

        if (isShow)
            rect.gameObject.SetActive(true);

        while (time < duration)
        {
            float t = time / duration;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            time += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = endPos;
        canvasGroup.alpha = endAlpha;

        if (!isShow)
            rect.gameObject.SetActive(false);

        onComplete?.Invoke();
    }
}
