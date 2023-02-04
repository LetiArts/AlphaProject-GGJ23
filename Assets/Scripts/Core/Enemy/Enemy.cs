using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

	
public class Enemy : MonoBehaviour
{
	public EnemyProperties enemyProperties;
	public bool isInvincible = false;
	public Transform attackCheck;
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WalkingLayers;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	private bool m_onGroundLevel;            // Whether or not the player is grounded.
	[SerializeField] private LayerMask m_WhatIsActualGround;					// A mask determining what is the actual ground player is on

	[Header("Events")]
	[Space]

	public UnityEvent OnFallEvent;
	public UnityEvent OnLandEvent;

	private bool m_Grounded;            // Whether or not the player is grounded.
	
	private float prevVelocityX = 0f;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private bool facingRight = true;
	private bool isHit = false;
	public GameObject targetObj;
	// private bool isDashing = false;
	private float distToPlayer;
	private float distToPlayerY;
	private bool canAttack = true;
	private int _curHealth;
	public int curHealth
	{
		get { return _curHealth; }
		set { _curHealth = Mathf.Clamp (value, 0, enemyProperties.maxHealth); }
	}

	[Header("Optional: ")]
	[SerializeField]
	public GameObject missile;
	public GameObject deathParticles;
	public GameObject throwableObject;
	public StatusIndicator statusIndicator;

	private float randomDecision = 0;
	private bool inDecision = true;
	private bool endDecision = false;
	bool isDead = false;
	private Animator anim;

	public void Init()
	{
		curHealth = enemyProperties.maxHealth;
	}

	void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	private void Start() {
		Init();

		FindTarget();

		if (deathParticles == null)
		{
			Debug.LogWarning("No death particles referenced on Enemy"); 
		}

		m_AirControl = enemyProperties.airControl;
	}

	void FindTarget()
	{
		bool gotTreeTarget = false;

		if (enemyProperties.target == "Player")
		{
			targetObj = GameObject.FindGameObjectWithTag (enemyProperties.target);
		}
		//if we have Tree and its not above ground, let's go for it else let's go for player
		else
		{
			targetObj = GameObject.FindGameObjectWithTag (enemyProperties.target);

			//if there are no available Trees on ground, let's get player
			if (targetObj == null)
			{
				targetObj = GameObject.FindGameObjectWithTag ("Player");
			}
		}
	}


	public void Falling()
	{
		m_Grounded = false;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The enemy is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapBoxAll(m_GroundCheck.position, _groundCheckSize, 0, m_WalkingLayers);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				m_Grounded = true;
				
			if (!wasGrounded )
			{
				OnLandEvent.Invoke();
			}
		}


		if (!m_Grounded)
		{
			OnFallEvent.Invoke();
			prevVelocityX = m_Rigidbody2D.velocity.x;
		}

		if (targetObj != null) 
		{
			if (!isHit)
			{
				distToPlayer = targetObj.transform.position.x - transform.position.x;
				distToPlayerY = targetObj.transform.position.y - transform.position.y;

				if (Mathf.Abs(distToPlayer) < 0.25f)
				{
					// anim.SetBool("IsWaiting", true);
					anim.SetBool("isRunning", false);
					m_Rigidbody2D.velocity = new Vector2(0f, m_Rigidbody2D.velocity.y);
				}
				else if (Mathf.Abs(distToPlayer) > 0.25f && Mathf.Abs(distToPlayer) < enemyProperties.meleeDist && Mathf.Abs(distToPlayerY) < enemyProperties.meleeDist)
				{
					m_Rigidbody2D.velocity = new Vector2(0f, m_Rigidbody2D.velocity.y);


					//I set the Melee attack function in animator
					if (canAttack)
					{
						DoMelee();
					}
				}
				else if (Mathf.Abs(distToPlayer) > enemyProperties.meleeDist && Mathf.Abs(distToPlayer) < enemyProperties.rangeDist)
				{
					// anim.SetBool("IsWaiting", false);
					Run();
				}
				else
				{
					if (!endDecision && !isDead)
					{
						Run();

						//if enemy is aggressive
						if(enemyProperties.canJump)
						{
							// if (randomDecision >= 0.4f && randomDecision < 0.6f)
							// {
							// 	Jump();
							// }
						}

						if (enemyProperties.canDash)
						{
							if (randomDecision >= 0.6f && randomDecision < 0.8f)
							{
								StartCoroutine(Dash());
							}
						}

						//if enemy is a witch
						if (enemyProperties.canShoot)
						{
							if (randomDecision >= 0.8f && randomDecision < 0.95f)
							{
								RangeAttack();
							}
						}
					}
					else if (endDecision && !isDead)
					{
						StartCoroutine(UnEndDecision());
					}
				}
			}
		}
		

		if (transform.localScale.x * m_Rigidbody2D.velocity.x > 0 && !m_FacingRight && curHealth > 0)
		{
			// ... flip the player.
			Flip();
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (transform.localScale.x * m_Rigidbody2D.velocity.x < 0 && m_FacingRight && curHealth > 0)
		{
			// ... flip the player.
			Flip();
		}
	}

	public bool OnGroundLevel()
	{
		bool wasOnGroundLevel = m_onGroundLevel;
		m_onGroundLevel = false;

		Collider2D[] colliders = Physics2D.OverlapBoxAll(m_GroundCheck.position, _groundCheckSize, 0, m_WhatIsActualGround);

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				m_onGroundLevel = true;
		}

		return m_onGroundLevel;
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

