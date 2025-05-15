using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HexColorInput : MonoBehaviour
{
    public TMP_InputField hexInput;
    public Image previewImage; // 顯示顏色
    public UnityEvent<Color> onColorChanged;

    public string defaultHax;
    void Start()
    {
        hexInput.onEndEdit.AddListener(HandleColorInput);
        hexInput.text = defaultHax;
        HandleColorInput(defaultHax); // 初始化
    }

    void HandleColorInput(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            previewImage.color = color;
            onColorChanged?.Invoke(color);
        }
        else
        {
            Debug.LogWarning($"Invalid HEX: {hex}");
        }
    }
}
