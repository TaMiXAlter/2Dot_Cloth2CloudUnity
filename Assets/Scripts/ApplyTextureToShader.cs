using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ApplyTextureToShader : MonoBehaviour
{
    
    [SerializeField, Range(0f, 1f)]
    private float colorSmoothingFactor = 0.95f; // Higher value for more smoothing
    
    [SerializeField, Range(0, 10)]
    private int averagingFrames = 5; // Number of frames to average for smoothing

    public event Action<bool> OnColorDetected;

    SocketReceiver socketReceiver;
    MeshRenderer targetRenderer;

    Color colorTemp;
    Color targetColor;
    bool lastColorValid = false;
    
    // Queue for storing recent colors for temporal averaging
    Queue<Color> recentColors;

    void Start()
    {
        socketReceiver = SocketReceiver.Instance;
        targetRenderer = GetComponent<MeshRenderer>();

        if (!socketReceiver || !targetRenderer)
        {
            Debug.LogError("Require socketReceiver and meshRenderer");
            Destroy(this);
        }

        colorTemp = Color.white;
        targetColor = Color.white;
        recentColors = new Queue<Color>();
        
        // Initialize queue with starting color
        for (int i = 0; i < averagingFrames; i++)
        {
            recentColors.Enqueue(Color.white);
        }
    }

    void Update()
    {
        if (!socketReceiver.outputTexture) return;

        // Get the dominant color from the camera
        Color cameraColor = GetDominantColor(socketReceiver.outputTexture);

        if (cameraColor == Color.white && colorTemp == Color.white)
            return;
            
        // Add to recent colors and remove oldest if needed
        recentColors.Enqueue(cameraColor);
        if (recentColors.Count > averagingFrames)
        {
            recentColors.Dequeue();
        }

        // Calculate average of recent colors for temporal smoothing
        targetColor = CalculateAverageColor(recentColors.ToArray());
        
        // Apply exponential smoothing for transition
        colorTemp = ExponentialSmoothing(colorTemp, targetColor, colorSmoothingFactor);
        
        // Apply final color to material
        targetRenderer.material.SetColor("_CameraColor", colorTemp);

        bool isValid = IsColorValid(cameraColor);
        if (isValid != lastColorValid) 
        {
            OnColorDetected?.Invoke(isValid);
            lastColorValid = isValid;
        }
    }
    
    // Exponential smoothing formula - smoother than linear interpolation
    Color ExponentialSmoothing(Color currentValue, Color targetValue, float alpha)
    {
        // Alpha closer to 1 means more weight on the previous value (smoother)
        return Color.Lerp(targetValue, currentValue, alpha);
    }
    
    // Calculate average of an array of colors
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

    Color GetAverageColor(Texture2D tex)
    {
        Color[] pixels = tex.GetPixels();
        float r = 0f, g = 0f, b = 0f;
        int count = 0;

        foreach (Color pixel in pixels)
        {
            if (pixel.r + pixel.g + pixel.b > 0.2f)
            {
                r += pixel.r;
                g += pixel.g;
                b += pixel.b;
                count++;
            }
        }

        if (count == 0) return Color.white;
        return new Color(r / count, g / count, b / count);
    }

    Color GetDominantColor(Texture2D tex)
    {
        Color[] pixels = tex.GetPixels();
        Dictionary<Color, int> colorCount = new Dictionary<Color, int>();
        float threshold = 0.2f; // 排除太黑的像素
        int quantizeLevel = 16; // 顏色量化等級，調整以平衡準確度與效率

        foreach (Color pixel in pixels)
        {
            // 排除接近黑色的像素
            if (pixel.r + pixel.g + pixel.b < threshold) continue;

            // 量化顏色
            Color key = new Color(
                Mathf.Floor(pixel.r * quantizeLevel) / quantizeLevel,
                Mathf.Floor(pixel.g * quantizeLevel) / quantizeLevel,
                Mathf.Floor(pixel.b * quantizeLevel) / quantizeLevel
            );

            if (colorCount.ContainsKey(key))
                colorCount[key]++;
            else
                colorCount[key] = 1;
        }

        if (colorCount.Count == 0) return Color.white;

        // 找出出現次數最多的顏色
        return colorCount.OrderByDescending(kv => kv.Value).First().Key;
    }

    bool IsColorValid(Color color)
    {
        return color != Color.white;
    }
}