	public void DamageEnemy(float damage)
	{
		if (!isInvincible)
		{
			if (enemyProperties.maxHealth < 200)
			{
				StartCoroutine(HitTime());
				curHealth -= (int)damage;
			}else{
				//let's do half damage when attacking bosses so that they feel harders
				curHealth -= (int)damage/2;
			}
			
			float direction = damage / Mathf.Abs(damage);
			damage = Mathf.Abs(damage);
			// transform.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			// transform.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * 10f, 5f)); 

			

			if (curHealth <= 0 && !isDead)
			{
				StartCoroutine(DestroyEnemy());
			}
		}
	}

	public void DoMelee()
	{
		anim.SetBool("Attack", true);
	}

	//this is called in animator
	public void MeleeAttack()
	{
		if(canAttack)
		{
			// transform.GetComponent<Animator>().SetBool("Attack", true);
			Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 1f);
			for (int i = 0; i < collidersEnemies.Length; i++)
			{
				if (collidersEnemies[i].gameObject.tag == "Player")
				{
					Player.instance.DamagePlayer(enemyProperties.dealingDamage);
				}
				else if (enemyProperties.target == "Tree" && collidersEnemies[i].gameObject.tag == "Tree")
				{
					//inflict half damage when targeting Tree
					collidersEnemies[i].gameObject.GetComponent<Tree>().DamageTree(enemyProperties.dealingDamage / 2);
					// Tree.instance.DamageTree(enemyProperties.dealingDamage / 2);
				}
			}
			StartCoroutine(EndAttack(1f));
		}

	}

	public void RangeAttack()
	{
		if (inDecision)
		{
			GameObject throwableProj = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f, -0.2f), Quaternion.identity) as GameObject;
			throwableProj.GetComponent<ThrowableProjectile>().owner = gameObject;
			Vector2 direction = new Vector2(transform.localScale.x, 0f);
			throwableProj.GetComponent<ThrowableProjectile>().direction = direction;
			StartCoroutine(NextDecision(0.5f));
		}
	}

	public void Run()
	{
		//only control the enemy if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			m_Rigidbody2D.velocity = new Vector2(distToPlayer / Mathf.Abs(distToPlayer) * enemyProperties.moveSpeed, m_Rigidbody2D.velocity.y);
			anim.SetBool("isRunning", true);
		}
		
		if (inDecision)
		{
			StartCoroutine(NextDecision(0.5f));
		}
	}
	public void Jump()
	{
		Vector3 targetVelocity = new Vector2(distToPlayer / Mathf.Abs(distToPlayer) * enemyProperties.moveSpeed, m_Rigidbody2D.velocity.y);
		Vector3 velocity = Vector3.zero;
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, 0.05f);
		if (inDecision)
		{
			// anim.SetBool("IsWaiting", false);
			m_Rigidbody2D.AddForce(new Vector2(0f, 850f));
			StartCoroutine(NextDecision(1f));
		}
	}

	public void EndDecision()
	{
		m_Rigidbody2D.velocity = new Vector2(0f, m_Rigidbody2D.velocity.y);
		
		anim.SetBool("isRunning", false);
		anim.SetBool("IsDashing", false);
		anim.SetBool("Attack", false);
					
		randomDecision = Random.Range(0.0f, 1.0f); 
		endDecision = true;
	}

	IEnumerator UnEndDecision()
	{
		yield return new WaitForSeconds(enemyProperties.timeToDecideNextMove);
		endDecision = false;
	}

	IEnumerator HitTime()
	{
		// isInvincible = true;
		isHit = true;
		anim.SetBool("Hit", true);
		canAttack = false;
		yield return new WaitForSeconds(0.2f);
		canAttack = true;
		isHit = false;
		anim.SetBool("Hit", false);
		// isInvincible = false;
	}

	IEnumerator EndAttack(float time)
	{
		canAttack = false;
		yield return new WaitForSeconds(time);
		anim.SetBool("Attack", false);
		canAttack = true;
	}

	IEnumerator Dash()
	{
		m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * enemyProperties.m_DashForce, 0);
		anim.SetBool("IsDashing", true);
		// isDashing = true;
		yield return new WaitForSeconds(0.1f);
		// isDashing = false;
		EndDecision();
	}

	IEnumerator NextDecision(float time)
	{
		inDecision = false;
		EndDecision();
		yield return new WaitForSeconds(time);
		inDecision = true;
		// anim.SetBool("IsWaiting", false);
	}

	// void OnCollisionEnter2D(Collision2D _colInfo)
	// {
	// 	//Only normal enemies can hurt player on collision 
	// 	if(enemyProperties.movingMine)
	// 	{
	// 		if (_colInfo.gameObject.tag == "Player")
	// 		{
	// 			Player.instance.DamagePlayer(enemyProperties.dealingDamage);
	// 			DamageEnemy(101);
	// 			Debug.Log("Col");
	// 		}
	// 	}
	// }

	IEnumerator DestroyEnemy()
	{
		isDead = true;

		if (UIController.instance != null)
        {
            UIController.instance.curKills++;
        }
		
		ItemDropper.instance.DropItem("Coin", transform.position);
		this.GetComponent<CapsuleCollider2D>().enabled = false;
		transform.GetComponent<Animator>().SetBool("IsDead", true);
		yield return new WaitForSeconds(1f);
		Destroy(gameObject);
				
		// //sound
		SoundManager.instance.PlaySFX(enemyProperties.deathSoundName);
	}

	#region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_GroundCheck.position, _groundCheckSize);
		Gizmos.DrawWireCube(attackCheck.position, new Vector2(1f, 1f));
    }
    #endregion
}
