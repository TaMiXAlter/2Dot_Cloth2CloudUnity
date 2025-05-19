using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowUp : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private CanvasGroup FliterWaterDrop;
    [SerializeField] private CanvasGroup FliterFog;
    [SerializeField] private CanvasGroup Text1;
    [SerializeField] private CanvasGroup Text2;
    [SerializeField] private CanvasGroup Text3;

    private CanvasGroup currentFliter;
    [Header("動畫時間設定")]
    [SerializeField] private float FliterShowTime = 0.5f;
    [SerializeField] private float Text1ShowTime = 0.5f;
    [SerializeField] private float Text2ShowTime = 0.5f;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private ApplyTextureToShader applyTextureToShader;

    private Coroutine currentCoroutine;

    public float GetFliterShowTime() => FliterShowTime;
    public float GetText1ShowTime() => Text1ShowTime;
    public float GetText2ShowTime() => Text2ShowTime;
    public float GetFadeDuration() => fadeDuration;

    public void SetFliterShowTime(float time) => FliterShowTime = time;
    public void SetText1ShowTime(float time) => Text1ShowTime = time;
    public void SetText2ShowTime(float time) => Text2ShowTime = time;
    public void SetFadeDuration(float time) => fadeDuration = time;

    private bool isFading = false;
    private bool queueFadeIn = false;
    private bool isFadingOut = false;
    private float lastTriggerTime;
    void Start()
    {
        FliterWaterDrop.alpha = 0;
        FliterWaterDrop.gameObject.SetActive(false);

        FliterFog.alpha = 0;
        FliterFog.gameObject.SetActive(false);

        Text1.alpha = 0;
        Text1.gameObject.SetActive(false);

        Text2.alpha = 0;
        Text2.gameObject.SetActive(false);

        Text3.alpha = 0;
        Text3.gameObject.SetActive(false);

        StartCoroutine(DelayedSubscribe());
    }

    public void HandleColorDetected(bool isVisible)
    {
        // 防抖動
        if (Time.time - lastTriggerTime < 0.5f) return;
        lastTriggerTime = Time.time;

        if (isFading)
            return;
        if (isFadingOut && isVisible)
        {
            if (!queueFadeIn) 
            {
                queueFadeIn = true;
            }
            return;
        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(isVisible ? FadeInSequence() : FadeOutAll());
    }
    private IEnumerator FadeInSequence()
    {
        isFading = true;

        currentFliter = GetRandomFliter();
        yield return StartCoroutine(FadeIn(currentFliter, fadeDuration, 0.3f));
        yield return new WaitForSeconds(FliterShowTime);

        yield return StartCoroutine(FadeIn(Text1, fadeDuration));
        yield return new WaitForSeconds(Text1ShowTime);

        yield return StartCoroutine(FadeIn(Text2, fadeDuration));
        yield return new WaitForSeconds(Text2ShowTime);

        yield return StartCoroutine(FadeIn(Text3, fadeDuration));

        isFading = false;
    }

    private IEnumerator FadeOutAll()
    {
         isFadingOut = true;

        if (currentFliter == null)
            currentFliter = GetRandomFliter(); 

        Coroutine fliterFade = StartCoroutine(FadeOut(currentFliter, fadeDuration));
        Coroutine text1Fade = StartCoroutine(FadeOut(Text1, fadeDuration));
        Coroutine text2Fade = StartCoroutine(FadeOut(Text2, fadeDuration));
        Coroutine text3Fade = StartCoroutine(FadeOut(Text3, fadeDuration));

        yield return fliterFade;
        yield return text1Fade;
        yield return text2Fade;
        yield return text3Fade;

        isFadingOut = false;

        if (queueFadeIn)
        {
            queueFadeIn = false;
            currentCoroutine = StartCoroutine(FadeInSequence());
        }
        else
        {
            queueFadeIn = false; 
        }
    }

    #region utils
    private IEnumerator FadeIn(CanvasGroup cg, float duration, float targetAlpha = 1f)
    {
        cg.gameObject.SetActive(true);
        cg.alpha = 0f;
        float time = 0f;
        while (time < duration)
        {
            cg.alpha = Mathf.Lerp(0f, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cg.alpha = targetAlpha;
    }

    private IEnumerator FadeOut(CanvasGroup cg, float duration)
    {
        float startAlpha = cg.alpha;
        float time = 0f;
        while (time < duration)
        {
            cg.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0f;
        cg.gameObject.SetActive(false);
    }
    private CanvasGroup GetRandomFliter()
    {
        CanvasGroup selected = Random.value > 0.5f ? FliterWaterDrop : FliterFog;

        if (selected == FliterWaterDrop) FliterFog.gameObject.SetActive(false);
        else FliterWaterDrop.gameObject.SetActive(false);

        return selected;
    }
    
    private IEnumerator DelayedSubscribe()
    {
        yield return new WaitForEndOfFrame();
        applyTextureToShader.OnColorDetected += HandleColorDetected;
    }
    #endregion
}