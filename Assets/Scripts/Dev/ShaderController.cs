using UnityEngine;
using UnityEngine.UI;

public class ShaderController : MonoBehaviour
{
    public Slider speedSlider;

    public HexColorInput color1Input;
    public HexColorInput color2Input;
    public HexColorInput color3Input;

    public MeshRenderer Target;
    private Material targetMaterial;

    void Start()
    {
        targetMaterial = Target.material;
        color1Input.onColorChanged.AddListener(color => targetMaterial.SetColor("_Color1", color));
        color2Input.onColorChanged.AddListener(color => targetMaterial.SetColor("_Color2", color));
        color3Input.onColorChanged.AddListener(color => targetMaterial.SetColor("_Color3", color));
    }

    void Update()
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat("_Speed", speedSlider.value);
        }
    }
}