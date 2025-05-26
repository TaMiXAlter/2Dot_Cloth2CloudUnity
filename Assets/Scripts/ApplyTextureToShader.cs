// ApplyTextureToShader.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ApplyTextureToShader : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float colorSmoothingFactor = 0.95f; 
    
    [SerializeField, Range(0, 10)]
    private int averagingFrames = 5; 


    public event Action<bool> OnColorDetected;

    SocketReceiver socketReceiver;
    MeshRenderer targetRenderer;

    Color colorTemp;
    Color targetColor;
    bool lastColorValid = false;
    
    Queue<Color> recentColors; 
    private float lastEventTriggerTime = 0f; 
    void Start()
    {
        socketReceiver = SocketReceiver.Instance;
        targetRenderer = GetComponent<MeshRenderer>();

        if (!socketReceiver || !targetRenderer)
        {
            Debug.LogError("Require socketReceiver and meshRenderer");
            Destroy(this);
            return;
        }

        colorTemp = Color.white; 
        targetColor = Color.white; 
        recentColors = new Queue<Color>(); 
        
        for (int i = 0; i < averagingFrames; i++)
        {
            recentColors.Enqueue(Color.white); 
        }
    }

    void Update()
    {
        if (!socketReceiver.outputTexture) return;

        Color cameraColor = GetDominantColor(socketReceiver.outputTexture); 

        if (cameraColor == Color.white && colorTemp == Color.white && !lastColorValid) 
            return;
            
        recentColors.Enqueue(cameraColor); 
        if (recentColors.Count > averagingFrames)
        {
            recentColors.Dequeue(); 
        }

        targetColor = CalculateAverageColor(recentColors.ToArray()); 
        colorTemp = ExponentialSmoothing(colorTemp, targetColor, colorSmoothingFactor); 
        targetRenderer.material.SetColor("_CameraColor", colorTemp); 

        bool isValid = IsColorValid(cameraColor); 
       
        if (isValid != lastColorValid && Time.time - lastEventTriggerTime > 0.5f) 
        {
            OnColorDetected?.Invoke(isValid);
            lastColorValid = isValid; 
            lastEventTriggerTime = Time.time; 
        }
    }
    
    Color ExponentialSmoothing(Color currentValue, Color targetValue, float alpha)
    {
        return Color.Lerp(targetValue, currentValue, alpha); 
    }
    
    Color CalculateAverageColor(Color[] colors)
    {
        if (colors.Length == 0) return Color.white;
        
        float r = 0, g = 0, b = 0;
        foreach (Color color in colors)
        {
            r += color.r;
            g += color.g;
            b += color.b;
        }
        return new Color(r / colors.Length, g / colors.Length, b / colors.Length); 
    }

    Color GetDominantColor(Texture2D tex)
    {
        Color[] pixels = tex.GetPixels(); 
        Dictionary<Color, int> colorCount = new Dictionary<Color, int>(); 
        float threshold = 0.2f; 
        int quantizeLevel = 16; 

        foreach (Color pixel in pixels)
        {
            if (pixel.r + pixel.g + pixel.b < threshold) continue; 

            Color key = new Color(
                Mathf.Floor(pixel.r * quantizeLevel) / quantizeLevel,
                Mathf.Floor(pixel.g * quantizeLevel) / quantizeLevel,
                Mathf.Floor(pixel.b * quantizeLevel) / quantizeLevel
            ); //

            if (colorCount.ContainsKey(key))
                colorCount[key]++; 
            else
                colorCount[key] = 1; 
        }

        if (colorCount.Count == 0) return Color.white; 
        return colorCount.OrderByDescending(kv => kv.Value).First().Key; 
    }

    private bool IsColorValid(Color color)
    {
        return color != Color.white;
    }
}