using System;
using System.Collections;
using UnityEngine;

public class GasterBlaster : MonoBehaviour
{
    public int damage;
    public GameObject gasterBlaster;
    public GameObject beam;
    public float growSpeed = 1f;
    public Animator anim;

    public bool facingRight = true;

    private bool canShoot;

    private void Start()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.GasterBlasterAppear);
        if (!facingRight)
        {
            gasterBlaster.transform.localScale *= -1;
        }
        Destroy(gasterBlaster,1.5f);
        StartCoroutine(Shoot());
    }

    void Update()
    {
        if(!canShoot)
            return;
        float delta = growSpeed * Time.deltaTime;

        if (facingRight)
        {
            beam.transform.localScale += new Vector3(delta, 0, 0);
            beam.transform.position += new Vector3(delta/2, 0, 0);
            //gasterBlaster.transform.position -= new Vector3(delta, 0, 0);
        }
        else
        {
            beam.transform.localScale -= new Vector3(delta, 0, 0);
            beam.transform.position -= new Vector3(delta/2, 0, 0);
            //gasterBlaster.transform.position += new Vector3(delta, 0, 0);
        }
        
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Shoot");
        canShoot = true;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.GasterBlasterShoot);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CharacterController2D>().ApplyDamage(damage, transform.position);    
        }
        
    }
}