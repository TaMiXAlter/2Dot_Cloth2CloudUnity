using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
[System.Serializable]
public class Config
{
    public List<string> imagePaths = new List<string>();
    public string color1;
    public string color2;
    public string color3;
    public float speed;

    public float fliterShowTime;
    public float text1ShowTime;
    public float text2ShowTime;
    public float fadeDuration;
}
public class SettingDataManager : MonoBehaviour
{
    public ShaderController shaderController;
    public List<SingleImageLoader> imageLoaders;
    public UIShowUp uiShowUp;
    private string configPath;

    void Start()
    {
        configPath = Path.Combine(Application.persistentDataPath, "config.json");
        LoadConfig();
    }

    void OnApplicationQuit()
    {
        SaveConfig();
    }

    public void SaveConfig()
    {
        Config config = new Config
        {
            speed = shaderController.speedSlider.value,
            color1 = ColorUtility.ToHtmlStringRGB(shaderController.color1Input.previewImage.color),
            color2 = ColorUtility.ToHtmlStringRGB(shaderController.color2Input.previewImage.color),
            color3 = ColorUtility.ToHtmlStringRGB(shaderController.color3Input.previewImage.color),
            fliterShowTime = uiShowUp.GetFliterShowTime(),
            text1ShowTime = uiShowUp.GetText1ShowTime(),
            text2ShowTime = uiShowUp.GetText2ShowTime(),
            fadeDuration = uiShowUp.GetFadeDuration()
        };

        // 儲存每個 loader 的圖片路徑
        foreach (var loader in imageLoaders)
        {
            config.imagePaths.Add(loader.GetImagePath());
        }

        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(configPath, json);
        Debug.Log("✅ 設定已儲存！");
    }

    public void LoadConfig()
    {
        if (!File.Exists(configPath)) return;

        string json = File.ReadAllText(configPath);
        Config config = JsonUtility.FromJson<Config>(json);

        // 載入圖片
        for (int i = 0; i < config.imagePaths.Count && i < imageLoaders.Count; i++)
        {
            string path = config.imagePaths[i];
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                imageLoaders[i].LoadImage(path);
            }
        }

        // 載入顏色
        Color c;
        if (ColorUtility.TryParseHtmlString("#" + config.color1, out c)) shaderController.color1Input.previewImage.color = c;
        if (ColorUtility.TryParseHtmlString("#" + config.color2, out c)) shaderController.color2Input.previewImage.color = c;
        if (ColorUtility.TryParseHtmlString("#" + config.color3, out c)) shaderController.color3Input.previewImage.color = c;

        // 載入速度
        shaderController.speedSlider.value = config.speed;

        // 載入 UI 淡入動畫設定
        uiShowUp.SetFliterShowTime(config.fliterShowTime);
        uiShowUp.SetText1ShowTime(config.text1ShowTime);
        uiShowUp.SetText2ShowTime(config.text2ShowTime);
        uiShowUp.SetFadeDuration(config.fadeDuration);
    }
}