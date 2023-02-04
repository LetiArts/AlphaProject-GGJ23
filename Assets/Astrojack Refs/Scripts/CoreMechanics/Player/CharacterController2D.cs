using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
	public static CharacterController2D instance;

	public PlayerStats playerStats;
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_WallCheck;								//Posicion que controla si el personaje toca una pared
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	[HideInInspector] public bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;
	private float limitFallSpeed = 25f; // Limit fall speed

	public bool canDoubleJump = true; //If player can double jump
	[SerializeField] private float m_DashForce = 25f;
	private bool canDash = true;
	private bool isDashing = false; //If player is dashing
	private bool m_IsWall = false; //If there is a wall in front of the player
	private bool isWallSliding = false; //If player is sliding in a wall
	private float prevVelocityX = 0f;
	public bool invincible = false; //If player can die
	private bool canMove = true; //If player can move

	private Animator animator;
	public ParticleSystem particleJumpUp; //Trail particles
	public ParticleSystem particleJumpDown; //Explosion particles


	public GameObject Arm;

	[Header("Events")]
	[Space]

	public UnityEvent OnFallEvent;
	public UnityEvent OnLandEvent;


	[Header("Jetpack: ")]
	[SerializeField]
	bool canUseJetpack = false;
	[HideInInspector]
	public bool switchedToJetpack = false;
	public JetpackFuelBar JetpackBar;
	public ParticleSystem flames;
	private float jetpackTimeCounter = 0f;
	float defaultForce;
	[HideInInspector] public float maxJetpackTime;
	[HideInInspector] public float fillupRate;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		instance = this;

		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		if (OnFallEvent == null)
			OnFallEvent = new UnityEvent();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		defaultForce = playerStats.thrustForce;
		maxJetpackTime = playerStats.maxJetpackTime;
		fillupRate = playerStats.fillupRate;
	}


	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapBoxAll(m_GroundCheck.position, _groundCheckSize, 0, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				m_Grounded = true;
				if (!wasGrounded )
				{
					OnLandEvent.Invoke();
					if (!m_IsWall && !isDashing) 
						particleJumpDown.Play();
					canDoubleJump = true;
				}
		}

		m_IsWall = false;

		if (!m_Grounded)
		{
			OnFallEvent.Invoke();
			Collider2D[] collidersWall = Physics2D.OverlapCircleAll(m_WallCheck.position, k_GroundedRadius, m_WhatIsGround);
			for (int i = 0; i < collidersWall.Length; i++)
			{
				if (collidersWall[i].gameObject != null)
				{
					isDashing = false;
					m_IsWall = true;
				}
			}
			prevVelocityX = m_Rigidbody2D.velocity.x;
		}

		if (canUseJetpack && Input.GetKey("space"))
		{
			if (jetpackTimeCounter < maxJetpackTime)
			{
				flames.Play();

				jetpackTimeCounter += Time.deltaTime;

				m_Rigidbody2D.AddForce(Vector2.up * playerStats.thrustForce, ForceMode2D.Impulse);

				playerStats.thrustForce -= Time.deltaTime;

				// if (m_Rigidbody2D.velocity.y < maxVerticalSpeed)
				// 	// m_Rigidbody2D.velocity += force * Time.deltaTime;
				// else
				// 	// m_Rigidbody2D.velocity = new Vector2(0f, maxVerticalSpeed);
				// 	m_Rigidbody2D.AddForce(Vector2.up * maxVerticalSpeed, m_ForceMode);
			}else{
				canUseJetpack = false;
				playerStats.thrustForce = defaultForce;
				flames.Stop();
			}
		}
		else
		{
			if (jetpackTimeCounter > 0f)
			{
				jetpackTimeCounter -= fillupRate * Time.deltaTime;
			}
			else
			{
				jetpackTimeCounter = 0f;
			}
			canUseJetpack = false;
			playerStats.thrustForce = defaultForce;
			flames.Stop();
		}

		if (JetpackBar)
		{
			JetpackBar.UpdateFuelBar(1f - jetpackTimeCounter / maxJetpackTime);
		}
	}


	public void Move(float move, bool jump, bool dash)
	{
		if (canMove) {
			if (dash && canDash && !isWallSliding)
			{
				//m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_DashForce, 0f));
				StartCoroutine(DashCooldown());
			}
			// If crouching, check to see if the character can stand up
			if (isDashing)
			{
				m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * m_DashForce, 0);
			}
			//only control the player if grounded or airControl is turned on
			else if (m_Grounded || m_AirControl)
			{
				if (m_Rigidbody2D.velocity.y < -limitFallSpeed)
					m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -limitFallSpeed);
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
			}
			// If the player should jump...
			if (m_Grounded && jump)
			{
				// Add a vertical force to the player.
				animator.SetBool("IsJumping", true);
				animator.SetBool("JumpUp", true);
				Jump();
			}
			else if (!m_Grounded && jump && canDoubleJump && !isWallSliding && !switchedToJetpack)
			{
				float tempForce = m_JumpForce -10f;
				canDoubleJump = false;
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
				// m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 1.2f));

				m_Rigidbody2D.AddForce(Vector2.up * tempForce, ForceMode2D.Impulse);

				animator.SetBool("IsDoubleJumping", true);
			}else if (!m_Grounded && jump && !canUseJetpack && switchedToJetpack)
			{
				canUseJetpack = true;
			}
		}
	}

	void Jump()
	{
		m_Grounded = false;
		// if (RB.velocity.y < 0)
        //     force -= RB.velocity.y;
        m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);

		// m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		canDoubleJump = true;
		particleJumpUp.Play();
	}

	public void KillPlayer()
	{
		StartCoroutine(WaitToDead());
	}


	public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	IEnumerator DashCooldown()
	{
		animator.SetBool("IsDashing", true);
		isDashing = true;
		canDash = false;
		yield return new WaitForSeconds(0.1f);
		isDashing = false;
		yield return new WaitForSeconds(0.5f);
		canDash = true;
	}

	IEnumerator Stun(float time) 
	{
		canMove = false;
		yield return new WaitForSeconds(time);
		canMove = true;
	}

	IEnumerator WaitToMove(float time)
	{
		canMove = false;
		yield return new WaitForSeconds(time);
		canMove = true;
	}

	IEnumerator WaitToEndSliding()
	{
		yield return new WaitForSeconds(0.1f);
		canDoubleJump = true;
		isWallSliding = false;
		animator.SetBool("IsWallSliding", false);
		m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
	}

	IEnumerator WaitToDead()
	{
		animator.SetBool("IsDead", true);
		canMove = false;
		invincible = true;
		Arm.SetActive(false);
		yield return new WaitForSeconds(0.4f);
		m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
		yield return new WaitForSeconds(1.1f);
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}

	#region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_GroundCheck.position, _groundCheckSize);
    }
    #endregion
}
