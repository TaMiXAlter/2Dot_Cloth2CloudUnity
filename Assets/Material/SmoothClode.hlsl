#define S(a, b, t) smoothstep(a, b, t)

void SmoothCloud_float(float2 UV, float Time, float2 Resolution,
float3 CameraColor, float3 Color1,float3 Color2,float3 Color3,
out float3 Color)
{
    // Main shader code
    float2 uv = UV;
    float ratio = Resolution.x / Resolution.y;
    float2 tuv = uv;
    tuv -= 0.5;
    // limit time
    float maxTime = 2000;
    float limitedTime = maxTime - abs(fmod(Time, 2.0 * maxTime) - maxTime);
    // Noise calculation (simplified to avoid nested functions)
    float2 noiseP = float2(limitedTime * 0.1, tuv.x * tuv.y);
    float2 i = floor(noiseP);
    float2 f = frac(noiseP);
    float2 u = f * f * (3.0 - 2.0 * f);
    
    // Calculate hash values for the 4 corners
    float2 hash00 = frac(sin(float2(dot(i + float2(0.0, 0.0), float2(2127.1, 81.17)), 
                         dot(i + float2(0.0, 0.0), float2(1269.5, 283.37)))) * 43758.5453);
    float2 hash10 = frac(sin(float2(dot(i + float2(1.0, 0.0), float2(2127.1, 81.17)), 
                         dot(i + float2(1.0, 0.0), float2(1269.5, 283.37)))) * 43758.5453);
    float2 hash01 = frac(sin(float2(dot(i + float2(0.0, 1.0), float2(2127.1, 81.17)), 
                         dot(i + float2(0.0, 1.0), float2(1269.5, 283.37)))) * 43758.5453);
    float2 hash11 = frac(sin(float2(dot(i + float2(1.0, 1.0), float2(2127.1, 81.17)), 
                         dot(i + float2(1.0, 1.0), float2(1269.5, 283.37)))) * 43758.5453);
    
    // Calculate noise value
    float n = lerp(
        lerp(dot(-1.0 + 2.0 * hash00, f - float2(0.0, 0.0)),
             dot(-1.0 + 2.0 * hash10, f - float2(1.0, 0.0)), u.x),
        lerp(dot(-1.0 + 2.0 * hash01, f - float2(0.0, 1.0)),
             dot(-1.0 + 2.0 * hash11, f - float2(1.0, 1.0)), u.x), u.y);
    
    float degree = 0.5 + 0.5 * n;
    
    // Apply aspect ratio and rotation
    tuv.y *= 1.0 / ratio;
    
    // Create rotation matrix
    float rotAngle = radians((degree - 0.5) * 720.0 + 180.0);
    float sinVal, cosVal;
    sincos(rotAngle, sinVal, cosVal);
    float2x2 rotMat = float2x2(cosVal, -sinVal, sinVal, cosVal);
    
    // Apply rotation
    tuv = mul(rotMat, tuv);
    tuv.y *= ratio;
    
    // Wave warp with sin
    float frequency = 5.0;
    float amplitude = 30.0;
    float speed = limitedTime * 2.0;
    tuv.x += sin(tuv.y * frequency + speed) / amplitude;
    tuv.y += sin(tuv.x * frequency * 1.5 + speed) / (amplitude * 0.5);
    
    // Create layer rotation matrix
    float layerAngle = radians(-5.0);
    float layerSin, layerCos;
    sincos(layerAngle, layerSin, layerCos);
    float2x2 layerRotMat = float2x2(layerCos, -layerSin, layerSin, layerCos);
    
    // Apply layer rotation for color mixing
    float2 rotTuv = mul(layerRotMat, tuv);
    
    // Draw the image

    float3 layer1 = lerp(Color1,Color2, S(-0.3, 0.2, rotTuv.x));
    
    float3 colorBlue = float3(0.350, 0.71, 0.953);
    float3 layer2 = lerp(CameraColor, Color3, S(-0.3, 0.2, rotTuv.x));
    
    float3 finalComp = lerp(layer1, layer2, S(0.5, -0.3, tuv.y));
    
    Color = finalComp;
}