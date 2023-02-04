using UnityEngine;
using System.Collections;
using Pathfinding;
using TMPro;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
public class PlayerCompanion : MonoBehaviour 
{
	public static PlayerCompanion instance;

	public PlayerCompanionStats companionProperties;
	public Transform missileSpot;
	public bool canShoot = false;
	public bool waveCompleted = false;
	// What to chase?
	private Transform target;
	public Animator DroneAnim;
	
	// How many times each second we will update our path
	public float updateRate = 2f;
	public Transform enemyGFX;
	// Caching
	private Seeker seeker;
	private Rigidbody2D rb;
	
	//The calculated path
	public Path path;
		
	[HideInInspector]
	public bool pathIsEnded = false;
	
	// The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	
	// The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	private bool searchingForTarget = false;
	private bool isInvincible = false;
	private bool reachedDestination = false;
	// private bool isHit = false;
	private int _curHealth;
	public int curHealth
	{
		get { return _curHealth; }
		set { _curHealth = Mathf.Clamp (value, 0, companionProperties.maxHealth); }
	}

	[HideInInspector] public int dealingDamage;
    [HideInInspector] public float moveSpeed;
	[HideInInspector] public float fireRate;
	[HideInInspector] public float missileSpeed;

	private void Awake() {
		instance = this;
	}

	public void Init()
	{
		curHealth = companionProperties.maxHealth;

		dealingDamage = companionProperties.dealingDamage;
		moveSpeed = companionProperties.moveSpeed;
		fireRate = companionProperties.fireRate;
		missileSpeed = companionProperties.missileSpeed;
	}
	
	void Start () {
		if (companionProperties != null)
		{
			Init();
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

	public void Shoot()
	{
		StartCoroutine (ShootMissile());
	}

	public void ExitShoot()
	{
		StopCoroutine (ShootMissile());
	}

	IEnumerator searchForTarget()
	{
		GameObject sResult = GameObject.FindGameObjectWithTag (companionProperties.target);
		if (sResult == null) 
		{
			yield return new WaitForSeconds (0.5f);
			StartCoroutine (searchForTarget ());
		}
		else 
		{
			target = sResult.transform;
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
		if (!p.error) {
			path = p;
			currentWaypoint = 0;
		}
	}
	
	void FixedUpdate () {
		if (path == null)
			return;
		
		if (currentWaypoint >= path.vectorPath.Count) {
			if (pathIsEnded)
				return;
			
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;
	
		//Direction to the next waypoint
		Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
		Vector2 force = dir * moveSpeed * Time.fixedDeltaTime;

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
	}

	public void Damage(float damage)
	{
		if (!isInvincible)
		{
			StartCoroutine(HitTime());
			
			float direction = damage / Mathf.Abs(damage);
			damage = Mathf.Abs(damage);

			transform.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * 10f, 5f)); 

			curHealth -= (int)damage;

			if (curHealth <= 0)
			{
				StartCoroutine(DestroyDrone());
			}
		}
	}

	IEnumerator DestroyDrone()
	{
		if (UIController.instance != null)
        {
            UIController.instance.curKills++;
        }
		
		ItemDropper.instance.DropItem("Coin", transform.position);
		rb.bodyType = RigidbodyType2D.Static;
		GetComponent<BoxCollider2D>().enabled = false;;

		transform.GetComponent<Animator>().SetBool("IsDead", true);
		yield return new WaitForSeconds(1f);
		Destroy(gameObject);
				
		// //sound
		SoundManager.instance.PlaySFX(companionProperties.deathSoundName);
	}

	IEnumerator HitTime()
	{
		// isInvincible = true;
		// isHit = true;
		DroneAnim.SetBool("Hit", true);
		yield return new WaitForSeconds(0.5f);
		// isHit = false;
		DroneAnim.SetBool("Hit", false);
		// isInvincible = false;
	}

	IEnumerator ShootMissile()
	{
		yield return new WaitForSeconds(fireRate);
		GameObject missile = ObjectPooler.SharedInstance.GetPooledObject("Missile");
		missile.GetComponent<HomingMissile>().ActivateMissile();
		missile.transform.position = missileSpot.transform.position;

		if (canShoot)
		{
			StartCoroutine (ShootMissile());
		}

		if (waveCompleted)
		{
			missile.SetActive(false);
		}else{
			missile.SetActive(true);
		}
	}
}
