using UnityEngine;

public class DashAfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color color;
    private float alpha = 0.5f;
    public float alphaDecay = 3f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
    }

    private void Update()
    {
        alpha -= alphaDecay * Time.deltaTime;
        color.a = alpha;
        sr.color = color;

        if (alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(Sprite sprite, Vector3 position, Vector3 scale, bool flipX)
    {
        sr.sprite = sprite;
        transform.position = position;
        transform.localScale = scale;
        sr.flipX = flipX;
    }
}