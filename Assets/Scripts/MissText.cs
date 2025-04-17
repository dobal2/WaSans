using System;
using UnityEngine;
using UnityEngine.UI;

public class MissText : MonoBehaviour
{
    
    public float moveSpeed = 20f;       // 위로 올라가는 속도
    public float fadeDuration = 1.0f;   // 사라지기까지 걸리는 시간
    private float elapsedTime = 0f;     // 경과 시간
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // 위로 이동
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 알파값 줄이기
        float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        
        if (elapsedTime >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}