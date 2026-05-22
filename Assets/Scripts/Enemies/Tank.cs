using System;
using System.Collections;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public float maxHp;
    public float hp;
    public GameObject bulletPrefab;
    public Transform bulletTransform;
    public float shootCoolDown = 2f;
    private float shootTimer = 0f;

    public Transform playerTransform;
    public float launchAngle = 70f;
    public GameObject explosion;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float retreatDistance = 5f;

    [Header("Wheels")]
    public Transform wheelL;
    public Transform wheelR;
    public float wheelRotateSpeed = 200f;

    [Header("Hit Flash")]
    public float flashDuration = 0.05f;
    public int flashCount = 4;

    private bool facingRight = false;
    private Rigidbody2D rb;
    private SpriteRenderer[] spriteRenderers;
    private Material[] originalMaterials;
    private Material[] flashMaterials;
    private bool isInvincible = false;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        Shader flashShader = Shader.Find("Custom/FlashWhite");
        originalMaterials = new Material[spriteRenderers.Length];
        flashMaterials = new Material[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalMaterials[i] = spriteRenderers[i].material;

            flashMaterials[i] = new Material(originalMaterials[i]);
            flashMaterials[i].shader = flashShader;
        }
    }

    void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootCoolDown)
        {
            shootTimer = 0;
            FireProjectile();
        }

        float distX = playerTransform.position.x - transform.position.x;
        float absDistX = Mathf.Abs(distX);

        float moveDir = 0f;
        if (absDistX < retreatDistance)
            moveDir = -Mathf.Sign(distX);

        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

        // 바퀴 회전 — 플립 시 localScale.x가 반전되므로 나눠서 보정
        if (Mathf.Abs(rb.linearVelocity.x) > 0.01f && wheelL != null && wheelR != null)
        {
            float worldDir = Mathf.Sign(rb.linearVelocity.x);
            float scale = Mathf.Sign(transform.localScale.x);
            float rotAmount = worldDir / scale * wheelRotateSpeed * Time.deltaTime;
            wheelL.Rotate(0f, 0f, rotAmount);
            wheelR.Rotate(0f, 0f, rotAmount);
        }

        if (distX > 0 && facingRight)
            Flip();
        else if (distX < 0 && !facingRight)
            Flip();
    }
    
    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void FireProjectile()
    {
        if (playerTransform == null) return;

        GameObject newBullet = Instantiate(bulletPrefab, bulletTransform.position, Quaternion.identity);
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();

        Vector2 velocity = GetVelocity(bulletTransform.position,rb, playerTransform.position, launchAngle);
        rb.linearVelocity = velocity;
    }

    public Vector3 GetVelocity(Vector3 player,Rigidbody2D bulletRigid, Vector3 target, float initialAngle)
    {
        float gravity = Physics.gravity.magnitude * bulletRigid.gravityScale;
        float angle = initialAngle * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(target.x, 0, target.z);
        Vector3 planarPosition = new Vector3(player.x, 0, player.z);

        float distance = Vector3.Distance(planarTarget, planarPosition);
        float yOffset = player.y - target.y;

        float initialVelocity
            = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity
            = new Vector3(0f, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        float angleBetweenObjects
            = Vector3.Angle(Vector3.forward, planarTarget - planarPosition) * (target.x > player.x ? 1 : -1);
        Vector3 finalVelocity
            = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        return finalVelocity;
    }

    private void Die()
    {
        GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        newExplosion.transform.localScale = new Vector3(3, 3, 1);
        Destroy(gameObject);
    }
    
    public void ApplyDamage(float damage)
    {
        if (isInvincible) return;

        hp--;
        if (HitStop.Instance != null)
            HitStop.Instance.Stop(0.06f);

        if (hp <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(HitFlash());
    }

    IEnumerator HitFlash()
    {
        isInvincible = true;
        for (int i = 0; i < flashCount; i++)
        {
            SetFlash(1f);
            yield return new WaitForSecondsRealtime(flashDuration);
            SetFlash(0f);
            yield return new WaitForSecondsRealtime(flashDuration);
        }
        SetFlash(0f);
        isInvincible = false;
    }

    void SetFlash(float amount)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (amount > 0f)
            {
                flashMaterials[i].SetFloat("_FlashAmount", amount);
                spriteRenderers[i].material = flashMaterials[i];
            }
            else
            {
                spriteRenderers[i].material = originalMaterials[i];
            }
        }
    }
}