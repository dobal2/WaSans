using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ally : MonoBehaviour
{
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.

	public float life = 10;

	private bool facingRight = true;

	public float speed = 5f; 

	public bool isInvincible = false;
	private bool isHitted = false;

	[SerializeField] private float m_DashForce = 25f;
	private bool isDashing = false;

	public GameObject enemy;
	private float distToPlayer;
	private float distToPlayerY;
	public float meleeDist = 1.5f;
	public float rangeDist = 5f;
	private bool canAttack = true;
	public Transform attackCheck;
	public int dmgValue = 4;

	public GameObject throwableObject;

	private float randomDecision = 0;
	private bool doOnceDecision = true;
	private bool endDecision = false;
	private Animator anim;

	public float attackRadius;

	void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		attackCheck = transform.Find("AttackCheck").transform;
		anim = GetComponent<Animator>();
	}

	private void Start()
	{
		enemy = GameObject.FindGameObjectWithTag("Player");
	}

	void FixedUpdate()
	{

		if (life <= 0)
		{
			StartCoroutine(DestroyEnemy());
		}

		else if (enemy != null) 
		{
			if (isDashing)
			{
				m_Rigidbody2D.linearVelocity = new Vector2(transform.localScale.x * m_DashForce, 0);
			}
			else if (!isHitted)
			{
				distToPlayer = enemy.transform.position.x - transform.position.x;
				distToPlayerY = enemy.transform.position.y - transform.position.y;
				
				//Debug.Log(distToPlayerY);

				if (Mathf.Abs(distToPlayer) < 3f)
				{
					GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, m_Rigidbody2D.linearVelocity.y);
					anim.SetBool("IsWaiting", true);
				}
				else if (Mathf.Abs(distToPlayer) > 3f && Mathf.Abs(distToPlayer) < meleeDist && Mathf.Abs(distToPlayerY) < 4f)
				{
					GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, m_Rigidbody2D.linearVelocity.y);
					if ((distToPlayer > 0f && transform.localScale.x < 0f) || (distToPlayer < 0f && transform.localScale.x > 0f)) 
						Flip();
					if (canAttack)
					{
						MeleeAttack();
					}
				}
				else if (Mathf.Abs(distToPlayer) > meleeDist && Mathf.Abs(distToPlayer) < rangeDist)
				{
					anim.SetBool("IsWaiting", false);
					m_Rigidbody2D.linearVelocity = new Vector2(distToPlayer / Mathf.Abs(distToPlayer) * speed, m_Rigidbody2D.linearVelocity.y);
				}
				else
				{
					if (!endDecision)
					{
						if ((distToPlayer > 0f && transform.localScale.x < 0f) || (distToPlayer < 0f && transform.localScale.x > 0f)) 
							Flip();

						if (randomDecision < 0.4f)
							Run();
						else if (randomDecision >= 0.4f && randomDecision < 0.6f)
							Jump();
						else if (randomDecision >= 0.6f && randomDecision < 0.8f)
							StartCoroutine(Dash());
						// else if (randomDecision >= 0.8f && randomDecision < 0.95f)
						// 	RangeAttack();
						else
							Idle();
					}
					else
					{
						endDecision = false;
					}
				}
			}
			else if (isHitted)
			{
				if ((distToPlayer > 0f && transform.localScale.x > 0f) || (distToPlayer < 0f && transform.localScale.x < 0f))
				{
					//Flip();
					//StartCoroutine(Dash());
				}
				else
				{
					//StartCoroutine(Dash());
				}
					
			}
		}

		if (!isHitted && transform.localScale.x * m_Rigidbody2D.linearVelocity.x > 0 && !m_FacingRight && life > 0)
		{
			Flip();
		}
		else if (!isHitted && transform.localScale.x * m_Rigidbody2D.linearVelocity.x < 0 && m_FacingRight && life > 0)
		{
			Flip();
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
		if (!isInvincible)
		{
			float direction = damage / Mathf.Abs(damage);
			damage = Mathf.Abs(damage);
			anim.SetBool("Hit", true);
			life -= damage;
			StartCoroutine(HitTime());
		}
	}

	public void MeleeAttack()
	{
		transform.GetComponent<Animator>().SetBool("Attack", true);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackRadius);
		for (int i = 0; i < collidersEnemies.Length; i++)
		{
			if (collidersEnemies[i].gameObject.tag == "Player")
			{
				collidersEnemies[i].gameObject.GetComponent<CharacterController2D>().ApplyDamage(dmgValue, transform.position);
			}
		}
		StartCoroutine(WaitToAttack(1f));
	}

	// public void RangeAttack()
	// {
	// 	if (doOnceDecision)
	// 	{
	// 		GameObject throwableProj = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f, -0.2f), Quaternion.identity) as GameObject;
	// 		throwableProj.GetComponent<ThrowableProjectile>().owner = gameObject;
	// 		Vector2 direction = new Vector2(transform.localScale.x, 0f);
	// 		throwableProj.GetComponent<ThrowableProjectile>().direction = direction;
	// 		StartCoroutine(NextDecision(0.5f));
	// 	}
	// }

	public void Run()
	{
		anim.SetBool("IsWaiting", false);
		m_Rigidbody2D.linearVelocity = new Vector2(distToPlayer / Mathf.Abs(distToPlayer) * speed, m_Rigidbody2D.linearVelocity.y);
		if (doOnceDecision)
			StartCoroutine(NextDecision(0.5f));
	}
	public void Jump()
	{
		Vector3 targetVelocity = new Vector2(distToPlayer / Mathf.Abs(distToPlayer) * speed, m_Rigidbody2D.linearVelocity.y);
		Vector3 velocity = Vector3.zero;
		m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref velocity, 0.05f);
		if (doOnceDecision)
		{
			anim.SetBool("IsWaiting", false);
			m_Rigidbody2D.AddForce(new Vector2(0f, 850f));
			StartCoroutine(NextDecision(1f));
		}
	}

	public void Idle()
	{
		m_Rigidbody2D.linearVelocity = new Vector2(0f, m_Rigidbody2D.linearVelocity.y);
		if (doOnceDecision)
		{
			anim.SetBool("IsWaiting", true);
			StartCoroutine(NextDecision(1f));
		}
	}

	public void EndDecision()
	{
		randomDecision = Random.Range(0.0f, 1.0f); 
		endDecision = true;
	}

	IEnumerator HitTime()
	{
		float direction = Mathf.Sign(enemy.transform.position.x - transform.position.x);
		isInvincible = true;
		isHitted = true;

		// 넉백 후 일정 시간 동안 이동 멈춤
		m_Rigidbody2D.linearVelocity = Vector2.zero;
		transform.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-direction * 1000f, 100f)); 
    
		// 움직임 막는 시간
		yield return new WaitForSeconds(0.1f);
		m_Rigidbody2D.linearVelocity = Vector2.zero;
    
		isInvincible = false;

		// 다시 움직이기까지 잠깐 기다림
		yield return new WaitForSeconds(0.2f);
		isHitted = false;
	}


	IEnumerator WaitToAttack(float time)
	{
		canAttack = false;
		yield return new WaitForSeconds(time);
		canAttack = true;
	}

	IEnumerator Dash()
	{
		anim.SetBool("IsDashing", true);
		isDashing = true;
		yield return new WaitForSeconds(0.1f);
		isDashing = false;
		EndDecision();
	}

	IEnumerator NextDecision(float time)
	{
		doOnceDecision = false;
		yield return new WaitForSeconds(time);
		EndDecision();
		doOnceDecision = true;
		anim.SetBool("IsWaiting", false);
	}

	IEnumerator DestroyEnemy()
	{
		transform.GetComponent<Animator>().SetBool("IsDead", true);
		yield return new WaitForSeconds(0.25f);
		m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
		yield return new WaitForSeconds(1f);
		Destroy(gameObject);
	}
	
	private void OnDrawGizmosSelected()
	{
		// 공격 체크 위치가 없으면 생략
		if (attackCheck != null)
		{
			// 근접 공격 범위 (빨간색)
			Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
			Gizmos.DrawWireSphere(attackCheck.position, attackRadius);
		}

		// 사거리 탐지 범위 (노란색)
		if (enemy != null)
		{
			Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
			Gizmos.DrawWireSphere(transform.position, rangeDist);

			// 근접 공격 범위 (파란색)
			Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
			Gizmos.DrawWireSphere(transform.position, meleeDist);
		}
	}

}
