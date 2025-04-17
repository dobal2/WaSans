using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public float dmgValue = 4;
	public GameObject throwableObject;
	public Transform attackCheck;
	private Rigidbody2D m_Rigidbody2D;
	public Animator animator;
	public bool canAttack = true;
	public bool isTimeToCheck = false;

	public float attackRadius;

	public GameObject cam;

	public int attackCombo;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		attackCombo = 1;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonDown(0) && canAttack)
		{
			canAttack = false;
			if (attackCombo == 1)
			{
				attackCombo = 2;
			}
			else if (attackCombo == 2)
			{
				attackCombo = 1;
			}
			else
			{
				attackCombo = 1;
			}
			
			animator.SetBool("IsAttacking", true);
			animator.SetInteger("AttackCombo", attackCombo);
			//DoDashDamage();
			StartCoroutine(AttackCooldown());
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			GameObject throwableWeapon = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f,-0.2f), Quaternion.identity) as GameObject; 
			Vector2 direction = new Vector2(transform.localScale.x, 0);
			throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction; 
			throwableWeapon.name = "ThrowableWeapon";
		}
	}

	IEnumerator AttackCooldown()
	{
		yield return new WaitForSeconds(0.25f);
		canAttack = true;
	}

	public void DoDashDamage()
	{
		dmgValue = Mathf.Abs(dmgValue);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackRadius);
		for (int i = 0; i < collidersEnemies.Length; i++)
		{
			if (collidersEnemies[i].gameObject.tag == "Enemy")
			{
				if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
				{
					dmgValue = -dmgValue;
				}
				collidersEnemies[i].gameObject.SendMessage("ApplyDamage", dmgValue);
				cam.GetComponent<CameraFollow>().ShakeCamera();
			}
		}
	}
	
	private void OnDrawGizmosSelected()
	{
		if (attackCheck == null)
			return;

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackCheck.position, attackRadius);
	}

}
