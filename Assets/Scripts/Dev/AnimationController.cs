using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AnimationController : MonoBehaviour
{
    [Header("參數控制目標")]
    [SerializeField] private UIShowUp uiShowUp;

    [Header("Slider 元件")]
    [SerializeField] private Slider FliterShowTimeSlider;
    [SerializeField] private Slider Text1ShowTimeSlider;
    [SerializeField] private Slider Text2ShowTimeSlider;
    [SerializeField] private Slider FadeDurationSlider;

    [Header("顯示數值用 Text")]
    [SerializeField] private TMP_Text FliterTimeText;
    [SerializeField] private TMP_Text Text1TimeText;
    [SerializeField] private TMP_Text Text2TimeText;
    [SerializeField] private TMP_Text FadeDurationText;

    void Start()
    {
        float fliterTime = uiShowUp.GetFliterShowTime();
        FliterShowTimeSlider.value = fliterTime;
        if (FliterTimeText) FliterTimeText.text = $"FliterTime: {fliterTime:F2}s";

        float text1Time = uiShowUp.GetText1ShowTime();
        Text1ShowTimeSlider.value = text1Time;
        if (Text1TimeText) Text1TimeText.text = $"Text1Time: {text1Time:F2}s";

        float text2Time = uiShowUp.GetText2ShowTime();
        Text2ShowTimeSlider.value = text2Time;
        if (Text2TimeText) Text2TimeText.text = $"Text2Time: {text2Time:F2}s";

        float fadeTime = uiShowUp.GetFadeDuration();
        FadeDurationSlider.value = fadeTime;
        if (FadeDurationText) FadeDurationText.text = $"FadeDuration: {fadeTime:F2}s";
        // 綁定 slider 事件
        FliterShowTimeSlider.onValueChanged.AddListener(value => {
            uiShowUp.SetFliterShowTime(value);
            if (FliterTimeText) FliterTimeText.text = $"FliterTime: {value:F2}s";
        });

        Text1ShowTimeSlider.onValueChanged.AddListener(value => {
            uiShowUp.SetText1ShowTime(value);
            if (Text1TimeText) Text1TimeText.text = $"Text1Time: {value:F2}s";
        });

        Text2ShowTimeSlider.onValueChanged.AddListener(value => {
            uiShowUp.SetText2ShowTime(value);
            if (Text2TimeText) Text2TimeText.text = $"Text2Time: {value:F2}s";
        });

        FadeDurationSlider.onValueChanged.AddListener(value => {
            uiShowUp.SetFadeDuration(value);
            if (FadeDurationText) FadeDurationText.text = $"FadeDuration: {value:F2}s";
        });
    }
}