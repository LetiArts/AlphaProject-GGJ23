using UnityEngine;
using System.Collections;
using Pathfinding;
using TMPro;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
public class EnemyAI : MonoBehaviour {
	public EnemyProperties enemyProperties;
	// What to chase?
	private Transform target;
	public Animator DroneAnim;
	public TMP_Text CountdownText;
	
	// How many times each second we will update our path
	public float updateRate = 2f;
	public Transform enemyGFX;
	public Transform attackCheck;
	public float attackArea = 1f;

	// Caching
	private Seeker seeker;
	private Rigidbody2D rb;
	
	//The calculated path
	public Path path;
	
	//The AI's speed per second
	public float speed = 300f;
	
	[HideInInspector]
	public bool pathIsEnded = false;
	
	// The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	
	// The waypoint we are currently moving towards
	private int currentWaypoint = 0;
	private bool canAttack = true;
	private bool searchingForTarget = false;
	private float distToTarget, distToTargetY;
	private bool isInvincible = false;
	private bool reachedDestination = false;
	private bool isDead = false;
	// private bool isHit = false;
	private int countdownTime;
	bool isCounting = false;
	private int _curHealth;
	public int curHealth
	{
		get { return _curHealth; }
		set { _curHealth = Mathf.Clamp (value, 0, enemyProperties.maxHealth); }
	}


	public void Init()
	{
		curHealth = enemyProperties.maxHealth;
	}
	
	void Start () 
	{
		if (enemyProperties != null)
		{
			Init();

			//if enemy explodes
			if (enemyProperties.movingMine)
			{
				//let's restict delay to 10 seconds
				if (enemyProperties.explosionDelay < 10)
				{
					countdownTime = 10;
				}else{
					countdownTime = enemyProperties.explosionDelay;
				}
			}
		}

		seeker = GetComponent<Seeker>();
		rb = GetComponent<Rigidbody2D>();
		
		if (target == null) {
			if (!searchingForTarget) 
			{
				searchingForTarget = true;
				StartCoroutine (searchForTarget ());
			}
			return;
		}
		
		// Start a new path to the target position, return the result to the OnPathComplete method
		seeker.StartPath (transform.position, target.position, OnPathComplete);
		StartCoroutine (UpdatePath ());
	}

	IEnumerator searchForTarget()
	{
		GameObject[] sResults = GameObject.FindGameObjectsWithTag (enemyProperties.target);
		int index = Random.Range (0, sResults.Length-1);

		GameObject result = sResults[index];

		if (result == null) 
		{
			yield return new WaitForSeconds (0.5f);
			StartCoroutine (searchForTarget ());
		}
		else 
		{
			target = result.transform;
			searchingForTarget = false;
			StartCoroutine (UpdatePath ());
			yield return false;
		}
	}

	IEnumerator UpdatePath()   
	{
		if (target == null) 
		{
			if (!searchingForTarget) 
			{
				searchingForTarget = true;
				reachedDestination = false;
				StartCoroutine (searchForTarget ());
			}
			yield return false;
		}

		//if we haven't reached Destination and there's a target
		if (!reachedDestination && target != null)
		{
			// Start a new path to the target position, return the result to the OnPathComplete method
			seeker.StartPath (transform.position, target.position, OnPathComplete);
		}
		
		yield return new WaitForSeconds ( 1f/updateRate );
		StartCoroutine (UpdatePath());
	}
	
	public void OnPathComplete (Path p) {
		Debug.Log ("We got a path. Did it have an error? " + p.error);
		if (!p.error) {
			path = p;
			currentWaypoint = 0;
		}
	}
	
