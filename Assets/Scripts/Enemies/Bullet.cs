    using System;
    using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject explosionPrefab;
    public int dmgValue;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Bullet")||other.gameObject.CompareTag("Destroyer"))
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            //Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            other.gameObject.GetComponent<CharacterController2D>().ApplyDamage(dmgValue, transform.position);
        }
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);    
        Destroy(gameObject);
    }
}
