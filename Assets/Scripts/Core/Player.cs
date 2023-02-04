using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour 
{
	public static Player instance;

	private SoundManager audioManager;
	public PlayerMovement playerMovement;
	PlayerAnimator playerAnimator;

	public PlayerStats stats;

	[SerializeField]
	private HealthBar healthBar;
	public bool isInvincible;
	[HideInInspector] public float healthRegenInterval;
	[HideInInspector] public float healthRegenRate, reductionMultiplier, refillMultiplier;
	public bool canRegenerate,isRegenerating;


	private void Awake() {
		instance = this;

		healthRegenInterval = stats.healthRegenInterval;
		healthRegenRate = stats.healthRegenRate;
		reductionMultiplier = stats.oxygenReductionMultiplier;
		refillMultiplier = stats.oxygenRefillMultiplier;
		
		playerMovement = PlayerMovement.instance;
		playerAnimator = PlayerAnimator.instance;
	}

	void Start()
	{
		stats.curHealth = stats.maxHealth;

		audioManager = SoundManager.instance;
		if (audioManager == null)
		{
			Debug.LogError("FREAK OUT! No AudioManager found in the scene.");
		}
	}


	void Update () {
		if (transform.position.y <= stats.fallBoundary)
		{
			DamagePlayer (995f);
		}

		if (Input.GetKeyDown("m")) {
			DamagePlayer(10);
			GameObject pill = ObjectPooler.SharedInstance.GetPooledObject("Pill");
			pill.SetActive(true);
		}	
	}

	void HealthReduction()
	{
		DamagePlayer(5f);
	}


	public bool NeedsHealth()
	{
		bool needsHealth = false;
		if (stats.curHealth < stats.maxHealth)
		{
			needsHealth = true;
		}
		return needsHealth;
	}

	public void RestoreHealth(int restoreAmount)
	{
		healthBar.HealDamage(restoreAmount); 
	}

	public void DamagePlayer (float damage) 
	{
		healthBar.TakeDamage(damage -= stats.damageResistance);
		//if we're not invincible then let's damage player
		if (!isInvincible)
		{
			if (stats.curHealth <= 0) {
				audioManager.PlaySFX (stats.deathSoundName);
				KillPlayer();
			} 
			else 
			{
				audioManager.PlaySFX (stats.hitSoundName);
			}
		}
	}

	public void KillPlayer()
	{
		StartCoroutine(WaitToDead());
	}

	IEnumerator WaitToDead()
	{
		playerAnimator.anim.SetBool("IsDead", true);
		yield return new WaitForSeconds(0.4f);
		playerMovement.RB.velocity = new Vector2(0, playerMovement.RB.velocity.y);
		yield return new WaitForSeconds(1.1f);
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}


	IEnumerator MakeInvincible(float time) 
	{
		isInvincible = true;
		yield return new WaitForSeconds(time);
		isInvincible = false;
		yield break;
	}
}
