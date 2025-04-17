using System;
using UnityEngine;

public class Bone : MonoBehaviour
{
    public int dmgValue;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Destroyer"))
        {
            Destroy(gameObject);
        }
        
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CharacterController2D>().ApplyDamage(dmgValue, transform.position);
        }
    }
}
