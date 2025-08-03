using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

        float startAlpha = canvasGroup.alpha;
        float endAlpha = isShow ? 1f : 0f;

        rect.anchoredPosition = startPos;

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

    public static void ScaleBounce(MonoBehaviour caller, RectTransform rect, CanvasGroup canvasGroup,
                               float startScale, float overshootScale, float endScale,
                               float upDuration = 0.25f, float downDuration = 0.2f,
                               AnimationCurve curveUp = null, AnimationCurve curveDown = null,
                               Action onComplete = null)
    {
        caller.StartCoroutine(ScaleBounceCoroutine(rect, canvasGroup, startScale, overshootScale, endScale,
                                                    upDuration, downDuration, curveUp, curveDown, onComplete));
    }

    private static IEnumerator ScaleBounceCoroutine(RectTransform rect, CanvasGroup canvasGroup,
                                                    float startScale, float overshootScale, float endScale,
                                                    float upDuration, float downDuration,
                                                    AnimationCurve curveUp, AnimationCurve curveDown,
                                                    Action onComplete)
    {
        float time = 0f;
        float startAlpha = canvasGroup.alpha;
        rect.localScale = Vector3.one * startScale;
        rect.gameObject.SetActive(true);

        // Phase 1: Scale Up to Overshoot
        while (time < upDuration)
        {
            float t = time / upDuration;
            float easedT = curveUp != null ? curveUp.Evaluate(t) : t;
            float scale = Mathf.Lerp(startScale, overshootScale, easedT);
            rect.localScale = Vector3.one * scale;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, easedT);
            time += Time.deltaTime;
            yield return null;
        }
        rect.localScale = Vector3.one * overshootScale;
        canvasGroup.alpha = 1f;

        // Phase 2: Scale Down to End Scale
        time = 0f;
        while (time < downDuration)
        {
            float t = time / downDuration;
            float easedT = curveDown != null ? curveDown.Evaluate(t) : t;
            float scale = Mathf.Lerp(overshootScale, endScale, easedT);
            rect.localScale = Vector3.one * scale;

            time += Time.deltaTime;
            yield return null;
        }
        rect.localScale = Vector3.one * endScale;
        onComplete?.Invoke();
    }

    public static void Fade(MonoBehaviour caller, Component target, float fromAlpha, float toAlpha, float duration = 0.5f, AnimationCurve curve = null, Action onComplete = null)
    {
        caller.StartCoroutine(FadeCoroutine(target, fromAlpha, toAlpha, duration, curve, onComplete));
    }

    private static IEnumerator FadeCoroutine(Component target, float fromAlpha, float toAlpha, float duration, AnimationCurve curve, Action onComplete)
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float easedT = curve != null ? curve.Evaluate(t) : t;
            float currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, easedT);

            SetAlpha(target, currentAlpha);

            time += Time.deltaTime;
            yield return null;
        }

        SetAlpha(target, toAlpha);
        onComplete?.Invoke();
    }
    private static void SetAlpha(Component target, float alpha)
    {
        if (target is CanvasGroup cg)
            cg.alpha = alpha;
        else if (target is Image img)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }
}