	void FixedUpdate () 
	{
		if (!isDead)
		{
			if (path == null)
				return;
			
			if (currentWaypoint >= path.vectorPath.Count) {
				if (pathIsEnded)
					return;
				
				Debug.Log ("End of path reached.");
				pathIsEnded = true;
				return;
			}
			pathIsEnded = false;
		
			//Direction to the next waypoint
			Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
			Vector2 force = dir * speed * Time.fixedDeltaTime;

			//Move the AI
			rb.AddForce (force);
			
			float dist = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
			
			if (dist < nextWaypointDistance) {
				currentWaypoint++;
			}

			//Always look at player
			if (rb.velocity.x > 1)
			{
				enemyGFX.localScale = new Vector3 (1f, 1f, 1f);
			}else if (rb.velocity.x < 0) {
				enemyGFX.localScale = new Vector3 (-1f, 1f, 1f);
			}

			distToTarget = target.transform.position.x - transform.position.x;
			distToTargetY = target.transform.position.y - transform.position.y;
			
			if (enemyProperties.target == "Tree" && enemyProperties.movingMine)
			{
				if (Mathf.Abs(distToTarget) > 0.25f && Mathf.Abs(distToTarget) < enemyProperties.explodeDist && Mathf.Abs(distToTargetY) < 2f)
				{
					if (isCounting == false && enemyProperties.movingMine)
					{
						PopCounter();
						isCounting = true;
					}
					else if (canAttack)
					{
						DoAttack();
					}
				}
			}
			else{
				if (Mathf.Abs(distToTarget) > 0.25f && Mathf.Abs(distToTarget) < enemyProperties.meleeDist && Mathf.Abs(distToTargetY) < enemyProperties.meleeDist)
				{
					if (canAttack)
					{
						DoAttack();
					}
				}
			}
		}
	}

	public void DoAttack()
	{
		DroneAnim.SetBool("Attack", true);
	}

	//this is called in animator
	public void InflictDamage()
	{
		if(canAttack)
		{
			// transform.GetComponent<Animator>().SetBool("Attack", true);
			Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackArea);
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
			StartCoroutine(EndAttack(0.3f));
		}

	}

	IEnumerator EndAttack(float time)
	{
		canAttack = false;
		yield return new WaitForSeconds(time);
		DroneAnim.SetBool("Attack", false);
		canAttack = true;
	}
	
	void PopCounter()
	{
		DroneAnim.Play("countdown_pop");
		//begin counting
		StartCoroutine(Countdown(countdownTime));
		Debug.Log("pop Countdown");
		reachedDestination = true;
		searchingForTarget = false;
	}

	//simple countdown timer
	IEnumerator Countdown(int countdownTime)
	{
		yield return new WaitForSeconds(1f);

		while (countdownTime > 0)
		{
			yield return new WaitForSeconds(1f);
			countdownTime--;
			CountdownText.text = countdownTime.ToString();
		}

		if (countdownTime == 0)
		{
			//explode and destroy Tree
			Debug.Log("Countdown complete!");
		}
	}

	public void DamageEnemy(float damage)
	{
		if (!isInvincible)
		{	
			StartCoroutine(HitTime());		

			float direction = damage / Mathf.Abs(damage);
			damage = Mathf.Abs(damage);

			transform.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * 10f, 5f)); 
			SoundManager.instance.PlaySFX("Hit_Enemy");

			curHealth -= (int)damage;

			if (curHealth <= 0 && !isDead)
			{
				StartCoroutine(DestroyEnemy());
			}
		}
	}

	IEnumerator DestroyEnemy()
	{
		isDead = true;

		if (UIController.instance != null)
        {
            UIController.instance.curKills++;
        }
		
		ItemDropper.instance.DropItem("Coin", transform.position);

		DroneAnim.SetBool("IsDead", true);
		rb.mass = 5f;
		rb.gravityScale = 5f;
		// this.GetComponent<CapsuleCollider2D>().enabled = false;
		yield return new WaitForSeconds(1.5f);
		Destroy(gameObject);
		
		// //sound
		SoundManager.instance.PlaySFX(enemyProperties.deathSoundName);
	}

	void OnCollisionEnter2D(Collision2D _colInfo)
	{
		//Only normal enemies can hurt player on collision 
		if (_colInfo.gameObject.tag == "Ground")
		{
			DroneAnim.SetBool("Land", true);
		}
	}

	IEnumerator HitTime()
	{
		DroneAnim.SetBool("Hit", true);
		yield return new WaitForSeconds(0.15f);
		DroneAnim.SetBool("Hit", false);
		yield break;
	}

	#region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
		if (attackCheck != null)
			Gizmos.DrawWireSphere(attackCheck.position, attackArea);
    }
    #endregion

}
