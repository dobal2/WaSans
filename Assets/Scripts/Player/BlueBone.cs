using System;
using UnityEngine;

public class BlueBone : MonoBehaviour
{
    public int dmgValue;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();

        if (other.CompareTag("Destroyer"))
        {
            Destroy(gameObject);
        }
        
        if (other.CompareTag("Player") && playerMovement.gameObject.GetComponent<Rigidbody2D>().linearVelocity != Vector2.zero)
        {
            other.gameObject.GetComponent<CharacterController2D>().ApplyDamage(dmgValue, transform.position);
        }
    }
}
