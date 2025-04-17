using UnityEngine;

public class RainbowColorChanger : MonoBehaviour
{
    public Material targetMaterial; // 베이스 컬러를 바꿀 머티리얼
    public float colorChangeSpeed = 1f; // 색 변화 속도

    private void Update()
    {
        // Hue 값을 시간에 따라 순환
        float hue = Mathf.Repeat(Time.time * colorChangeSpeed, 1f);

        // HSV -> RGB 변환 (채도, 명도는 최대)
        Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);

        targetMaterial.SetColor("_BaseColor", rainbowColor);
    }
}