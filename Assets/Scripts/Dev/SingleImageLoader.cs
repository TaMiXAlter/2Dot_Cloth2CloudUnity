using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SFB; // Standalone File Browser 命名空間

public class SingleImageLoader : MonoBehaviour
{
    public RawImage targetImage;
    private string loadedImagePath;

    public void LoadImageFromFile()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("選擇圖片", "", "png", false);

        if (paths.Length > 0 && File.Exists(paths[0]))
        {
            LoadImage(paths[0]);
        }
    }

    public void LoadImage(string path)
    {
        byte[] imageBytes = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        if (tex.LoadImage(imageBytes))
        {
            targetImage.texture = tex;
            targetImage.color = Color.white;
            loadedImagePath = path;
            Debug.Log($"載入圖片：{path}");
        }
        else
        {
            Debug.LogWarning("圖片載入失敗！");
        }
    }

    public string GetImagePath()
    {
        return loadedImagePath;
    }
}