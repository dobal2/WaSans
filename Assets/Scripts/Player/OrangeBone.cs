using UnityEngine;

public class OrangeBone : MonoBehaviour
{
    public int dmgValue;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Destroyer"))
        {
            Destroy(gameObject);
        }
        
        PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
        if (other.CompareTag("Player") && playerMovement.gameObject.GetComponent<Rigidbody2D>().linearVelocity == Vector2.zero)
        {
            other.gameObject.GetComponent<CharacterController2D>().ApplyDamage(dmgValue, transform.position);
        }
    }
}
