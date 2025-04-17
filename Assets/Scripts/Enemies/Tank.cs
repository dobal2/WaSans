using System;
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
    public float launchAngle = 70f; // 발사 각도 (도 단위)
    public GameObject explosion;

    private bool facingRight = false;


    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootCoolDown)
        {
            shootTimer = 0;
            FireProjectile();
        }
        
        if (playerTransform.position.x > transform.position.x)
        {
            if (facingRight)
            {
                Flip();    
            }
        }
        else if (playerTransform.position.x < transform.position.x)
        {
            if (!facingRight)
            {
                Flip();    
            }
        }
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
        hp--;
        if (hp <= 0)
        {
            Die();
        }
        
        // if (!isInvincible)
        // {
        //     hp--;
        //     StartCoroutine(HitTime());
        // }
    }
}