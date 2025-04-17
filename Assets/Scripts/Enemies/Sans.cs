using System.Collections;
using UnityEngine;
public class Sans : MonoBehaviour
{
    public int hp;
    public int dodgeCount;
    private Animator anim;
    private Rigidbody2D rigid;
    public Transform player;
    private Rigidbody2D playerRigid;
    public float speed;
    private bool facingRight;
    public GameObject bonePrefab;
    public GameObject blueBonePrefab;
    public GameObject orangeBonePrefab;
    public GameObject missTextPrefab;
    public GameObject damageTextPrefab;
    public GameObject gasterBlasterPrefab;
    
    
    
    private bool isAttacking;
    private float distance;

    private bool dead;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        playerRigid = player.gameObject.GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (dead || player.GetComponent<Animator>().GetBool("IsDead"))
        {
            anim.SetBool("IsWalking",false);
            StopCoroutine(PatternRoutine());
            return;
        }
            
        
        distance = Vector2.Distance(player.position, transform.position);
        Move();
        if (distance <= 5f)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.SansTeleport);
            float direction = Mathf.Sign(player.localScale.x);
            float offset = 8.0f;

            Vector3 newPosition = new Vector3(player.position.x + (direction * offset), transform.position.y, 0);
            transform.position = newPosition;
        }

        
        if (player.position.x > transform.position.x)
        {
            if (facingRight)
            {
                Flip();    
            }
        }
        else if (player.position.x < transform.position.x)
        {
            if (!facingRight)
            {
                Flip();    
            }
        }
        
        if (!isAttacking)
        {
            StartCoroutine(PatternRoutine());
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        AudioManager.Instance.sfxVolume = 1.5f;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.StrikeSans);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.StrikeSans);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.StrikeSans);
        dead = true;
        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0,0,90));
        StartCoroutine(Wait());

    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.GameClear();
    }
    
    IEnumerator PatternRoutine()
    {
        isAttacking = true;

        yield return new WaitForSeconds(1f);

        int rand = Random.Range(0, 9);

        switch (rand)
        {
            case 0:
                yield return StartCoroutine(ShootBonePattern());
                break;
            case 1:
                yield return StartCoroutine(MultiBonePattern());
                break;
            case 2:
                Teleport();
                break;
            case 3:
                yield return StartCoroutine(BlueBoneMany());
                break;
            case 4:
                yield return StartCoroutine(BlueAndOrangeMany());
                break;
            case 5:
                yield return StartCoroutine(ShootGasterBlasterPattern());
                break;
            case 6:
                yield return StartCoroutine(ShootThreeGasterBlasterPattern());
                break;
            case 7:
                yield return StartCoroutine(OrangeBoneMany());
                break;
            case 8:
                yield return StartCoroutine(ShootGasterBlasterPlayerXPos());
                break;
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
    
    IEnumerator ShootBonePattern()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.BoneWarning);
    
        GameObject newBone = Instantiate(bonePrefab, new Vector3(transform.position.x + (direction * 2),0,0), Quaternion.identity);
        newBone.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * 800f, 0f));
        

        yield return new WaitForSeconds(0.2f);
    }
    
    IEnumerator ShootGasterBlasterPattern()
    {
        yield return new WaitForSeconds(0.1f);
        float direction = Mathf.Sign(player.position.x - transform.position.x);
    
        GameObject newGasterBlaster = Instantiate(gasterBlasterPrefab, new Vector3(transform.position.x + (direction * 20),-2,0), Quaternion.identity);
        newGasterBlaster.GetComponentInChildren<GasterBlaster>().facingRight = facingRight;

        yield return new WaitForSeconds(0.2f);
    }
    
    IEnumerator ShootGasterBlasterPlayerXPos()
    {
        yield return new WaitForSeconds(0.03f);
        float direction = Mathf.Sign(player.position.x - transform.position.x);
    
        GameObject newGasterBlaster = Instantiate(gasterBlasterPrefab, new Vector3(transform.position.x + (direction * 25),player.transform.position.y,0), Quaternion.identity);
        newGasterBlaster.GetComponentInChildren<GasterBlaster>().facingRight = facingRight;

        yield return new WaitForSeconds(0.2f);
    }
    
    IEnumerator ShootThreeGasterBlasterPattern()
    {
        yield return new WaitForSeconds(0.3f);
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        for (int i = 0; i < 3; i++)
        {
            GameObject newGasterBlaster = Instantiate(gasterBlasterPrefab, new Vector3(transform.position.x + (direction * 20),-2 +(i*2.5f),0), Quaternion.identity);
            newGasterBlaster.GetComponentInChildren<GasterBlaster>().facingRight = facingRight;
        }
        

        yield return new WaitForSeconds(0.2f);
    }
    
    IEnumerator MultiBonePattern()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        for (int i = 0; i < 5; i++)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.BoneWarning);
            int random = Random.Range(1, 4);

            GameObject newBone = null;
            
            direction = Mathf.Sign(player.position.x - transform.position.x);

            if (random == 1)
            {
                newBone = Instantiate(bonePrefab, new Vector3(transform.position.x + (direction * 2),0,0), Quaternion.identity);
            }
            else if (random == 2)
            {
                newBone = Instantiate(blueBonePrefab, new Vector3(transform.position.x + (direction * 2),0,0), Quaternion.identity);
            }
            else if (random == 3)
            {
                newBone = Instantiate(orangeBonePrefab, new Vector3(transform.position.x + (direction * 2),0,0), Quaternion.identity);
            }

            float randomForce = Random.Range(300f, 700f);
            
            newBone.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * randomForce, 0));
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.5f);
    }
    
    IEnumerator BlueBoneMany()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        for (int i = 0; i < 10; i++)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.BoneWarning);
            direction = Mathf.Sign(player.position.x - transform.position.x);
            GameObject newBone = Instantiate(blueBonePrefab, new Vector3(transform.position.x + (direction * 2),0,0), Quaternion.identity);
            
            float randomForce = Random.Range(300f, 700f);
            
            newBone.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * randomForce, 0));
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.4f);
    }
    
    IEnumerator OrangeBoneMany()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        for (int i = 0; i < 10; i++)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.BoneWarning);
            direction = Mathf.Sign(player.position.x - transform.position.x);
            GameObject newBone = Instantiate(orangeBonePrefab, new Vector3(transform.position.x + (direction * 2),0,0), Quaternion.identity);
            
            float randomForce = Random.Range(300f, 700f);
            
            newBone.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * randomForce, 0));
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.4f);
    }
    
    IEnumerator BlueAndOrangeMany()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        for (int i = 0; i < 10; i++)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.BoneWarning);
            direction = Mathf.Sign(player.position.x - transform.position.x);
            int random = Random.Range(1, 3);

            GameObject newBone = null;

            if (random == 1)
            {
                newBone = Instantiate(orangeBonePrefab, new Vector3(transform.position.x + (direction * 2),0,0), Quaternion.identity);
            }
            else if (random == 2)
            {
                newBone = Instantiate(blueBonePrefab, new Vector3(transform.position.x + (direction * 2),0,0), Quaternion.identity);
            }
            float randomForce = Random.Range(300f, 1200f);
            
            newBone.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction *randomForce, 0));
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(0.7f);
    }

    private void Teleport()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.SansTeleport);
        float direction = Mathf.Sign(player.localScale.x); // 또는 playerRigid.velocity.x도 가능
        float offset = 7.0f; // 플레이어 앞쪽 몇 유닛에 텔레포트할지 설정

        Vector3 newPosition = new Vector3(player.position.x + (direction * offset), transform.position.y, 0);
        transform.position = newPosition;
    }





    private void Move()
    {
        distance = Vector2.Distance(player.position, transform.position);

        if (distance >= 10)
        {
            anim.SetBool("IsWalking",true);
            if (facingRight)
            {
                transform.Translate(new Vector2((speed) * -1, rigid.linearVelocity.y) * Time.deltaTime);
            }
            else
            {
                transform.Translate(new Vector2((speed), rigid.linearVelocity.y) * Time.deltaTime);;  
            }
            
        }
        else
        {
            anim.SetBool("IsWalking",false);
        }
        if (distance >= 20)
        {
            Teleport();
            // if (facingRight)
            // {
            //     transform.Translate(new Vector2((speed) * -1, rigid.linearVelocity.y));
            // }
            // else
            // {
            //     transform.Translate(new Vector2((speed), rigid.linearVelocity.y));
            // }
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

    public void ApplyDamage(float damage)
    {
        if(dead)
            return;
        if (dodgeCount<=0)
        {
            Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            Die();
            return;
        }
        dodgeCount--;
        Debug.Log("DodgeCount= "+dodgeCount);
        Instantiate(missTextPrefab, transform.position, Quaternion.identity);
        float direction = damage / Mathf.Abs(damage);
        damage = Mathf.Abs(damage);
        transform.Translate(new Vector2(-direction * 15, rigid.linearVelocity.y));
        
        //playerRigid.AddForce(new Vector2(direction * 900,playerRigid.linearVelocity.y),ForceMode2D.Impulse);

        if (Random.value <= 0.5f)
        {
            StartCoroutine(ShootBonePattern());
        }
        else
        {
            StartCoroutine(ShootGasterBlasterPattern());
        }
        
        
        
        
    }
}
