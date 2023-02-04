using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{
	public static Weapon instance;
	public SpriteRenderer WeaponImage;
	public LayerMask whatToHit;
	public Transform HitPrefab; 
	public Transform BulletTrailPrefab;
	public Transform MuzzleFlashPrefab;
	float timeToSpawnEffect = 0;
	private int currentAmmo = -1;
	private bool isReloading = false;
	public Animator animator;
	CameraShake camShake;
	float timeToFire = 0;
	public Transform firePoint;

	SoundManager audioManager;

	public WeaponStats weaponStats;

	[HideInInspector] public int Damage, maxAmmo;
	[HideInInspector] public float fireRate, effectSpawnRate, reloadTime;

	private void Awake() {
		instance = this;
	}

	void Start()
	{
		WeaponImage.sprite = weaponStats.weaponImage;

		camShake = GameMaster.instance.GetComponent<CameraShake> ();
		if (camShake == null)
			Debug.LogError ("No CamShake");

		audioManager = SoundManager.instance;
		if (audioManager == null)
		{
			Debug.LogError("FREAK OUT! No AudioManager found in the scene.");
		}

		Damage = weaponStats.Damage;
		maxAmmo = weaponStats.maxAmmo;
		fireRate = weaponStats.fireRate;
		effectSpawnRate = weaponStats.effectSpawnRate;
		reloadTime = weaponStats.reloadTime;

		if (currentAmmo == -1)
			currentAmmo = maxAmmo; 
	}

	public void UpdateChangedWeapon(WeaponStats weaponStats)
	{
		WeaponImage.sprite = weaponStats.weaponImage;
		currentAmmo = weaponStats.maxAmmo; 
	}

	
	// Update is called once per frame
	void FixedUpdate () {
		if (isReloading)
			return;

		if (currentAmmo <= 0) 
		{
			StartCoroutine(Reload ());
			return;
		} 
		
		if (fireRate == 0) {
			if (Input.GetButtonDown ("Fire1")) {
				Shoot();
			}
		}
		else {
			if (Input.GetButton ("Fire1") && Time.time > timeToFire) {
				timeToFire = Time.time + 1/fireRate;
				Shoot();
			}
		}
	}

	IEnumerator Reload()
	{
		isReloading = true;
		// Debug.Log ("Reloading.....");

		animator.SetBool ("Reloading", true);
		audioManager.PlaySFX ("Reloading");

		yield return new WaitForSeconds (reloadTime - .25f);
		animator.SetBool ("Reloading", false);
		yield return new WaitForSeconds (.25f);

		currentAmmo = maxAmmo;
		isReloading = false;
	}
	
	void Shoot () {

		currentAmmo--;

		Vector2 mousePosition = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
		Vector2 firePointPosition = new Vector2 (firePoint.position.x, firePoint.position.y); 
		RaycastHit2D hit = Physics2D.Raycast (firePointPosition, mousePosition-firePointPosition, 100, whatToHit);


		Debug.DrawLine (firePointPosition, (mousePosition-firePointPosition)*100, Color.cyan);
		if (hit.collider != null) {
			Debug.DrawLine (firePointPosition, hit.point, Color.red);
			Enemy enemy = hit.collider.GetComponent<Enemy>();
			EnemyAI enemyAI = hit.collider.GetComponent<EnemyAI>();
			if (enemy != null)
			{
				enemy.DamageEnemy (Damage);
			}

			if (enemyAI != null)
			{
				enemyAI.DamageEnemy (Damage);
			}
		}

		if (Time.time >= timeToSpawnEffect)
		{
			Vector3 hitPos;
			Vector3 hitNormal;

			if (hit.collider == null) 
			{
				hitPos = (mousePosition - firePointPosition) * 30;
				hitNormal = new Vector3(9999, 9999, 9999);
			}
			else
			{
				hitPos = hit.point;
				hitNormal = hit.normal;
			}

			Effect(hitPos, hitNormal);
			timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
		}
	}

	void Effect(Vector3 hitPos, Vector3 hitNormal)
	{
		GameObject trail = ObjectPooler.SharedInstance.GetPooledObject("BulletTrail") as GameObject;
		LineRenderer lr = trail.GetComponent<LineRenderer>();
		trail.transform.position = firePoint.position;
		trail.transform.rotation = firePoint.rotation;
		trail.SetActive(true);

		if (lr != null)
		{
			lr.SetPosition(0, firePoint.position);
			lr.SetPosition(1, hitPos);
		}
		ObjectPooler.SharedInstance.TakePooledObject(trail, 0.02f);
		 
		if (hitNormal != new Vector3(9999, 9999, 9999))
		{
			GameObject hitParticle = ObjectPooler.SharedInstance.GetPooledObject("BulletHit") as GameObject;
			hitParticle.transform.position = hitPos;
			hitParticle.transform.rotation = Quaternion.FromToRotation (Vector3.right, hitNormal);
			hitParticle.SetActive(true);

			ObjectPooler.SharedInstance.TakePooledObject(hitParticle, 1f);
		}

		GameObject clone = ObjectPooler.SharedInstance.GetPooledObject("MuzzleFlash") as GameObject;
		clone.transform.position = firePoint.position;
		float size = Random.Range (1f, 0.8f);
		clone.transform.localScale = new Vector3 (size, size, size);
		clone.SetActive(true);
		ObjectPooler.SharedInstance.TakePooledObject(clone, 0.02f);

		//Shake Cam
		camShake.Shake (weaponStats.camShakeAmt, weaponStats.camShakeLenght);

		//PlayShootSound
		audioManager.PlaySFX(weaponStats.weaponShootSound);
	}


}
